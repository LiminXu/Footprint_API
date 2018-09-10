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
            //authorization server parameters owned from the client
            //this values are issued from the authorization server to the client through a separate process (registration, etc...)
            Uri authorizationServerTokenIssuerUri = new Uri("https://localhost:5002/connect/token");

            //Access with a user with full access right.
            string clientId = "ClientIdWithFullAccess";
            string clientSecret = "secret2";
            string scope = "scope.fullaccess";

            //Access with a user with read only access right.
            /*string clientId = "ClientIdThatCanOnlyRead";
            string clientSecret = "secret1";
            string scope = "scope.readaccess"*/;

            //send access token request
            string rawJwtToken = RequestTokenToAuthorizationServer(
                 authorizationServerTokenIssuerUri,
                 clientId,
                 scope,
                 clientSecret)
                .GetAwaiter()
                .GetResult();

            AuthorizationServerAnswer authorizationServerToken;
            authorizationServerToken = Newtonsoft.Json.JsonConvert.DeserializeObject<AuthorizationServerAnswer>(rawJwtToken);

            if (authorizationServerToken.error == null)
            {
                Console.WriteLine("Token acquired from Authorization Server:");
                Console.WriteLine(authorizationServerToken.access_token);
                Console.WriteLine("Token expires after: " + authorizationServerToken.expires_in + " seconds.");
                
            }
            else
            {
                Console.WriteLine("Authorization failed: No token is acquired from Authorization Server.");
                Console.WriteLine("Error message from Authorization server: " + authorizationServerToken.error);
            }

            //invoke secured web api request with security token
            //authorizationServerToken.access_token = authorizationServerToken.access_token + "R"; // Sending the wrong token.
            string response = RequestValuesToSecuredWebApi(authorizationServerToken)
                .GetAwaiter()
                .GetResult();

            Console.WriteLine("");
            Console.WriteLine("Response received from WebAPI:");
            Console.WriteLine(response);

            Console.ReadKey();
        }

        private static async Task<string> RequestTokenToAuthorizationServer(Uri uriAuthorizationServer, string clientId, string scope, string clientSecret)
        {
            HttpResponseMessage responseMessage;
            using (HttpClient client = new HttpClient())
            {
                HttpRequestMessage tokenRequest = new HttpRequestMessage(HttpMethod.Post, uriAuthorizationServer);
                HttpContent httpContent = new FormUrlEncodedContent(
                    new[]
                    {
                    new KeyValuePair<string, string>("grant_type", "client_credentials"),
                    new KeyValuePair<string, string>("client_id", clientId),
                    new KeyValuePair<string, string>("scope", scope),
                    new KeyValuePair<string, string>("client_secret", clientSecret)
                    });
                tokenRequest.Content = httpContent;
                responseMessage = await client.SendAsync(tokenRequest);
            }
            return await responseMessage.Content.ReadAsStringAsync();
        }

        private static async Task<string> RequestValuesToSecuredWebApi(AuthorizationServerAnswer authorizationServerToken)
        {
            HttpResponseMessage responseMessage;
            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authorizationServerToken.access_token);
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, "https://localhost:5001/api/values");
                responseMessage = await httpClient.SendAsync(request);
            }

            return await responseMessage.Content.ReadAsStringAsync();
        }

        private class AuthorizationServerAnswer
        {
            public string access_token { get; set; }
            public string expires_in { get; set; }
            public string token_type { get; set; }
            public string error { get; set; }
        }
    }
}
