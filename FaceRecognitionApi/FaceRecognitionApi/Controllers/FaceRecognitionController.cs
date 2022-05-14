using Emgu.CV;
using FaceRecognitionApi.Models;
using FaceRecognitionApi.Sources;
using Microsoft.AspNetCore.Mvc;
using System.Drawing;

namespace FaceRecognitionApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class FaceRecognitionController : ControllerBase
    {
        private readonly IFaceRecognitionProcess _faceRecognitionProcess;
        public FaceRecognitionController(IFaceRecognitionProcess faceRecognitionProcess)
        {
            _faceRecognitionProcess = faceRecognitionProcess;
        }




        private readonly string[] AllowedExtensions = new string[] { ".jpg", ".jpeg", ".png" };

        [HttpPost]
        public async Task<IActionResult> UploadImageAsync(IFormFile file)
        {
            try
            {
                var extension = Path.GetExtension(file.FileName.ToLower());
                if (file.Length > 0 && AllowedExtensions.Contains(extension))
                {
                    using (var ms = new MemoryStream())
                    {
                        file.CopyTo(ms);
                        var image = Image.FromStream(ms);
                        var output = _faceRecognitionProcess.ProcessFrame(image);
                        var buffer_img = CvInvoke.Imencode(".jpg", output);
                        var base64 = Convert.ToBase64String(buffer_img);
                        return Ok(base64);
                    }
                }
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            return Ok("Uploading is ok");
        }

        [HttpGet]
        public IActionResult getgo()
        {
            return Ok("Oske");
        }


    }
}