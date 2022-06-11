using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Queue;
using SwitchService_Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SwitchService_WebRole.Controllers
{
    public class SwitchController : Controller
    {
        private SwitchDataRepository repo = new SwitchDataRepository();

        // GET: Switch
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Add(string Name, string State)
        {
            try
            {
                if (!State.Equals("otvoren") && !State.Equals("zatvoren"))
                {
                    return View("Error");
                }

                CloudQueue queue = QueueHelper.GetQueueReference("vezba");
                queue.AddMessage(new CloudQueueMessage($"{Name}:{State}"));

                return View("Index");
            }
            catch
            {
                return View("Index");
            }
        }

        [HttpPost]
        public ActionResult History(string Name)
        {
            var switches = repo.RetrieveSwitches(Name);

            BlobHelper blobHelper = new BlobHelper();
            CloudBlockBlob blob = blobHelper.GetBlockBlobReference("vezba", "snapshot");

            string text = "";
            foreach (Switch sw in switches)
            {
                text += $"{sw.Name}:{sw.State}$$";
            }
            blobHelper.UploadText(text, blob);

            return View("History", switches);
        }

        public ActionResult Error()
        {
            return View("Error");
        }
    }
}