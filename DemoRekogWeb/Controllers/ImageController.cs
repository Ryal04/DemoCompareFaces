using Amazon.Rekognition;
using Amazon.Rekognition.Model;
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

        private readonly IAmazonRekognition rekognition;

        public ImageController(IAmazonRekognition rekognition)
        {
            this.rekognition = rekognition;
        }

        [HttpPost]
        public async Task<IActionResult> UploadImageToAWS([FromForm] IFormFile CedFrontal, IFormFile Foto) {

            if (Foto != null && CedFrontal != null )
            {

                //Convercion Imagen
                Image ImageSource = new Image();
                using (var ms = new MemoryStream())
                {
                    Foto.CopyTo(ms);
                    ImageSource.Bytes = new MemoryStream(ms.ToArray());
                    // act on the Base64 data
                }

                //Convercion Imagen
                Image ImageTarget = new Image();
                using (var ms = new MemoryStream())
                {
                    CedFrontal.CopyTo(ms);
                    ImageTarget.Bytes = new MemoryStream(ms.ToArray());
                    // act on the Base64 data
                }

                var response = await rekognition.CompareFacesAsync(new CompareFacesRequest()
                {
                    //Imagen Byte 1
                    SourceImage = ImageSource,
                    //Imagen Byte 2
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
