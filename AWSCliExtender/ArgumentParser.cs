using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AWSCliExtender
{
    static class ArgumentParser
    {
        public static Dictionary<string, string> ArgsDict;

        public static void Parse(string[] args)
        {
            ArgsDict = new Dictionary<string, string>();
            if (args.Count() < 2)
            {
                throw new Exception("Minimum 2 arguments should be given to the program. Service and operation arguments did not recognized.");
            }

            ArgsDict.Add("SERVICE", args[0].ToUpperInvariant());
            ArgsDict.Add("OPERATION", args[1].ToUpperInvariant());

            for (int i = 2; i < args.Length; i++)   // First 2 argument already set.
            {
                if (i != args.Length-1)
                {
                    if (args[i].StartsWith("--") && (!(args[i + 1].StartsWith("--"))))
                    {
                        ArgsDict.Add(args[i].ToUpperInvariant(), args[i + 1]);
                        i++; // Next Argument is value because of that set and skipped.
                    }
                    else
                        ArgsDict.Add(args[i], "TRUE"); // If Argument does not have value part that means it is enabling the parameter.
                }
                else
                    ArgsDict.Add(args[i], "TRUE"); // If It is the latest argument that means it is the true/false parameter.
            }
        }
    }
}
