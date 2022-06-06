using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Microsoft.WindowsAzure.Storage.Queue;
using StudentService_Data;

namespace ImageConverter_WorkerRole
{
    public class WorkerRole : RoleEntryPoint
    {
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly ManualResetEvent runCompleteEvent = new ManualResetEvent(false);

        public override void Run()
        {
            Trace.TraceInformation("ImageConverter_WorkerRole is running");

            CloudQueue queue = QueueHelper.GetQueueReference("vezba");
            StudentDataRepository studentDataRepository = new StudentDataRepository();

            while (true)
            {
                CloudQueueMessage message = queue.GetMessage();
                if (message == null)
                {
                    Trace.TraceInformation("Trenutno nema poruka u redu!");
                }
                else
                {
                    Trace.TraceInformation($"Poruka: {message.AsString}");

                    Student student = studentDataRepository.GetStudent(message.AsString);
                    if (student == null)
                    {
                        Trace.TraceInformation("Trazeni student ne postoji!");
                        continue;
                    }

                    BlobHelper blobHelper = new BlobHelper();
                    CloudBlockBlob imageBlob = blobHelper.GetBlockBlobReference("vezba", $"image_{student.RowKey}");
                    Image image = blobHelper.DownloadImage(imageBlob);
                    image = ImageConverterClass.ConvertImage(image);

                    CloudBlockBlob thumbnailBlob = blobHelper.GetBlockBlobReference("vezba", $"thumbnail_{student.RowKey}");
                    thumbnailBlob.Properties.ContentType = "image/bmp";
                    string thumbnailUrl = blobHelper.UploadImage(image, thumbnailBlob);
                    student.ThumbnailUrl = thumbnailUrl;
                    studentDataRepository.UpdateStudent(student);

                    queue.DeleteMessage(message);
                    Trace.TraceInformation("Poruka je uspesno obradjena!");
                }

                Thread.Sleep(5000);
            }
        }

        public override bool OnStart()
        {
            // Set the maximum number of concurrent connections
            ServicePointManager.DefaultConnectionLimit = 12;

            // For information on handling configuration changes
            // see the MSDN topic at https://go.microsoft.com/fwlink/?LinkId=166357.

            bool result = base.OnStart();

            Trace.TraceInformation("ImageConverter_WorkerRole has been started");

            return result;
        }

        public override void OnStop()
        {
            Trace.TraceInformation("ImageConverter_WorkerRole is stopping");

            this.cancellationTokenSource.Cancel();
            this.runCompleteEvent.WaitOne();

            base.OnStop();

            Trace.TraceInformation("ImageConverter_WorkerRole has stopped");
        }

        private async Task RunAsync(CancellationToken cancellationToken)
        {
            // TODO: Replace the following with your own logic.
            while (!cancellationToken.IsCancellationRequested)
            {
                Trace.TraceInformation("Working");
                await Task.Delay(1000);
            }
        }
    }
}
