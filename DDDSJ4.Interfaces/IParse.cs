using System.Collections.Generic;
using DDDSJ4.Models;

namespace DDDSJ4.Interfaces
{
    public interface IParse
    {
        List<ObjBatch> Parse(List<string> input, List<MtlMaterial> materials);
        string Generate(List<ObjBatch> batch);
        ObjFaceType GetFaceType(string face);
    }
}