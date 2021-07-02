using System;
using System.Collections.Generic;
using TenmoClient.Models;

namespace TenmoClient
{
    class Program
    {
        private static readonly ConsoleService consoleService = new ConsoleService();
        private static readonly AuthService authService = new AuthService();
        private static readonly AccountService accountService = new AccountService();
        

        static void Main(string[] args)
        {
            Run();
        }

        private static void Run()
        {
            int loginRegister = -1;
            while (loginRegister != 1 && loginRegister != 2)
            {
                Console.WriteLine("Welcome to TEnmo!");
                Console.WriteLine("1: Login");
                Console.WriteLine("2: Register");
                Console.Write("Please choose an option: ");

                if (!int.TryParse(Console.ReadLine(), out loginRegister))
                {
                    Console.WriteLine("Invalid input. Please enter only a number.");
                }
                else if (loginRegister == 1)
                {
                    while (!UserService.IsLoggedIn()) //will keep looping until user is logged in
                    {
                        LoginUser loginUser = consoleService.PromptForLogin();
                        ApiUser user = authService.Login(loginUser);
                        if (user != null)
                        {
                            UserService.SetLogin(user);
                        }
                    }
                }
                else if (loginRegister == 2)
                {
                    bool isRegistered = false;
                    while (!isRegistered) //will keep looping until user is registered
                    {
                        LoginUser registerUser = consoleService.PromptForLogin();
                        isRegistered = authService.Register(registerUser);
                        if (isRegistered)
                        {
                            Console.WriteLine("");
                            Console.WriteLine("Registration successful. You can now log in.");
                            loginRegister = -1; //reset outer loop to allow choice for login
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Invalid selection.");
                }
            }

            MenuSelection();
        }

        private static void MenuSelection()
        {
            int menuSelection = -1;
            while (menuSelection != 0)
            {
                Console.WriteLine("");
                Console.WriteLine("Welcome to TEnmo! Please make a selection: ");
                Console.WriteLine("1: View your current balance");
                Console.WriteLine("2: View your past transfers");
                Console.WriteLine("3: View your pending requests");
                Console.WriteLine("4: Send TE bucks");
                Console.WriteLine("5: Request TE bucks");
                Console.WriteLine("6: Log in as different user");
                Console.WriteLine("0: Exit");
                Console.WriteLine("---------");
                Console.Write("Please choose an option: ");

                if (!int.TryParse(Console.ReadLine(), out menuSelection))
                {
                    Console.WriteLine("Invalid input. Please enter only a number.");
                }
                else if (menuSelection == 1)
                {
                    Console.WriteLine($"Your current balance is: ${accountService.GetBalance()}");
                }
                else if (menuSelection == 2)
                {
                    accountService.GetTransactions();
                    List<Transaction> transactionsList = accountService.GetTransactions();
                    
                    foreach (Transaction transaction in transactionsList)
                    {
                        Console.WriteLine($"Account from: {transaction.SenderName}\n Account to: {transaction.RecipientName}\n Amount Transfered: {transaction.AmountTransfered}\n Status: {transaction.StatusString}\n Type of transfer: {transaction.TypeString} \n");
                    }
                }
                else if (menuSelection == 3)
                {

                }
                else if (menuSelection == 4)
                {
                    //TODO: remove current user from list displayed back
                    List<OtherUser> otherUsers = accountService.GetUserList();
                    Console.WriteLine($"Here are available users:");
                    foreach (OtherUser users in otherUsers)
                    {
                        Console.WriteLine($"{users.UserId}, {users.Username}");
                    }
                    //TODO: handle bad input
                    Console.WriteLine("Please enter user Id you wish to transfer to: ");
                    string userIdToTransferTo = Console.ReadLine();
                    Console.WriteLine("Enter the amount to be transferred");
                    string transferAmount =  Console.ReadLine();

                    Transfer transfer = new Transfer();
                    transfer.UserIdToReceive = int.Parse(userIdToTransferTo);
                    transfer.AmountToTransfer = Convert.ToDecimal(transferAmount);
                   
                    if (accountService.TransferTEBucks(transfer))
                    {
                        Console.WriteLine("successful transaction");
                    }
                    else
                    {
                        Console.WriteLine("transaction declined");
                    }

                    //,aybe we can mimic a list of users the user can select from and have a readkey 

                }
                else if (menuSelection == 5)
                {

                }
                else if (menuSelection == 6)
                {
                    Console.WriteLine("");
                    UserService.SetLogin(new ApiUser()); //wipe out previous login info
                    Run(); //return to entry point
                }
                else
                {
                    Console.WriteLine("Goodbye!");
                    Environment.Exit(0);
                }
            }
        }
    }
}
