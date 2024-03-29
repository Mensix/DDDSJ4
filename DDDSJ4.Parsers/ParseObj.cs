using System.Xml;
using System.Linq;
using System;
using System.Collections.Generic;
using DDDSJ4.Models;
using DDDSJ4.Interfaces;
using DDDSJ4.Utilities;

namespace DDDSJ4.Parsers
{
    public class ParseObj : IParse
    {
        public List<ObjBatch> Parse(string[] input, List<MtlMaterial> materials)
        {
            List<ObjBatch> batches = new();
            List<ObjVertex> vertices = new();
            List<ObjFace> faces = new();
            int index = 1;

            batches.Add(new ObjBatch
            {
                Id = "0",
                Diffuse = "0xFFFFFF",
                Faces = new List<ObjFace>(),
                Vertices = new List<ObjVertex>()
            });

            for (int i = 0; i < input.Length; i++)
            {
                string[] line = input[i].Trim().Split(" ");

                if (line[0].StartsWith("v") && line[0].EndsWith("v"))
                {
                    if (line.Length != 4)
                    {
                        Logger.LogError($"Invalid vertex definition found, line {i}\n");
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
                    bool wasDiffuseFound = materials.Where(x => x.Name == line[1]).ToArray().Length > 0;
                    if (!wasDiffuseFound) Logger.LogWarn($"Material {line[1]} diffuse wasn't found, using 0xFFFFFF color");

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
                    if (line.Length != 4)
                    {
                        Logger.LogWarn($"Face definition must contain exactly three vertices identifications, triangulate your model ie. in Blender, line {i + 1}");
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
                ConformanceLevel = ConformanceLevel.Fragment,
                Indent = true,
                OmitXmlDeclaration = true,
            });
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
            xmlWriter.WriteStartElement("model-instance");
            xmlWriter.WriteAttributeString("id", "model");
            xmlWriter.WriteAttributeString("refx", "inrun");
            xmlWriter.WriteAttributeString("refy", "inrun-top");
            xmlWriter.WriteAttributeString("x", "0");
            xmlWriter.WriteAttributeString("y", "0");
            xmlWriter.WriteAttributeString("z", "0");
            xmlWriter.WriteEndElement();

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