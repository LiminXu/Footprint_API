using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using ConsoleTestClient.Services;
using NUnit.Framework;

namespace ConsoleTestClient.Tests
{
    [TestFixture]
    public class AuthenticationTest
    {
        private readonly AuthenticationService _authenticationService;

        public AuthenticationTest()
        {
            _authenticationService = new AuthenticationService();
        }

        [Test]
        public void TestAPILogginWithFullAccess()
        {
            string clientId = "ClientIdWithFullAccess";
            string clientSecret = "secret2";
            string scope = "scope.fullaccess";
            string api = "https://localhost:5001/api/values";
            string correctResult = "[\"value1\",\"value2\"]";
            string result = "";

            try
            {
                result = _authenticationService.LogginToAPI(clientId, clientSecret, scope, api);

                Assert.AreEqual(correctResult, result);
                printSuccessMessage(correctResult, result);
            }
            catch (Exception e)
            {
                printFailedMessage(correctResult, result, e.Message);
            }
        }

        [Test]
        public void TestAPILogginWithWrongPassword()
        {
            string clientId = "ClientIdWithFullAccess";
            string clientSecret = "secret1";
            string scope = "scope.fullaccess";
            string api = "https://localhost:5001/api/values";
            string correctResult = "Status Code: 401; Unauthorized";
            string result = "";

            try
            {
                result = _authenticationService.LogginToAPI(clientId, clientSecret, scope, api);

                Assert.AreEqual(correctResult, result);
                printSuccessMessage(correctResult, result);
            }
            catch (Exception e)
            {
                printFailedMessage(correctResult, result, e.Message);
            }
        }

        [Test]
        public void TestAPILogginWithWrongToken()
        {
            string clientId = "ClientIdWithFullAccess";
            string clientSecret = "secret2";
            string scope = "scope.fullaccess";
            string api = "https://localhost:5001/api/values";
            string correctResult = "Status Code: 401; Unauthorized";
            string result = "";

            try
            {
                result = _authenticationService.LogginToAPIWithWrongToken(clientId, clientSecret, scope, api);

                Assert.AreEqual(correctResult, result);
                printSuccessMessage(correctResult, result);
            }
            catch (Exception e)
            {
                printFailedMessage(correctResult, result, e.Message);
            }
        }

        [Test]
        public void TestAPILogginWithReadAccess()
        {
            string clientId = "ClientIdThatCanOnlyRead";
            string clientSecret = "secret1";
            string scope = "scope.readaccess";
            string api = "https://localhost:5001/api/values";
            string correctResult = "Status Code: 403; Forbidden";
            string result = "";

            try
            {
                result = _authenticationService.LogginToAPI(clientId, clientSecret, scope, api);

                Assert.AreEqual(correctResult, result);
                printSuccessMessage(correctResult, result);
            }
            catch (Exception e)
            {
                printFailedMessage(correctResult, result, e.Message);
            }
        }

        [Test]
        public void TestAPILogginWithTokenExpired()
        {
            string clientId = "ClientIdWithFullAccess";
            string clientSecret = "secret2";
            string scope = "scope.fullaccess";
            string api = "https://localhost:5001/api/values";
            string correctResult = "Status Code: 401; Unauthorized";
            string result = "";

            try
            {
                result = _authenticationService.LogginToAPIWithTokenExpired(clientId, clientSecret, scope, api);

                Assert.AreEqual(correctResult, result);
                printSuccessMessage(correctResult, result);
            }
            catch (Exception e)
            {
                printFailedMessage(correctResult, result, e.Message);
            }
        }


        private void printSuccessMessage(string correctResult, string result, [CallerMemberName] string memberName = "")
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine(memberName + " passed.");
            Console.WriteLine("Expected Result: " + correctResult);
            Console.WriteLine("Actual Result: " + result);
            Console.WriteLine("");
            Console.ResetColor();
        }

        private void printFailedMessage(string correctResult, string result, string errorMessage, [CallerMemberName] string memberName = "")
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine(MethodBase.GetCurrentMethod().Name + " failed.");
            Console.WriteLine("Error Message: " + errorMessage);
            Console.WriteLine("Expected Result: " + correctResult);
            Console.WriteLine("Actual Result: " + result);
            Console.WriteLine("");
            Console.ResetColor();
        }
    }
}
