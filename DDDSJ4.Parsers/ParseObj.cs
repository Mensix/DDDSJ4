using System.Drawing;
using System.Xml;
using System.Linq;
using System;
using System.Collections.Generic;
using DDDSJ4.Models;
using DDDSJ4.Interfaces;
using Console = Colorful.Console;
namespace DDDSJ4.Parsers
{
    public class ParseObj : IParse
    {
        public List<ObjBatch> Parse(List<string> input, List<MtlMaterial> materials)
        {
            List<ObjBatch> batches = new();
            List<ObjVertex> vertices = new();
            List<ObjFace> faces = new();
            bool wasFaceInvalid = false;
            int index = 1;

            batches.Add(new ObjBatch
            {
                Id = "0",
                Diffuse = "0xFFFFFF",
                Faces = new List<ObjFace>(),
                Vertices = new List<ObjVertex>()
            });

            for (int i = 0; i < input.Count; i++)
            {
                List<string> line = input[i].Split(" ").ToList();

                if (line[0].StartsWith("v") && line[0].EndsWith("v"))
                {
                    if (line.Count != 4)
                    {
                        Console.Write("ERROR: ", Color.Red);
                        Console.Write("Vertex definition must contain exactly three values\n");
                        Environment.Exit(1);
                    }

                    vertices.Add(new ObjVertex
                    {
                        Id = index,
                        X = line[1],
                        Y = line[2],
                        Z = line[3]
                    });
                    index++;
                }

                if (line[0].StartsWith("usemtl"))
                {
                    bool wasDiffuseFound = materials.Where(x => x.Name == line[1]).ToList().Count > 0;
                    if (!wasDiffuseFound)
                    {
                        Console.Write("WARNING: ", Color.Gold);
                        Console.Write($"Material {line[1]} diffuse wasn't found, using 0xFFFFFF color...\n");
                    }

                    Random random = new();
                    batches.Add(new ObjBatch
                    {
                        Id = $"{line[1]}_{random.NextDouble()}",
                        Diffuse = wasDiffuseFound ? materials.Find(x => x.Name == line[1]).Diffuse : "0xFFFFFF",
                        Vertices = new List<ObjVertex>(),
                        Faces = new List<ObjFace>()
                    });
                }

                if (line[0].StartsWith("f"))
                {
                    if (line.Count != 4 && !wasFaceInvalid)
                    {
                        wasFaceInvalid = true;
                        Console.Write("WARNING: ", Color.Gold);
                        Console.Write("Face definition must contain exactly three vertices identifications, triangulate your model ie. in Blender\n");
                    }

                    batches[^1].Faces.Add(new ObjFace
                    {
                        Type = GetFaceType(line[1]),
                        V1 = line[1],
                        V2 = line[2],
                        V3 = line[3]
                    });
                }
            }

            for (int i = 0; i < batches.Count; i++)
            {
                for (int j = 0; j < batches[i].Faces.Count; j++)
                {
                    ObjFace currentFace = batches[i].Faces[j];
                    if (currentFace.Type != ObjFaceType.NORMAL_INDICE)
                    {
                        batches[i].Vertices.Add(vertices.Find(x => x.Id.ToString() == currentFace.V1));
                        batches[i].Vertices.Add(vertices.Find(x => x.Id.ToString() == currentFace.V2));
                        batches[i].Vertices.Add(vertices.Find(x => x.Id.ToString() == currentFace.V3));
                    }
                    else
                    {
                        batches[i].Vertices.Add(vertices.Find(x => x.Id.ToString() == currentFace.V1.Split("/")[0]));
                        batches[i].Vertices.Add(vertices.Find(x => x.Id.ToString() == currentFace.V2.Split("/")[0]));
                        batches[i].Vertices.Add(vertices.Find(x => x.Id.ToString() == currentFace.V3.Split("/")[0]));
                    }
                }
            }

            for (int i = 0; i < batches.Count; i++)
            {
                batches[i].Vertices = batches[i].Vertices.Distinct().ToList();
            }

            return batches;
        }

        public void Generate(List<ObjBatch> batch, string fileName)
        {
            XmlWriter xmlWriter = XmlWriter.Create(fileName, new XmlWriterSettings
            {
                Indent = true,
                OmitXmlDeclaration = true,
            });
            xmlWriter.WriteStartDocument();
            xmlWriter.WriteComment("To place your generated model as you want, go to the last line of the code and modify model-instance element attributes.");
            xmlWriter.WriteStartElement("model");
            xmlWriter.WriteAttributeString("id", "model");

            for (int i = 0; i < batch.Count; i++)
            {
                xmlWriter.WriteStartElement("batch");
                xmlWriter.WriteAttributeString("id", batch[i].Id);
                xmlWriter.WriteAttributeString("texture1", "Textures\\concrete5.png");
                xmlWriter.WriteAttributeString("material", "Materials\\material1.xml");
                xmlWriter.WriteAttributeString("fvf", "322");
                xmlWriter.WriteAttributeString("order", "0");

                for (int j = 0; j < batch[i].Vertices.Count; j++)
                {
                    ObjVertex currentVertex = batch[i].Vertices[j];
                    if (currentVertex != null)
                    {
                        xmlWriter.WriteStartElement("vertex");
                        xmlWriter.WriteAttributeString("id", currentVertex.Id.ToString());
                        xmlWriter.WriteAttributeString("x", currentVertex.X);
                        xmlWriter.WriteAttributeString("y", currentVertex.Y);
                        xmlWriter.WriteAttributeString("z", currentVertex.Z);
                        xmlWriter.WriteAttributeString("diffuse", batch[i].Diffuse);
                        xmlWriter.WriteEndElement();
                    }
                }

                for (int j = 0; j < batch[i].Faces.Count; j++)
                {
                    ObjFace currentFace = batch[i].Faces[j];
                    xmlWriter.WriteStartElement("face");
                    xmlWriter.WriteAttributeString("v1", currentFace.V1);
                    xmlWriter.WriteAttributeString("v2", currentFace.V2);
                    xmlWriter.WriteAttributeString("v3", currentFace.V3);
                    xmlWriter.WriteEndElement();
                }

                xmlWriter.WriteEndElement();
            }

            xmlWriter.WriteEndElement();
            xmlWriter.WriteComment("<model-instance id=\"model\" refx=\"inrun\" refy=\"inrun-top\" x=\"0\" y=\"0\" z=\"0\" />");

            xmlWriter.WriteEndDocument();
            xmlWriter.Close();
        }

        public ObjFaceType GetFaceType(string face)
        {
            ObjFaceType faceType = new();

            if (!face.Contains("/"))
            {
                faceType = ObjFaceType.INDICE;
            }
            else
            {
                if (face.Contains("//"))
                {
                    faceType = ObjFaceType.NORMAL_INDICE_WITHOUT_TEXTURE_COORDINATE_INDICES;
                }
                else
                {
                    if (face.Contains("/"))
                    {
                        if (face.Split("/").Length == 2)
                        {
                            faceType = ObjFaceType.TEXTURE_COORDINATE_INDICE;
                        }
                        else
                        {
                            faceType = ObjFaceType.NORMAL_INDICE;
                        }
                    }
                }
            }

            return faceType;
        }
    }
}