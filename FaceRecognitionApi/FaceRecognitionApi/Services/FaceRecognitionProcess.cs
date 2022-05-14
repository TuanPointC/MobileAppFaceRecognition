using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Face;
using Emgu.CV.Structure;
using FaceRecognitionApi.Models;
using FaceRecognitionApi.Services;
using Microsoft.ML;
using Microsoft.ML.Vision;
using System.Drawing;
using System.Drawing.Imaging;
using static Microsoft.ML.DataOperationsCatalog;

namespace FaceRecognitionApi.Sources
{
    public class FaceRecognitionProcess : IFaceRecognitionProcess
    {
        private readonly CascadeClassifier HaarCascade = new(Environment.CurrentDirectory + "\\Sources\\haarcascade_frontalface_default.xml");
        public MLContextCustom mlContext = new();
        public Image<Bgr, byte> ProcessFrame(Image image)
        {
            try
            {
                Bitmap bitmapImage = (Bitmap)image;
                Rectangle rectangle = new(0, 0, bitmapImage.Width, bitmapImage.Height);
                BitmapData bmpData = bitmapImage.LockBits(rectangle, ImageLockMode.ReadWrite, bitmapImage.PixelFormat);

                Image<Bgr, byte> outputImage = new(bitmapImage.Width, bitmapImage.Height, bmpData.Stride, bmpData.Scan0);
                var outputImageCopy = outputImage.Copy();
                var faces = HaarCascade.DetectMultiScale(outputImage, 1.1, 10, Size.Empty);

                foreach (var face in faces)
                {
                    outputImageCopy.ROI = face;
                    if (mlContext.Predict(outputImageCopy))
                    {
                        outputImage.Draw(face, new Bgr(Color.Green), 2);
                        CvInvoke.PutText(outputImage, "With Mask", new Point(face.X, face.Y - 5), FontFace.HersheySimplex, 0.5, new Bgr(Color.Green).MCvScalar, 1);
                    }
                    else
                    {
                        outputImage.Draw(face, new Bgr(Color.Red), 2);
                        CvInvoke.PutText(outputImage, "Without Mask", new Point(face.X, face.Y - 5), FontFace.HersheySimplex, 0.5, new Bgr(Color.Red).MCvScalar, 1);
                    }
                    return outputImage;
                }
                throw new Exception("Cannot detect face");
            }
            catch
            {
                throw;
            }
        }
    }
}
