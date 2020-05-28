using System.Collections.Generic;
using System.IO;
using PhoneOperator.Console.Models;

namespace PhoneOperator.Console
{
    public class ResultSaver
    {
        private const string fileName = "result.csv";
        public void Save(IEnumerable<PhoneViewModel> models)
        {
            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }
            
            using (var stream = File.CreateText(fileName))
            {
                foreach (var model in models)
                {
                    string csvRow = $"{model.Name},{model.Number}";
                    stream.WriteLine(csvRow);
                }
            }
        }
    }
}