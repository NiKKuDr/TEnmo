using System;
using System.Collections.Generic;
using TenmoClient.Models;

namespace TenmoClient.Services
{
    public class TenmoConsoleService : ConsoleService
    {
        /************************************************************
            Print methods
        ************************************************************/
        public void PrintLoginMenu()
        {
            Console.Clear();
            Console.WriteLine("");
            Console.WriteLine("Welcome to TEnmo!");
            Console.WriteLine("1: Login");
            Console.WriteLine("2: Register");
            Console.WriteLine("0: Exit");
            Console.WriteLine("---------");
        }

        public void PrintMainMenu(string username)
        {
            Console.Clear();
            Console.WriteLine("");
            Console.WriteLine($"Hello, {username}!");
            Console.WriteLine("1: View your current balance");
            Console.WriteLine("2: View your past transfers");
            Console.WriteLine("3: View your pending requests");
            Console.WriteLine("4: Send TE bucks");
            Console.WriteLine("5: Request TE bucks");
            Console.WriteLine("6: Log out");
            Console.WriteLine("0: Exit");
            Console.WriteLine("---------");
        }
        public LoginUser PromptForLogin()
        {
            string username = PromptForString("User name");
            if (String.IsNullOrWhiteSpace(username))
            {
                return null;
            }
            string password = PromptForHiddenString("Password");

            LoginUser loginUser = new LoginUser
            {
                Username = username,
                Password = password
            };
            return loginUser;
        }

        // Add application-specific UI methods here...
        public void PrintBalance(decimal balance)
        {
            Console.WriteLine(balance);
            Pause();
        }

        public void PrintUsernames(List<Account> accounts)
        {
            for (int i = 0; i < accounts.Count; i++)
            {
                Console.Write(i + 1 + ") ");
                Console.WriteLine(accounts[i].UserName);
            }
        }
        public void PrintTransferHistory(List<StringifiedTransfer> transfers, string username)
        {
            Console.WriteLine("----------------Transfers----------------");
            Console.WriteLine("ID   From/To                    Amount   ");
            Console.WriteLine("-----------------------------------------");
            foreach (StringifiedTransfer transfer in transfers)
            {
                Console.Write(transfer.TransferId + "   ");
                if (username == transfer.UserFrom)
                {
                    Console.Write("To:   " + transfer.UserTo);
                }
                else if(username == transfer.UserTo)
                {
                    Console.Write("From: " + transfer.UserFrom);
                }
                Console.WriteLine($"\t\t{transfer.Amount:c2}");
            }
        }
        public void PrintTransferDetails(StringifiedTransfer transfer)
        {
            Console.WriteLine("----------------------------------------");
            Console.WriteLine("            Transfer Details            ");
            Console.WriteLine("----------------------------------------");
            Console.WriteLine($"Id: {transfer.TransferId}");
            Console.WriteLine($"From: {transfer.UserFrom}");
            Console.WriteLine($"To: {transfer.UserTo}");
            Console.WriteLine($"Type: {transfer.TransferType}");
            Console.WriteLine($"Status: {transfer.TransferStatus}");
            Console.WriteLine($"Amount: {transfer.Amount:c2}");
        }
        public void PrintTransferSendOutcome(StringifiedTransfer transfer)
        {
            if (transfer != null)
            {
                Console.WriteLine($"{transfer.Amount:c2} successfully transfered to {transfer.UserTo}");
            }
            else
            {
                Console.WriteLine("transfer denied");
            }
            Pause();
        }
        public void PrintTransferRequestOutcome(StringifiedTransfer transfer)
        {
            if (transfer != null)
            {
                Console.WriteLine($"Transfer request of {transfer.Amount:c2} from {transfer.UserFrom} submitted and pending their approval.");
            }
            else
            {
                Console.WriteLine("transfer request denied");
            }
            Pause();
        }
        public void PrintPendingTransfers(List<StringifiedTransfer> transfers)
        {
            if(transfers != null && transfers.Count != 0)
            {
                Console.WriteLine("------------Pending Transfers------------");
                Console.WriteLine("ID          To                  Amount   ");
                Console.WriteLine("-----------------------------------------");
                foreach(StringifiedTransfer transfer in transfers)
                {
                    Console.Write(transfer.TransferId + "        ");
                    Console.Write($"{transfer.UserTo}");
                    Console.WriteLine($"\t\t{transfer.Amount:c2}");
                }
            }
            else
            {
                Console.WriteLine("No pending transfers");
            }
        }
    }
}
