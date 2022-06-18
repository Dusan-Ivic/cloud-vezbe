using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Drawing;
//using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OperationService_Data
{
    public class BlobHelper
    {
        private CloudStorageAccount _storageAccount;
        private CloudBlobClient _blobStorage;

        public BlobHelper()
        {
            _storageAccount
                = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("DataConnectionString"));

            _blobStorage = _storageAccount.CreateCloudBlobClient();
        }

        public CloudBlockBlob GetBlockBlobReference(string containerName, string blobName)
        {
            CloudBlobContainer container
                = _blobStorage.GetContainerReference(containerName);

            CloudBlockBlob blob
                = container.GetBlockBlobReference(blobName);

            return blob;
        }

        public string DownloadText(CloudBlockBlob blob)
        {
            return blob.DownloadText();
        }

        public void UploadText(string text, CloudBlockBlob blob)
        {
            blob.Properties.ContentType = "text/plain";
            blob.UploadText(text);
        }

        //public Image DownloadImage(CloudBlockBlob blob)
        //{
        //    using (MemoryStream memoryStream = new MemoryStream())
        //    {
        //        blob.DownloadToStream(memoryStream);
        //        return new Bitmap(memoryStream);
        //    }
        //}

        //public string UploadImage(Image image, CloudBlockBlob blob)
        //{
        //    using (MemoryStream memoryStream = new MemoryStream())
        //    {
        //        image.Save(memoryStream, ImageFormat.Bmp);
        //        memoryStream.Position = 0;
        //        blob.Properties.ContentDisposition = "image/bmp";
        //        blob.UploadFromStream(memoryStream);
        //        return blob.Uri.ToString();
        //    }
        //}
    }
}
