using System.Text;
using System.Linq;
using System;
using System.Collections.Generic;
using DDDSJ4.Models;
using DDDSJ4.Interfaces;

namespace DDDSJ4.Parsers
{
    public class ParseObj : IParse
    {
        public (List<ObjVertex>, List<ObjFace>) Parse(List<string> input)
        {
            List<ObjVertex> vertices = new();
            List<ObjFace> faces = new();
            int index = 1;

            for (int i = 0; i < input.Count; i++)
            {
                List<string> line = input[i].Split(" ").ToList();

                if (line[0].StartsWith("f") && line.Count != 4)
                {
                    Console.WriteLine($"! .obj file must contain three values in vertex and face definition, found at line {i + 1}, this may cause not desired 3dmodel rendering in-game");
                }

                if (line[0].StartsWith("vn"))
                {
                    Console.WriteLine($"! vn definition was found in .obj file at line {i + 1}, this may cause not desired 3dmodel rendering in-game");
                }

                if (line[0].StartsWith("v") && line[0].EndsWith("v"))
                {
                    vertices.Add(new ObjVertex
                    {
                        Id = index,
                        X = line[1],
                        Y = line[2],
                        Z = line[3]
                    });
                    index++;
                }
                else if (line[0].StartsWith("f"))
                {
                    faces.Add(new ObjFace
                    {
                        V1 = line[1],
                        V2 = line[2],
                        V3 = line[3]
                    });
                }
            }

            return (vertices, faces);
        }

        public string Generate(List<ObjVertex> vertices, List<ObjFace> faces)
        {
            StringBuilder sb = new StringBuilder("<3dmodel id=\"model\">\n\t<batch id=\"batch\" texture1=\"Textures\\concrete5.png\" material=\"Materials\\material1.xml\" fvf=\"322\" order=\"0\">");

            for (int i = 0; i < vertices.Count; i++)
            {
                sb.Append("\n\t\t<vertex id=\"").Append(vertices[i].Id).Append("\" x=\"").Append(vertices[i].X).Append("\" y=\"").Append(vertices[i].Y).Append("\" z=\"").Append(vertices[i].Z).Append("\" diffuse=\"0xFFFFFF\" />");
            }

            for (int i = 0; i < faces.Count; i++)
            {
                sb.Append("\n\t\t<face v1=\"").Append(faces[i].V1).Append("\" v2=\"").Append(faces[i].V2).Append("\" v3=\"").Append(faces[i].V3).Append("\" />");
            }

            sb.Append("\n\t</batch>\n</3dmodel>\n<3dmodel-instance id=\"model\" x=\"0\" y=\"0\" z=\"0\" refx=\"inrun\" refy=\"terrain\" />");
            return sb.ToString();
        }
    }
}