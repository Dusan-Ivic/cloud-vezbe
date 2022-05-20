using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using StudentService_Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace StudentService_WebRole.Controllers
{
    public class StudentController : Controller
    {
        StudentDataRepository repo = new StudentDataRepository();

        // GET: Student
        public ActionResult Index()
        {
            return View(repo.RetrieveAllStudents());
        }

        public ActionResult Create()
        {
            return View("AddEntity");
        }

        [HttpPost]
        public ActionResult AddEntity(string RowKey, string Name, string LastName, HttpPostedFileBase file)
        {
            try
            {
                // kreiranje blob sadrzaja i kreiranje blob klijenta
                string uniqueBlobName = string.Format("image_{0}", RowKey);
                var storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("DataConnectionString"));
                CloudBlobClient blobStorage = storageAccount.CreateCloudBlobClient();
                CloudBlobContainer container = blobStorage.GetContainerReference("vezba");
                CloudBlockBlob blob = container.GetBlockBlobReference(uniqueBlobName);
                blob.Properties.ContentType = file.ContentType;
                // postavljanje odabrane datoteke (slike) u blob servis koristeci blob klijent
                blob.UploadFromStream(file.InputStream);
                // upis studenta u table storage koristeci StudentDataRepository klasu
                Student entry = new Student(RowKey)
                {
                    Name = Name,
                    LastName = LastName,
                    PhotoUrl = blob.Uri.ToString(),
                    ThumbnailUrl = blob.Uri.ToString()
                };
                repo.AddStudent(entry);
                return RedirectToAction("Index");
            }
            catch
            {
                return View("AddEntity");
            }
        }
    }
}