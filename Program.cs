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
                Console.WriteLine("! Reading file");
                objFileContent = File.ReadAllLines(args[0]).ToList();
                mtlFileContent = File.ReadAllLines(args[1]).ToList();
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

            Console.WriteLine("! Invoking parsers");
            ParseObj parseObj = new();
            ParseMtl parseMtl = new();

            Console.WriteLine("! Parsing materials");
            List<MtlMaterial> materials = parseMtl.Parse(mtlFileContent);
            Console.WriteLine("! Parsing .obj file");
            List<ObjBatch> batch = parseObj.Parse(objFileContent, materials);
            Console.WriteLine("! Writing XML file");
            File.WriteAllText($"{args[0].Split(".")[0]}.xml", parseObj.Generate(batch));
            Console.WriteLine($"! XML file was written to {args[0].Split(".")[0]}.xml");
        }
    }
}
