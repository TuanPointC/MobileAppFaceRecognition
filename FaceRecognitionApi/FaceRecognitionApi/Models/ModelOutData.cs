using Microsoft.ML.Data;

namespace FaceRecognitionApi.Models
{
    public class ModelOutData
    {
        [VectorType(2)]
        [ColumnName("dense_3")]
        public float[] Data { get; set; }
    }
}
