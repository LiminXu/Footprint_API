using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ProtectedWebAPI.Controllers
{
    [Route("api/[controller]")]
    //[Authorize]
    public class ValuesController : ControllerBase
    {
        // GET api/values
        [HttpGet]
        [Authorize("Fullaccess")]
        public IEnumerable<string> Get()
        {
            //this is a basic code snippet to validate the scope inside the API
            /*bool userHasRightScope = User.HasClaim("scope", "scope.fullaccess");
            if (userHasRightScope == false)
            {
                return new string[] { "Access denied" };
            }*/

            return new string[] { "value1", "value2" };
        }

        // GET api/values/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
