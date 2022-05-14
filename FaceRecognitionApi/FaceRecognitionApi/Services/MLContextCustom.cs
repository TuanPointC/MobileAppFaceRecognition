using Emgu.CV;
using Emgu.CV.Structure;
using FaceRecognitionApi.Models;
using Microsoft.ML;
using System.Drawing;

namespace FaceRecognitionApi.Services
{
    public class MLContextCustom
    {
        public string modelPath = Environment.CurrentDirectory + "\\Sources\\my_model.onnx";
        readonly MLContext mlContext = new();

        public List<double> Softmax(float[] data)
        {
            var sumData = Math.Exp(data[0]) + Math.Exp(data[1]);
            var result = new List<double>();
            var e1 = Math.Exp(data[0]) / sumData;
            result.Add(e1);
            var e2 = Math.Exp(data[1]) / sumData;
            result.Add(e2);
            return result;
        }
        public Bitmap ResizeBitmap(Bitmap bmp, int width, int height)
        {
            Bitmap result = new Bitmap(width, height);
            using (Graphics g = Graphics.FromImage(result))
            {
                g.DrawImage(bmp, 0, 0, width, height);
            }

            return result;
        }
        public bool Predict(Image<Bgr, byte> image)
        {
            var bitMatImage = image.ToBitmap();
            var newImage = ResizeBitmap(bitMatImage, 224, 224);

            var emptyData = new List<ImageInputData>();
            var data = mlContext.Data.LoadFromEnumerable(emptyData);
            var pipeline = mlContext.Transforms.ExtractPixels(outputColumnName: "sequential_1_input", interleavePixelColors: true)
              .Append(mlContext.Transforms.ApplyOnnxModel(modelFile: modelPath, outputColumnName: "dense_3", inputColumnName: "sequential_1_input"));

            var model = pipeline.Fit(data);
            var predictionEngine = mlContext.Model.CreatePredictionEngine<ImageInputData, ModelOutData>(model);
            var h_hat = predictionEngine.Predict(new ImageInputData { Image = newImage });

            var result = Softmax(h_hat.Data);

            if (result[0] > result[1])
            {
                return true;
            }
            else
            {
                return false;
            }

        }
    }
}
