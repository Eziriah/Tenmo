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
                else if (menuSelection == 1) //showing balance
                {
                    Console.WriteLine($"Your current balance is: ${accountService.GetBalance()}");
                }
                else if (menuSelection == 2) //viewing past transfers
                {
                    //calling a method to create a list of transactions
                    List<Transaction> transactionsList = accountService.GetTransactions();
                    Console.WriteLine("Transfers\n ID   From:   To:    Amount:");
                    //printing transactions one at a time
                    foreach (Transaction transaction in transactionsList)
                    {
                        Console.WriteLine($"{transaction.TransferId} {transaction.SenderName}  {transaction.RecipientName} {transaction.AmountTransfered}\n");
                    }
                    //prompting user input for transaction ID to view more details
                    int transferInt = consoleService.PromptForTransferID("view");
                    //checking user input against list of transactions; for a match, will print details
                    foreach (Transaction transaction in transactionsList)
                    {
                        if (transaction.TransferId == transferInt)
                        {
                            Console.WriteLine($"TransferId: {transaction.TransferId}\nFrom: {transaction.SenderName}\nTo: {transaction.RecipientName}\nAmount Transferred: {transaction.AmountTransfered}\nStatus: {transaction.StatusString}\nType of transfer: {transaction.TypeString} \n");
                        }
                    }
                }
                else if (menuSelection == 3)//view pending(only what has been requested from other users) 
                {
                    List<Transaction> pendingTransactions = accountService.GetPendingTransactions();
                    Console.WriteLine("Pending ID:   Transfers from:   Amount:");
                    //printing transactions one at a time
                    foreach (Transaction transaction in pendingTransactions)
                    {
                        Console.WriteLine($"{transaction.TransferId}    {transaction.RecipientName}     {transaction.AmountTransfered}\n");
                    }
                    //"please enter transfer ID to approve/reject"
                    int transferInt = consoleService.PromptForTransferID("approve or reject");
                    //checking user input against list of transactions; for a match, will print details
                    foreach (Transaction transaction in pendingTransactions)
                    {
                        if (transaction.TransferId == transferInt)
                        {
                            int pendingInput = consoleService.PendingInput("1: Approve\n2: Reject\n0: Don't approve or reject");
                            if(pendingInput == 1 )
                            {
                                //need a method that will update pending to accepted
                                Transfer transfer = new Transfer();
                                transfer.AmountToTransfer = transaction.AmountTransfered;
                                transfer.UserIdToReceive = transaction.AccountFrom;
                                accountService.TransferTEBucks(transfer);
                            }
                            else if (pendingInput == 2)
                            {
                                //we need to create server-side PUT method for rejected
                            }
                        }
                    }
                }
                else if (menuSelection == 4) //transfer TE Bucks to other users
                {
                    //creating user list (not including ourselves) and printing out
                    List<OtherUser> otherUsers = accountService.GetUserList();
                    Console.WriteLine($"Here are available users:");
                    foreach (OtherUser users in otherUsers)
                    {
                        Console.WriteLine($"{users.UserId}, {users.Username}");
                    }
                    //getting user input for recipient user ID, making sure input is valid & matches user in DB
                    int userInt = consoleService.PromptForUserID("view");
                    bool userMatch = false;
                    foreach (OtherUser user in otherUsers)
                    {
                        if (user.UserId == userInt.ToString())
                        {
                            userMatch = true;
                        }
                    }
                    //if user ID from input matches DB:
                    if (userMatch)
                    {
                        //prompting user to enter transfer amt, making sure it is valid
                        decimal transferAmount = consoleService.GetTransferAmount("Enter the amount to be transferred");
                        //if amt is valid # and positive
                        if (transferAmount > 0 )
                        {
                            Transfer transfer = new Transfer();
                            transfer.UserIdToReceive = userInt;
                            transfer.AmountToTransfer = transferAmount;
                            //saving current balance 
                            decimal oldBalance = accountService.GetBalance();
                            accountService.TransferTEBucks(transfer);
                            decimal newBalance = accountService.GetBalance();
                            //comparing balances; if balance unchanged, unsuccessful
                            if (oldBalance == newBalance)
                            {
                                Console.WriteLine("Transaction unsuccessful");
                            }
                            else
                            {
                                Console.WriteLine("Transaction successful");
                            }
                        } 
                        //amt not valid
                        else
                        {
                            Console.WriteLine("Transfer amount must be greater than $0.00");
                        }
                    }
                    //user ID not valid
                    else
                    {
                        Console.WriteLine("Sorry, user ID invalid");
                    }
                }
                else if (menuSelection == 5)//request money//(list of users, amount to send. move to new pending trans. list)
                {
                    List<OtherUser> requests = accountService.GetUserList();
                    Console.WriteLine($"Here are available users:");
                    foreach (OtherUser users in requests)
                    {
                        Console.WriteLine($"{users.UserId}, {users.Username}");
                    }
                    //getting user input for recipient user ID, making sure input is valid & matches user in DB
                    int userInt = consoleService.PromptForUserID("view");
                    bool userMatch = false;
                    foreach (OtherUser user in requests)
                    {
                        if (user.UserId == userInt.ToString())
                        {
                            userMatch = true;
                        }
                    }
                    if (userMatch)
                    {
                        decimal requestAmount = consoleService.GetTransferAmount("Enter the amount to be requested");
                        //check to make sure positive
                        if (requestAmount > 0 )
                        {
                            Transfer transfer = new Transfer();
                            transfer.UserIdToReceive = userInt;
                            transfer.AmountToTransfer = requestAmount;
                            accountService.RequestTEBucks(transfer);
                        }
                        else
                        {
                            Console.WriteLine("Request amount must be greater than $0.00");
                        }
                 
                    }
                    else
                    {

                    }
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
