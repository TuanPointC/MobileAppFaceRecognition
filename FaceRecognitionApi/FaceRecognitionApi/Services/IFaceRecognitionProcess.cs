using Emgu.CV;
using Emgu.CV.Structure;
using FaceRecognitionApi.Models;
using System.Drawing;

namespace FaceRecognitionApi.Sources
{
    public interface IFaceRecognitionProcess
    {
        public Image<Bgr, byte> ProcessFrame(Image image);
    }
}
