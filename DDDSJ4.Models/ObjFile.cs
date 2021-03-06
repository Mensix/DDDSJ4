using System.Collections.Generic;

namespace DDDSJ4.Models
{
    public class ObjBatch
    {
        public string Id { get; set; }
        public string Diffuse { get; set; }
        public List<ObjVertex> Vertices { get; set; }
        public List<ObjFace> Faces { get; set; }
    }

    public class ObjVertex
    {
        public int Id { get; set; }
        public string X { get; set; }
        public string Y { get; set; }
        public string Z { get; set; }
    }

    public enum ObjFaceType
    {
        INDICE,
        TEXTURE_COORDINATE_INDICE,
        NORMAL_INDICE,
        NORMAL_INDICE_WITHOUT_TEXTURE_COORDINATE_INDICES
    }

    public class ObjFace
    {
        public string V1 { get; set; }
        public string V2 { get; set; }
        public string V3 { get; set; }
        public ObjFaceType Type { get; set;}
}
}