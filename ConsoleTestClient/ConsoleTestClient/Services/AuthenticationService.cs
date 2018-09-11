using ConsoleTestClient.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace ConsoleTestClient.Services
{
    public class AuthenticationService
    {
        //authorization server parameters owned from the client
        //this values are issued from the authorization server to the client through a separate process (registration, etc...)
        Uri authorizationServerTokenIssuerUri = new Uri("http://localhost:5002/connect/token");

        public string LogginToAPI(string clientId, string clientSecret, string scope, string api)
        {
            try
            {
                string rawJwtToken = RequestTokenToAuthorizationServer(
                     authorizationServerTokenIssuerUri,
                     clientId,
                     scope,
                     clientSecret)
                    .GetAwaiter()
                    .GetResult();

                AuthorizationServerAnswer authorizationServerToken;
                authorizationServerToken = Newtonsoft.Json.JsonConvert.DeserializeObject<AuthorizationServerAnswer>(rawJwtToken);
                //Console.WriteLine("Token: " + authorizationServerToken.access_token);
                //Console.WriteLine("Token expires after: " + authorizationServerToken.expires_in + " seconds.");
                
                //invoke secured web api request with security token
                string response = RequestValuesToSecuredWebApi(authorizationServerToken, api)
                    .GetAwaiter()
                    .GetResult();
                return response.Trim();
            }catch(Exception e){
                Console.WriteLine("Error: " + e.Message);
                if (e.InnerException != null)
                {
                    Console.WriteLine("Inner Exception: " + e.InnerException.Message);
                }
                Console.WriteLine(e.StackTrace);
                return "Error: " + e.Message;
            }
        }

        public string LogginToAPIWithWrongToken(string clientId, string clientSecret, string scope, string api)
        {
            try
            {
                string rawJwtToken = RequestTokenToAuthorizationServer(
                     authorizationServerTokenIssuerUri,
                     clientId,
                     scope,
                     clientSecret)
                    .GetAwaiter()
                    .GetResult();

                AuthorizationServerAnswer authorizationServerToken;
                authorizationServerToken = Newtonsoft.Json.JsonConvert.DeserializeObject<AuthorizationServerAnswer>(rawJwtToken);
                authorizationServerToken.access_token = authorizationServerToken.access_token + "A";
                //Console.WriteLine("Token: " + authorizationServerToken.access_token);
                //Console.WriteLine("Token expires after: " + authorizationServerToken.expires_in + " seconds.");

                //invoke secured web api request with security token
                string response = RequestValuesToSecuredWebApi(authorizationServerToken, api)
                    .GetAwaiter()
                    .GetResult();
                return response.Trim();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.Message);
                if (e.InnerException != null)
                {
                    Console.WriteLine("Inner Exception: " + e.InnerException.Message);
                }
                Console.WriteLine(e.StackTrace);
                return "Error: " + e.Message;
            }
        }

        public string LogginToAPIWithTokenExpired(string clientId, string clientSecret, string scope, string api)
        {
            try
            {
                string rawJwtToken = RequestTokenToAuthorizationServer(
                     authorizationServerTokenIssuerUri,
                     clientId,
                     scope,
                     clientSecret)
                    .GetAwaiter()
                    .GetResult();

                AuthorizationServerAnswer authorizationServerToken;
                authorizationServerToken = Newtonsoft.Json.JsonConvert.DeserializeObject<AuthorizationServerAnswer>(rawJwtToken);
                Console.WriteLine("Token: " + authorizationServerToken.access_token);
                Console.WriteLine("Token expires after: " + authorizationServerToken.expires_in + " seconds.");
                Task.Delay(20000).Wait();
                //invoke secured web api request with security token
                string response = RequestValuesToSecuredWebApi(authorizationServerToken, api)
                    .GetAwaiter()
                    .GetResult();
                return response.Trim();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.Message);
                if (e.InnerException != null)
                {
                    Console.WriteLine("Inner Exception: " + e.InnerException.Message);
                }
                Console.WriteLine(e.StackTrace);
                return "Error: " + e.Message;
            }
        }

        private async Task<string> RequestTokenToAuthorizationServer(Uri uriAuthorizationServer, string clientId, string scope, string clientSecret)
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
                    new KeyValuePair<string, string>("client_secret", clientSecret),
                    });
                tokenRequest.Content = httpContent;
                responseMessage = await client.SendAsync(tokenRequest);
            }
            return await responseMessage.Content.ReadAsStringAsync();
        }

        private async Task<string> RequestValuesToSecuredWebApi(AuthorizationServerAnswer authorizationServerToken, string api)
        {
            HttpResponseMessage responseMessage;
            using (HttpClient httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authorizationServerToken.access_token);
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, api);
                responseMessage = await httpClient.SendAsync(request);
            }

            return await responseMessage.Content.ReadAsStringAsync();
        }
    }
}
