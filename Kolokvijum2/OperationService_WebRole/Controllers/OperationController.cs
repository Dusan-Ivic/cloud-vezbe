using Microsoft.WindowsAzure.Storage.Queue;
using OperationService_Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OperationService_WebRole.Controllers
{
    public class OperationController : Controller
    {
        private OperationDataRepository repo = new OperationDataRepository();
        private CloudQueue queue = QueueHelper.GetQueueReference("vezba");

        // GET: Operation
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Add(string firstOperand, string secondOperand, string arithmeticOperator)
        {
            if (int.TryParse(firstOperand, out int operand1))
            {
                if (int.TryParse(secondOperand, out int operand2))
                {
                    if (arithmeticOperator == "+" || arithmeticOperator == "-")
                    {
                        int result = 0;

                        switch (arithmeticOperator)
                        {
                            case "+":
                                result = operand1 + operand2;
                                break;
                            case "-":
                                result = operand1 - operand2;
                                break;
                        }

                        Trace.TraceInformation($"{operand1} {arithmeticOperator} {operand2} = {result}");
                        
                        Operation entity = new Operation(DateTime.Now.ToString("yyyyMMddHHmmss"))
                        {
                            Operand1 = operand1,
                            Operand2 = operand2,
                            Operator = arithmeticOperator,
                            Result = result
                        };
                        repo.InsertEntity(entity);
                        Trace.TraceInformation("Entitet je dodat u TABELU!");
                        
                        queue.AddMessage(new CloudQueueMessage($"{result},{arithmeticOperator}"));
                        Trace.TraceInformation("Poruka je dodata u QUEUE!");

                        return RedirectToAction("Table");
                    }
                    else
                    {
                        return View("Error", null, "Operacija mora biti sabiranje ili oduzimanje!");
                    }
                }
                else
                {
                    return View("Error", null, "Drugi operand mora biti ceo broj!");
                }
            }
            else
            {
                return View("Error", null, "Prvi operand mora biti ceo broj!");
            }
        }

        public ActionResult Error()
        {
            return View();
        }

        public ActionResult Table()
        {
            return View(repo.RetrieveAllEntities());
        }
    }
}