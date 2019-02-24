using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AWSCliExtender
{
    class Program
    {
        static void DictionaryPrint(Dictionary<string, string> Dictionary)
        {
            foreach (var key in Dictionary)
                Console.WriteLine("Key: " + key.Key + " => " + key.Value);
        }

        static void Main(string[] args)
        {
            ArgumentParser.Parse(args);
            DictionaryPrint(ArgumentParser.ArgsDict);

            /// IN ALPHA VERSION, PROGRAM CAN ONLY BE USED WITH AWS PROFILES = ACCESS KEY-SECRET KEY METHODS WILL BE CODED
            AWSCredential.Initialize(ArgumentParser.ArgsDict["--PROFILE"]);

            switch (ArgumentParser.ArgsDict["SERVICE"])
            {
                case "IAM":
                    {
                        switch (ArgumentParser.ArgsDict["OPERATION"])
                        {
                            case "CREATE-POLICY-EXT":
                                {
                                    IAM.CreatePolicyExt(ArgumentParser.ArgsDict["--POLICY-NAME"], ArgumentParser.ArgsDict["--POLICY-DOCUMENT-FILE-PATH"], ArgumentParser.ArgsDict.ContainsKey("--NO-SET-AS-DEFAULT"), ArgumentParser.ArgsDict.ContainsKey("--NO-REMOVE-OLDEST-POLICY-VERSION"));
                                    break;
                                }
                        }
                        break;
                    }
            }

            Console.ReadKey();
        }
    }
}

