using ConsoleTestClient.Tests;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace ConsoleTestClient
{
    class Program
    {
        static void Main(string[] args)
        {
            AuthenticationTest test = new AuthenticationTest();
            Console.WriteLine("Client Console Test Start...");
            test.TestAPILogginWithFullAccess();
            test.TestAPILogginWithWrongPassword();
            test.TestAPILogginWithWrongToken();
            test.TestAPILogginWithReadAccess();
            test.TestAPILogginWithTokenExpired();
            Console.WriteLine("Client Console Test Completed.");
            Console.ReadKey();
        }
    }
}
