using System.Collections.Generic;
using DDDSJ4.Models;

namespace DDDSJ4.Interfaces
{
    public interface IParse
    {
        (List<ObjVertex>, List<ObjFace>) Parse(List<string> input);
        string Generate(List<ObjVertex> vertices, List<ObjFace> faces);
    }
}