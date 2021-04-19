using System;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using System.Drawing;
using Console = Colorful.Console;
using DDDSJ4.Parsers;
using DDDSJ4.Models;

namespace DDDSJ4
{
    class Program
    {
        static void Main(string[] args)
        {
            List<string> objFileContent = new();
            List<string> mtlFileContent = new();

            Console.Write("INFO: ", Color.Blue);
            Console.Write("Reading files...\n");

            try
            {
                objFileContent = File.ReadAllLines(args[0]).ToList();
                if(args.ElementAtOrDefault(1) != null) mtlFileContent = File.ReadAllLines(args[1]).ToList();
            }
            catch (FileNotFoundException)
            {
                Console.Write("ERROR: ", Color.Red);
                Console.Write($"Files {args[0]} and/or {args[1]} weren't found!");
                Environment.Exit(1);
            }

            Console.Write("INFO: ", Color.Blue);
            Console.Write("Invoking parsers...\n");

            ParseMtl parseMtl = new();
            Console.Write("INFO: ", Color.Blue);
            Console.Write("Parsing materials...\n");
            List<MtlMaterial> materials = parseMtl.Parse(mtlFileContent);

            ParseObj parseObj = new();
            Console.Write("INFO: ", Color.Blue);
            Console.Write("Parsing obj...\n");
            List<ObjBatch> batches = parseObj.Parse(objFileContent, materials);

            Console.Write("INFO: ", Color.Blue);
            Console.Write("Writing XML file...\n");
            parseObj.Generate(batches, $"{args[0].Split(".")[0]}.xml");

            Console.Write("INFO: ", Color.Blue);
            Console.Write($"XML file was written to {args[0].Split(".")[0]}.xml");
            Environment.Exit(0);
        }
    }
}
