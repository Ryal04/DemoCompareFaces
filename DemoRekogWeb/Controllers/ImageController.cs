using Amazon.Rekognition;
using Amazon.Rekognition.Model;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
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

            //bucketName (Nombre de Repositorio de Almacenamiento) 
            var bucketName = "rdy-bucket-1209";

            if (file1 != null && file2 != null )
            {

                // Upload Imagen uno
                var putRequest1 = new PutObjectRequest()
                {
                    BucketName = bucketName,
                    Key = file1.FileName,
                    InputStream = file1.OpenReadStream(),
                    ContentType = file1.ContentType,
                };

                await this.amazons3.PutObjectAsync(putRequest1);

                // Upload Imagen dos
                var putRequest2 = new PutObjectRequest()
                {
                    BucketName = bucketName,
                    Key = file2.FileName,
                    InputStream = file2.OpenReadStream(),
                    ContentType = file2.ContentType,
                };

                await this.amazons3.PutObjectAsync(putRequest2);


                // Comparacion de Rostros de Imagenes 
                var response = await rekognition.CompareFacesAsync(new CompareFacesRequest()
                {
                    SourceImage = new Image()
                    {
                        
                        
                        
                        S3Object = new Amazon.Rekognition.Model.S3Object()
                        {
                            Bucket = bucketName,
                            Name = file1.FileName
                        }
                    },
                    TargetImage = new Image()
                    {

                        S3Object = new Amazon.Rekognition.Model.S3Object()
                        {
                            Bucket = bucketName,
                            Name = file2.FileName
                        }
                    },
                    // Umbral de Similaridad
                    SimilarityThreshold = 90
                });

                // Elimina las imagenes subidas en la solicitud 
                await this.amazons3.DeleteObjectAsync(bucketName, file1.FileName);
                await this.amazons3.DeleteObjectAsync(bucketName, file2.FileName);

                

                foreach (var label in response.FaceMatches)
                {
                    return Ok(label.Similarity);
                }

                //Respuesta
                return Ok(response);
            }

            //Bad Respuesta
            return BadRequest("images is required");  
        }
    }
}
