using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ProtectedWebAPI.Controllers
{
    [Route("api/[controller]")]
    //[Authorize]   // attribute used for authorized the class level.
    public class ValuesController : ControllerBase
    {
        /// <summary>
        /// Basic API used in the testing suits
        /// GET api/values
        /// </summary>
        /// <returns>A string array with 2 strings in it. used in demo and unit testing for server authorization.</returns>

        [HttpGet]
        [Authorize("Fullaccess")]   // attribute used for authorized the method level.
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
