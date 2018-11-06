using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using ClassLibrary1;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Blob;

namespace ThumbnailFunction
{
    public static class Function1
    {
        [FunctionName("Function1")]
        public static void GenerateThumbnail(
            [QueueTrigger("thumbnailrequest-poison")] BlobInformation blobInfo,
            [Blob("images/{BlobName}", FileAccess.Read)] Stream input,
            [Blob("images/{BlobNameWithoutExtension}_thumbnail.jpg")] CloudBlockBlob outputBlob, ILogger log)
        {
            using (Stream output = outputBlob.OpenWrite())
            {
                ConvertImageToThumbnailJPG(input, output);
                outputBlob.Properties.ContentType = "image/jpeg";
            }

            using (MyDemoContext db = new MyDemoContext())
            {
                var id = blobInfo.EmpId;
                Employee emp = db.Employees.Find(id);
                if (emp == null)
                {
                    throw new Exception(String.Format("EmpId: {0} not found, can't create thumbnail", id.ToString()));
                }
                emp.ThumbnailURL = outputBlob.Uri.ToString();
                db.SaveChanges();
            }
        }

        public static void ConvertImageToThumbnailJPG(Stream input, Stream output)
        {
            int thumbnailsize = 80;
            int width;
            int height;
            var originalImage = new Bitmap(input);

            if (originalImage.Width > originalImage.Height)
            {
                width = thumbnailsize;
                height = thumbnailsize * originalImage.Height / originalImage.Width;
            }
            else
            {
                height = thumbnailsize;
                width = thumbnailsize * originalImage.Width / originalImage.Height;
            }

            Bitmap thumbnailImage = null;
            try
            {
                thumbnailImage = new Bitmap(width, height);

                using (Graphics graphics = Graphics.FromImage(thumbnailImage))
                {
                    graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    graphics.DrawImage(originalImage, 0, 0, width, height);
                }
                thumbnailImage.Save(output, System.Drawing.Imaging.ImageFormat.Jpeg);
            }
            finally
            {
                if (thumbnailImage != null)
                {
                    thumbnailImage.Dispose();
                }
            }
        }
    }
} 
