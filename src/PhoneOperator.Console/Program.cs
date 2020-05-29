using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PhoneOperator.Console.Models;

namespace PhoneOperator.Console
{
    class Program
    {
        static async Task Main(string[] args)
        {
            const int batchSize = 100;
            var sourceReader = new SourcePhoneReader();
            var serviceClient = new NumberServiceClient();
            var resultSaver = new ResultSaver();
            
            var phoneNumbers = sourceReader.LoadNumbers(args[0]);
            var result = new List<PhoneViewModel>();

            int processedCount = 0;

            while (processedCount < phoneNumbers.Count)
            {
                var restItemsCount = phoneNumbers.Count - processedCount;
                System.Console.WriteLine($"Processed {processedCount} records, there {restItemsCount} records left to process.");
                var numbers = phoneNumbers.Skip(processedCount).Take(restItemsCount >= batchSize ? batchSize : restItemsCount);
                processedCount += restItemsCount >= batchSize ? batchSize : restItemsCount;
                result.AddRange(await serviceClient.RequestInformation(numbers));
            }
            
            resultSaver.Save(result);
            
            System.Console.WriteLine("All records processed!");
        }
    }
}
