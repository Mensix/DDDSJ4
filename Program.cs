using System.Linq;
using System.IO;
using System;
using System.Collections.Generic;
using DDDSJ4.Parsers;
using DDDSJ4.Models;
using DDDSJ4.Utilities;

namespace DDDSJ4
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            string[] objFileContent = Array.Empty<string>();
            string[] mtlFileContent = Array.Empty<string>();
            ParseMtl parseMtl = new();
            ParseObj parseObj = new();

            Logger.LogInfo("Reading files...");
            string[] objAndMtlFiles = Directory
                 .GetFiles(Environment.CurrentDirectory, "*.*", SearchOption.TopDirectoryOnly)
                 .Select(x => Path.GetFileName(x))
                 .Where(x => x.Split(".").Length > 1)
                 .GroupBy(x => x.Split(".")[0])
                 .First(x => x.Any(y => y.Contains(".obj") || y.Contains(".mtl")))
                 .ToArray();

            string outputFileName = objAndMtlFiles[0].Split(".")[0];
            objFileContent = File.ReadAllLines(objAndMtlFiles.First(x => x.EndsWith(".obj")));
            if(objAndMtlFiles.Any(x => x.EndsWith(".mtl"))) mtlFileContent = File.ReadAllLines(objAndMtlFiles.First(x => x.EndsWith(".mtl")));

            Logger.LogInfo("Invoking parsers...");
            Logger.LogInfo("Parsing materials...");
            List<MtlMaterial> materials = parseMtl.Parse(mtlFileContent);
            Logger.LogInfo("Parsing obj...");
            List<ObjBatch> batches = parseObj.Parse(objFileContent, materials);

            Logger.LogInfo("Writing XML file...");
            parseObj.Generate(batches, $"{outputFileName}.xml");
            Logger.LogInfo($"XML file was written to {outputFileName}.xml");
            Environment.Exit(0);
        }
    }
}
