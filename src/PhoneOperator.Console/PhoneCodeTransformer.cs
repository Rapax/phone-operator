using System.Collections.Generic;
using System.Linq;

namespace PhoneOperator.Console
{
    public class PhoneCodeTransformer
    {
        private Dictionary<char, List<string>> _codes;
        
        public void Initialize(IEnumerable<string> phoneCodes)
        {
            _codes = new Dictionary<char, List<string>>();

            foreach (var phoneCode in phoneCodes)
            {
                var code = phoneCode[1];
                if (_codes.ContainsKey(code))
                {
                    _codes[code].Add(phoneCode);
                }
                else
                {
                    _codes.Add(code, new List<string>{phoneCode});
                }
            }
        }

        public IEnumerable<string> Transform(string original)
        {
            var code = original[1];
            var result = new List<string>();
            if (_codes.TryGetValue(code, out List<string> options))
            {
                var foundItems = options.Where(original.Contains);
                foreach (var foundItem in foundItems)
                {
                    result.Add(original.Insert(foundItem.Length, "-").Substring(1));
                }
            }
            else
            {
                result.Add(original);
            }

            return result;
        }
    }
}