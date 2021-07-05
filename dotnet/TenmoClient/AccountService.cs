﻿using RestSharp;
using RestSharp.Authenticators;
using System;
using TenmoClient.Models;
using System.Collections.Generic;

namespace TenmoClient
{
    public class AccountService
    {
        private readonly static string API_BASE_URL = "https://localhost:44315/";
        private readonly IRestClient client = new RestClient();

        public decimal GetBalance()
        {
            RestRequest request = new RestRequest(API_BASE_URL + "account");
            client.Authenticator = new JwtAuthenticator(UserService.GetToken());
            IRestResponse<decimal> response = client.Get<decimal>(request);
            if (response.ResponseStatus != ResponseStatus.Completed)
            {
                Console.WriteLine("An error occurred communicating with the server.");
                return 0;
            }
            else if (!response.IsSuccessful)
            {
                if (response == null)
                {
                    Console.WriteLine("An error message was received: " + response);
                }
                else
                {
                    Console.WriteLine("An error response was received from the server. The status code is " + (int)response.StatusCode);
                }
                return 0;
            }
            else
            {
                //return response;
                return response.Data;
            }
        }

        public List<OtherUser> GetUserList()
        {
            RestRequest request = new RestRequest(API_BASE_URL + "account" + "/" + "userslist");
            client.Authenticator = new JwtAuthenticator(UserService.GetToken());
            IRestResponse<List<OtherUser>> response = client.Get<List<OtherUser>>(request);
            if (response.ResponseStatus != ResponseStatus.Completed)
            {
                Console.WriteLine("An error occurred communicating with the server.");
                return null;
            }
            else if (!response.IsSuccessful)
            {
                if (response == null)
                {
                    Console.WriteLine("An error message was received: " + response);
                    return null;
                }
                else
                {
                    Console.WriteLine("An error response was received from the server. The status code is " + (int)response.StatusCode);
                    return null;
                }
            }
            else
            {
                //return response;
                return response.Data;
            }

            
        }


<<<<<<< HEAD
        public Transfer TransferTEBucks(Transfer transfer)
=======
        public bool TransferTEBucks(Transfer transfer)
>>>>>>> d1cfd824ccd206eef438fe631a1c601b7594b4fe
        {

            RestRequest request = new RestRequest(API_BASE_URL + "account");
            request.AddJsonBody(transfer);
            client.Authenticator = new JwtAuthenticator(UserService.GetToken());
            IRestResponse<bool> response = client.Put<bool>(request);
            if (response.ResponseStatus != ResponseStatus.Completed)
            {
                Console.WriteLine("An error occurred communicating with the server.");
<<<<<<< HEAD
                return null ;
=======
                return false;
>>>>>>> d1cfd824ccd206eef438fe631a1c601b7594b4fe
            }
            else if (!response.IsSuccessful)
            {
                if (response == null)
                {
                    Console.WriteLine("An error message was received: " + response);
                    return null;
                }
                else
                {
                    Console.WriteLine("An error response was received from the server. The status code is " + (int)response.StatusCode);
                    return null;
                }
            }
            else
            {
                //return response;
                return transfer;
            }

        }

        public List<Transaction> GetTransactions()
        {
            RestRequest request = new RestRequest(API_BASE_URL + "account" + "/" + "transfer");
            client.Authenticator = new JwtAuthenticator(UserService.GetToken());
            IRestResponse<List<Transaction>> response = client.Get<List<Transaction>>(request);
            if (response.ResponseStatus != ResponseStatus.Completed)
            {
                Console.WriteLine("An error occurred communicating with the server.");
                return null;
            }
            else if (!response.IsSuccessful)
            {
                if (response == null)
                {
                    Console.WriteLine("An error message was received: " + response);
                    return null;
                }
                else
                {
                    Console.WriteLine("An error response was received from the server. The status code is " + (int)response.StatusCode);
                    return null;
                }
            }
            else
            {
                return response.Data;
            }


        }
    }
}
