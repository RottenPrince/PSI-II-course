﻿using System.Net;

namespace MVC
{
    public class APIError
    {
        public HttpStatusCode Status { get; private set; }
        public string Message { get; private set; }
        public APIError(HttpStatusCode statusCode, string message)
        {
            Status = statusCode;
            Message = message;
        }
    }
}
