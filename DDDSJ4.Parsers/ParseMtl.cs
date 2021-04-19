using System.Globalization;
using System;
using System.Net;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using DDDSJ4.Models;

namespace DDDSJ4.Parsers
{
    public class ParseMtl
    {
        public List<MtlMaterial> Parse(string[] input)
        {
            List<MtlMaterial> materials = new();

            for (int i = 0; i < input.Length; i++)
            {
                string[] line = input[i].Split(" ");
                if (line[0] == "newmtl")
                {
                    materials.Add(new MtlMaterial
                    {
                        Name = line[1]
                    });
                }

                if (line[0] == "Kd")
                {
                    int r = Convert.ToInt32(float.Parse(line[1], CultureInfo.InvariantCulture.NumberFormat) * 255);
                    int g = Convert.ToInt32(float.Parse(line[2], CultureInfo.InvariantCulture.NumberFormat) * 255);
                    int b = Convert.ToInt32(float.Parse(line[3], CultureInfo.InvariantCulture.NumberFormat) * 255);

                    materials[^1].Diffuse = RgbToHex(r, g, b);
                }
            }

            return materials;
        }

        public static string RgbToHex(int r, int g, int b)
        {
            Color color = Color.FromArgb(r, g, b);
            return $"0x{color.R:X2}{color.G:X2}{color.B:X2}";
        }
    }
}