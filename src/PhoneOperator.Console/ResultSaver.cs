using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using PhoneOperator.Console.Models;

namespace PhoneOperator.Console
{
    public class ResultSaver
    {
        private const string fileName = "result.csv";
        private const string NotFound = "Operatör saknas";
        
        public void Save(IEnumerable<PhoneViewModel> models)
        {
            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }

            var all = models.ToList();
            var found = all.Where(x => !x.Name.Equals(NotFound, StringComparison.InvariantCultureIgnoreCase));
            var notFound = all.Where(x => x.Name.Equals(NotFound, StringComparison.InvariantCultureIgnoreCase));

            var allTask = Task.Factory.StartNew(() =>
            {
                using (var stream = File.CreateText($"all-{fileName}"))
                {
                    foreach (var model in all)
                    {
                        string csvRow = $"{model.Name},{model.Number}";
                        stream.WriteLine(csvRow);
                    }
                }
            });

            var foundTask = Task.Factory.StartNew(() =>
            {
                using (var stream = File.CreateText($"found-{fileName}"))
                {
                    foreach (var model in found)
                    {
                        string csvRow = $"{model.Name},{model.Number}";
                        stream.WriteLine(csvRow);
                    }
                }
            });

            var notFoundTask = Task.Factory.StartNew(() =>
            {
                using (var stream = File.CreateText($"notfound-{fileName}"))
                {
                    foreach (var model in notFound)
                    {
                        string csvRow = $"{model.Name},{model.Number}";
                        stream.WriteLine(csvRow);
                    }
                }
            });

            Task.WaitAll(allTask, foundTask, notFoundTask);
        }
    }
}