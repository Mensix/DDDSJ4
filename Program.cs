using System.Linq;
using System.IO;
using System;
using DDDSJ4.Parsers;
using System.Collections.Generic;
using DDDSJ4.Models;

namespace DDDSJ4
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length != 2)
            {
                Console.WriteLine("! No input file was specified");
                Environment.Exit(0);
            }

            List<string> objFileContent = new();
            List<string> mtlFileContent = new();

            try
            {
                objFileContent = File.ReadAllLines(args[0]).ToList();
                mtlFileContent = File.ReadAllLines(args[1]).ToList();
                Console.WriteLine("! Reading file");
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine($"! File {args[0]} or/and {args[1]} was not found");
                Environment.Exit(0);
            }
            catch (FileLoadException)
            {
                Console.WriteLine($"! Unable to load {args[0]} or/and {args[1]} file");
                Environment.Exit(0);
            }

            ParseObj parseObj = new();
            ParseMtl parseMtl = new();
            List<MtlMaterial> materials = parseMtl.Parse(mtlFileContent);

            List<ObjBatch> batch = parseObj.Parse(objFileContent, materials);
            File.WriteAllText($"{args[0].Split(".")[0]}.xml", parseObj.Generate(batch));
        }
    }
}
