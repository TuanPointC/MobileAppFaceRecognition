using Microsoft.ML.Data;
using Microsoft.ML.Transforms.Image;
using System.Drawing;

namespace FaceRecognitionApi.Models
{
    public class ImageSetting
    {
        public const int imageHeight = 224;
        public const int imageWidth = 224;
    }
    public class ImageInputData
    {
        [ImageType(ImageSetting.imageHeight, ImageSetting.imageWidth)]
        [ColumnName("sequential_1_input")]
        public Bitmap Image { get; set; }
    }
}
