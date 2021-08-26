using Amazon.Rekognition;
using Amazon.Rekognition.Model;
using Amazon.S3;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;


namespace DemoRekogWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        private readonly IAmazonS3 amazons3;
        private readonly IAmazonRekognition rekognition;

        public ImageController(IAmazonS3 amazons3, IAmazonRekognition rekognition)
        {
            this.amazons3 = amazons3;
            this.rekognition = rekognition;
        }

        [HttpPost]
        public async Task<IActionResult> UploadImageToAWS([FromForm] IFormFile file1, IFormFile file2) {

            if (file1 != null && file2 != null )
            {
                Image ImageSource = new Image();
                using (var ms = new MemoryStream())
                {
                    file1.CopyTo(ms);
                    var fileBytes = ms.ToArray();
                    ImageSource.Bytes = new MemoryStream(ms.ToArray());
                    // act on the Base64 data
                }

                Image ImageTarget = new Image();
                using (var ms = new MemoryStream())
                {
                    file2.CopyTo(ms);
                    var fileBytes = ms.ToArray();
                    ImageTarget.Bytes = new MemoryStream(ms.ToArray());
                    // act on the Base64 data
                }

                var response = await rekognition.CompareFacesAsync(new CompareFacesRequest()
                {
                    SourceImage = ImageSource,
                    TargetImage = ImageTarget,
                    // Umbral de Similaridad
                    SimilarityThreshold = 90
                });


                //Respuesta
                return Ok(response);
            }

            //Bad Respuesta
            return BadRequest("images is required");  
        }
    }
}
