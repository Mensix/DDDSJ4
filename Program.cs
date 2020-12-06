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
            if (args.Length != 1)
            {
                Console.WriteLine("! No input file was specified");
                Environment.Exit(0);
            }

            List<string> inputFileContent = new();
            List<ObjVertex> vertices = new();
            List<ObjFace> faces = new();
            string outputXml;

            try
            {
                inputFileContent = File.ReadAllLines(args[0]).ToList();
                Console.WriteLine("! Reading file");
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine($"! File {args[0]} was not found");
                Environment.Exit(0);
            }
            catch (FileLoadException)
            {
                Console.WriteLine($"! Unable to load {args[0]} file");
                Environment.Exit(0);
            }

            ParseObj parser = new();
            Console.WriteLine("! Parsing file");
            (vertices, faces) = parser.Parse(inputFileContent);
            Console.WriteLine("! Generating XML");
            outputXml = parser.Generate(vertices, faces);
            Console.WriteLine("! Writing XML");
            File.WriteAllText($"{args[0].Split(".")[0]}.xml", outputXml);
            Console.WriteLine("! Output file succesfully written");
        }
    }
}
