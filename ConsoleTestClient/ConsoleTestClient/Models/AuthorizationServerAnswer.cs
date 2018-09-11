using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleTestClient.Models
{
    public class AuthorizationServerAnswer
    {
        public string access_token { get; set; }
        public string expires_in { get; set; }
        public string token_type { get; set; }
        public string error { get; set; }
    }
}
