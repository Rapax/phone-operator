using System;
using System.Collections;
using System.Collections.Concurrent;
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
            var phoneOperatorManager = new PhoneOperatorManager();
            
            phoneOperatorManager.CollectInformation(args[1], args[0]);
            System.Console.WriteLine("All records processed!");
            
            phoneOperatorManager.SaveResult();
        }
    }
}
