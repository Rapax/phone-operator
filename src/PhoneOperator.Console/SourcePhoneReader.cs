using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PhoneOperator.Console
{
    public class SourcePhoneReader
    {
        public ICollection<string> LoadNumbers(string filePath, PhoneCodeTransformer phoneCodeTransformer)
        {
            var fi = new FileInfo(filePath);
            var result = new List<string>();

            if (fi.Exists)
            {
                using (var fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                using (var sr = new StreamReader(fs))
                {
                    string line;
                    while ((line = sr.ReadLine()) != null)
                    {
                        var options = phoneCodeTransformer.Transform(line);
                        result.AddRange(options);
                    }
                }
            }

            return result;
        }
    }
}