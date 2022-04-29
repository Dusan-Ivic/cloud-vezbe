using BankData;
using Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bank
{
    public class BankServerProvider : IBank, ITransaction
    {
        public BankDataRepository bankDataRepository = new BankDataRepository();

        public void ListClients()
        {
            List<User> users = bankDataRepository.ListClients().ToList();

            foreach (User user in users)
            {
                Trace.TraceInformation(user.ToString());
            }
        }

        public void EnlistMoneyTransfer(string userID, double amount)
        {
            bankDataRepository.EnlistMoneyTransfer(userID, amount);
        }

        public bool Prepare()
        {
            return bankDataRepository.Prepare();
        }

        public void Commit()
        {
            bankDataRepository.Commit();
        }

        public void Rollback()
        {
            bankDataRepository.Rollback();
        }
    }
}
