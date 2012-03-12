using System;
using System.Configuration;
using System.IO;
using System.Web.Mvc;
using Amazon.S3;
using Amazon.S3.Model;

namespace Integracao_Amazon_S3_MVC3.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/

        public ActionResult Index()
        {
            return View("Index");
        }


        public ActionResult CarregarArquivo()
        {
            var fileUpload = Request.Files[0];
            if (fileUpload == null)
                return Content("Error", "text/plain");

            FileInfo oFileInfo = new FileInfo(fileUpload.FileName);
            SaveObjectInAws(fileUpload.InputStream, oFileInfo.Name);

            ViewData["mensagem"] = string.Format("Arquivo {0} enviado com sucesso.", oFileInfo.Name);

            return View("Index");
        }

        static void SaveObjectInAws(Stream pObject, string keyname)
        {
            try
            {
                using (var client = Amazon.AWSClientFactory.CreateAmazonS3Client())
                {
                    // simple object put
                    PutObjectRequest request = new PutObjectRequest();
                    request.WithBucketName(ConfigurationManager.AppSettings["bucketname"]).WithKey(keyname).WithInputStream(pObject);

                    using (client.PutObject(request)) { }
                }
            }
            catch (AmazonS3Exception amazonS3Exception)
            {
                if (amazonS3Exception.ErrorCode != null &&
                    (amazonS3Exception.ErrorCode.Equals("InvalidAccessKeyId") ||
                    amazonS3Exception.ErrorCode.Equals("InvalidSecurity")))
                {
                    Console.WriteLine("Please check the provided AWS Credentials.");
                    Console.WriteLine("If you haven't signed up for Amazon S3, please visit http://aws.amazon.com/s3");
                    throw;
                }

                Console.WriteLine("An error occurred with the message '{0}' when writing an object", amazonS3Exception.Message);
                throw;
            }
        }
    }
}
