using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net;

namespace Footprint_API.Models
{
    public class S3Response
    {
        public HttpStatusCode status { get; set; }
        public string Message { get; set; }

    }
}
