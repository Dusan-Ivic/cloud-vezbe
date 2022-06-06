using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentService_Data
{
    public class BlobHelper
    {
        private CloudStorageAccount storageAccount;
        private CloudBlobClient blobStorage;

        public BlobHelper()
        {
            storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("DataConnectionString"));
            blobStorage = storageAccount.CreateCloudBlobClient();
        }

        public CloudBlockBlob GetBlockBlobReference(string containerName, string blobName)
        {
            CloudBlobContainer container = blobStorage.GetContainerReference(containerName);
            CloudBlockBlob blob = container.GetBlockBlobReference(blobName);

            return blob;
        }

        public Image DownloadImage(CloudBlockBlob blob)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                blob.DownloadToStream(memoryStream);
                return new Bitmap(memoryStream);
            }
        }

        public string UploadImage(Image image, CloudBlockBlob blob)
        {
            using (MemoryStream memoryStream = new MemoryStream())
            {
                image.Save(memoryStream, ImageFormat.Bmp);
                memoryStream.Position = 0;
                blob.Properties.ContentDisposition = "image/bmp";
                blob.UploadFromStream(memoryStream);
                return blob.Uri.ToString();
            }
        }
    }
}
