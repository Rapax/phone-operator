using System.Collections.Generic;
using System.IO;

namespace PhoneOperator.Console
{
    public class PhoneCodeReader
    {
        public ICollection<string> Read(string filePath)
        {
            var result = new List<string>();
            if (File.Exists(filePath))
            {
                using (var fs = File.Open(filePath, FileMode.Open, FileAccess.Read))
                using (var reader = new StreamReader(fs))
                {
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {
                        result.Add(line);
                    }
                }
            }

            return result;
        }
    }
}