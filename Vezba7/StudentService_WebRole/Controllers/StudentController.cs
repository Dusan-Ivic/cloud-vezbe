using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Queue;
using StudentService_Data;
using System;
using System.Collections.Generic;
using System.Drawing;
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
                if (repo.Exists(RowKey))
                {
                    ViewBag.Message = "Student sa unetim indeksom vec postoji!";
                    return View();
                }

                BlobHelper blobHelper = new BlobHelper();
                CloudBlockBlob blob = blobHelper.GetBlockBlobReference("vezba", $"image_{RowKey}");
                blob.Properties.ContentType = file.ContentType;
                blobHelper.UploadImage(new Bitmap(file.InputStream), blob);

                Student entry = new Student(RowKey)
                {
                    Name = Name,
                    LastName = LastName,
                    PhotoUrl = blob.Uri.ToString(),
                    ThumbnailUrl = blob.Uri.ToString()
                };
                repo.AddStudent(entry);

                CloudQueue queue = QueueHelper.GetQueueReference("vezba");
                queue.AddMessage(new CloudQueueMessage(RowKey));

                return RedirectToAction("Index");
            }
            catch
            {
                return View("AddEntity");
            }
        }
    }
}