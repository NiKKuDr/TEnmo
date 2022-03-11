using System;
using System.Collections.Generic;
using TenmoClient.Models;
using TenmoClient.Services;

namespace TenmoClient
{
    public class TenmoApp
    {
        private readonly TenmoConsoleService console = new TenmoConsoleService();
        private readonly TenmoApiService tenmoApiService;

        public TenmoApp(string apiUrl)
        {
            tenmoApiService = new TenmoApiService(apiUrl);
        }

        public void Run()
        {
            bool keepGoing = true;
            while (keepGoing)
            {
                // The menu changes depending on whether the user is logged in or not
                if (tenmoApiService.IsLoggedIn)
                {
                    keepGoing = RunAuthenticated();
                }
                else // User is not yet logged in
                {
                    keepGoing = RunUnauthenticated();
                }
            }
        }

        private bool RunUnauthenticated()
        {
            console.PrintLoginMenu();
            int menuSelection = console.PromptForInteger("Please choose an option", 0, 2, 1);
            while (true)
            {
                if (menuSelection == 0)
                {
                    return false;   // Exit the main menu loop
                }

                if (menuSelection == 1)
                {
                    // Log in
                    Login();
                    return true;    // Keep the main menu loop going
                }

                if (menuSelection == 2)
                {
                    // Register a new user
                    Register();
                    return true;    // Keep the main menu loop going
                }
                console.PrintError("Invalid selection. Please choose an option.");
                console.Pause();
            }
        }

        private bool RunAuthenticated()
        {
            console.PrintMainMenu(tenmoApiService.Username);
            int menuSelection = console.PromptForInteger("Please choose an option", 0, 6);
            if (menuSelection == 0)
            {
                // Exit the loop
                return false;
            }

            if (menuSelection == 1)
            {
                decimal balance = tenmoApiService.GetAccount().Balance;
                Console.WriteLine($"Your current account balance is: ${balance}");
                console.Pause();
                //console.PrintBalance(tenmoApiService.GetAccount().Balance);
            }

            if (menuSelection == 2)
            {
                List<Transfer> transfers = tenmoApiService.GetTransfers();
                console.PrintTransferHistory(transfers, tenmoApiService.Username);
                int transferId = console.PromptForInteger("Please enter transfer ID to view details (0 to cancel): ");
                if(transferId != 0)
                {
                    bool transferInTransfers = false;
                    Transfer transfer = new Transfer();
                    foreach(Transfer transfer1 in transfers)
                    {
                        if (transfer1.TransferId == transferId)
                        {
                            transfer = transfer1;
                            transferInTransfers = true;
                        }
                    }
                    if (transferInTransfers)
                    {
                        console.PrintTransferDetails(transfer);
                    }
                    else
                    {
                        Console.WriteLine("No matching transfer in transfer history");
                    }
                }
                
                console.Pause();
            }

            if (menuSelection == 3)
            {
                // View your pending requests
            }

            if (menuSelection == 4)
            {
                Account recipient = GetAccountFromList("Please select which account you would like to send to");
                decimal sendAmount = console.PromptForDecimal("Please enter the amount you would like to send");
                console.PrintTransferOutcome(tenmoApiService.TransferFunds(recipient.UserId, sendAmount));
            }

            if (menuSelection == 5)
            {
                Account requestee = GetAccountFromList("Please select which account you would like to request from");
            }

            if (menuSelection == 6)
            {
                // Log out
                tenmoApiService.Logout();
                console.PrintSuccess("You are now logged out");
            }

            return true;    // Keep the main menu loop going
        }

        private void Login()
        {
            LoginUser loginUser = console.PromptForLogin();
            if (loginUser == null)
            {
                return;
            }

            try
            {
                ApiUser user = tenmoApiService.Login(loginUser);
                if (user == null)
                {
                    console.PrintError("Login failed.");
                }
                else
                {
                    console.PrintSuccess("You are now logged in");
                }
            }
            catch (Exception)
            {
                console.PrintError("Login failed.");
            }
            console.Pause();
        }

        private void Register()
        {
            LoginUser registerUser = console.PromptForLogin();
            if (registerUser == null)
            {
                return;
            }
            try
            {
                bool isRegistered = tenmoApiService.Register(registerUser);
                if (isRegistered)
                {
                    console.PrintSuccess("Registration was successful. Please log in.");
                }
                else
                {
                    console.PrintError("Registration was unsuccessful.");
                }
            }
            catch (Exception)
            {
                console.PrintError("Registration was unsuccessful.");
            }
            console.Pause();
        }
        public Account GetAccountFromList(string message)
        {
            List<Account> accounts = tenmoApiService.GetAllAccountsExceptUser();
            console.PrintUsernames(accounts);
            int selection = console.PromptForInteger(message, 1, accounts.Count);
            Account recipient = accounts[selection - 1];
            return recipient;
        }
    }
}
