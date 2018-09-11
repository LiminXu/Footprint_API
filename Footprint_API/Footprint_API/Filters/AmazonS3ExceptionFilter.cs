using Amazon.S3;
using Footprint_API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Net;

namespace Footprint_API.Filters
{
    public class AmazonS3ExceptionFilter : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            // Print out status code and error message after caught an amazon S3 exception.
            if (context.Exception is AmazonS3Exception)
            {
                AmazonS3Exception e = (AmazonS3Exception)context.Exception;
                context.Result = new JsonResult(new S3Response
                {
                    status = e.StatusCode,
                    Message = e.Message
                });
            }else if (context.Exception is Exception) //Print out status code and error message after caught any exception.
            {
                context.Result = new JsonResult(new S3Response
                {
                    status = HttpStatusCode.InternalServerError,
                    Message = context.Exception.Message
                });
                
            }
        }
    }
}
