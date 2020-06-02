using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using PhoneOperator.Console.Models;

namespace PhoneOperator.Console
{
    public class PhoneOperatorManager
    {
        const int BatchSize = 100;
        
        private SourcePhoneReader sourceReader = new SourcePhoneReader();
        private ResultSaver resultSaver = new ResultSaver();
        private PhoneCodeReader codeReader = new PhoneCodeReader();
        private PhoneCodeTransformer phoneCodeTransformer = new PhoneCodeTransformer();

        private readonly ConcurrentBag<PhoneViewModel> _result = new ConcurrentBag<PhoneViewModel>();
        
        public void CollectInformation(string phoneCodesFilePath, string phonesFilePath)
        {
            var codes = codeReader.Read(phoneCodesFilePath);
            phoneCodeTransformer.Initialize(codes.OrderBy(x => x));

            var phoneNumbers = sourceReader.LoadNumbers(phonesFilePath, phoneCodeTransformer);

            var batches = PrepareBatches(phoneNumbers);
            
            var options = new ParallelOptions
            {
                MaxDegreeOfParallelism = Environment.ProcessorCount
            };

            Parallel.ForEach(batches, options, BatchProcessor);
        }

        public void SaveResult()
        {
            resultSaver.Save(_result.ToArray().OrderBy(x => x.Number));
        }
        
        private List<IEnumerable<string>> PrepareBatches(ICollection<string> original)
        {
            var processorsCount = Environment.ProcessorCount;
            var batchSize = (int)Math.Ceiling(original.Count / (double)processorsCount);
            List<IEnumerable<string>> batches = new List<IEnumerable<string>>(processorsCount);

            for (int i = 0; i < processorsCount; i++)
            {
                var batch = original.Skip(i * batchSize).Take(
                    original.Count - i * batchSize > batchSize
                        ? batchSize
                        : original.Count - i * batchSize);
                
                batches.Add(batch);
            }
            
            System.Console.WriteLine($"Where are {batches.Count} batches prepared.");
            return batches;
        }
        
        private void BatchProcessor(IEnumerable<string> phoneNumbers)
        {
            var client = new NumberServiceClient();
            int processedCount = 0;
            var list = phoneNumbers.ToList();

            while (processedCount < list.Count)
            {
                var restItemsCount = list.Count - processedCount;
                System.Console.WriteLine($"[Worker# {Thread.CurrentThread.ManagedThreadId}] Processed {processedCount} records, {restItemsCount} records left to be processed.");
                var numbers = list.Skip(processedCount).Take(restItemsCount >= BatchSize ? BatchSize : restItemsCount);
                processedCount += restItemsCount >= BatchSize ? BatchSize : restItemsCount;

                var response = client.RequestInformation(numbers).GetAwaiter().GetResult();
                foreach (var viewModel in response)
                {
                    _result.Add(viewModel);
                }
            }
        }
    }
}