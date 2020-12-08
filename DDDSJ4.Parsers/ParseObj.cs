using System.Text;
using System.Linq;
using System;
using System.Collections.Generic;
using DDDSJ4.Models;
using DDDSJ4.Interfaces;

namespace DDDSJ4.Parsers
{
    public class ParseObj
    {
        public List<ObjBatch> Parse(List<string> input, List<MtlMaterial> materials)
        {
            List<ObjBatch> batches = new();
            List<ObjVertex> vertices = new();
            List<ObjFace> faces = new();
            int index = 1;

            for (int i = 0; i < input.Count; i++)
            {
                List<string> line = input[i].Split(" ").ToList();

                if (line[0].StartsWith("v"))
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

                if (line[0].StartsWith("usemtl"))
                {
                    batches.Add(new ObjBatch
                    {
                        Id = line[1],
                        Diffuse = materials.First(x => x.Name == line[1]).Diffuse,
                        Vertices = new List<ObjVertex>(),
                        Faces = new List<ObjFace>()
                    });
                }

                if (line[0].StartsWith("f"))
                {
                    batches[^1].Faces.Add(new ObjFace
                    {
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
                    batches[i].Vertices.Add(vertices.Find(x => x.Id.ToString() == batches[i].Faces[j].V1));
                    batches[i].Vertices.Add(vertices.Find(x => x.Id.ToString() == batches[i].Faces[j].V2));
                    batches[i].Vertices.Add(vertices.Find(x => x.Id.ToString() == batches[i].Faces[j].V3));
                }
            }

            for (int i = 0; i < batches.Count; i++)
            {
                batches[i].Vertices = batches[i].Vertices.Distinct().ToList();
            }

            return batches;
        }

        public string Generate(List<ObjBatch> batch)
        {
            StringBuilder stringBuilder = new(@"<3dmodel id=""model"">");

            for (int i = 0; i < batch.Count; i++)
            {
                stringBuilder.Append(@"<batch id=""").Append(batch[i].Id).Append(@""" texture1=""Textures\concrete5.png"" material=""Materials\material1.xml"" fvf=""322"" order=""0"">").Append("\n");

                for (int j = 0; j < batch[i].Vertices.Count; j++)
                {
                    ObjVertex currentVertex = batch[i].Vertices[j];
                    stringBuilder.Append(@"<vertex id=""").Append(currentVertex.Id).Append(@""" x=""").Append(currentVertex.X).Append(@""" y=""").Append(currentVertex.Y).Append(@""" z=""").Append(currentVertex.Z).Append(@""" diffuse=""").Append(batch[i].Diffuse).Append(@""" />").Append("\n");
                }

                for (int j = 0; j < batch[i].Faces.Count; j++)
                {
                    ObjFace currentFace = batch[i].Faces[j];
                    stringBuilder.Append(@"<face v1=""").Append(currentFace.V1).Append(@""" v2=""").Append(currentFace.V2).Append(@""" v3=""").Append(currentFace.V3).Append(@""" />").Append("\n");
                }

                stringBuilder.Append(@"</batch>").Append("\n");
            }

            stringBuilder.Append(@"</3dmodel>").Append("\n").Append(@"<3dmodel-instance id=""model"" refx=""inrun"" refy=""inrun-top"" x=""0"" y=""0"" z=""0""/>");

            return stringBuilder.ToString();
        }
    }
}