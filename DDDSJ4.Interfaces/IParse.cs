using System.Collections.Generic;
using DDDSJ4.Models;

namespace DDDSJ4.Interfaces
{
    public interface IParse
    {
        List<ObjBatch> Parse(string[] input, List<MtlMaterial> materials);
        void Generate(List<ObjBatch> batch, string fileName);
        ObjFaceType GetFaceType(string face);
    }
}