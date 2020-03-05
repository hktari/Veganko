using System;
using System.Collections.Generic;
using System.Text;

namespace Veganko.Services.Http.Errors
{
    /// <summary>
    /// See ValidationProblemDetails.
    /// https://docs.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.problemdetails.instance?view=aspnetcore-2.1#Microsoft_AspNetCore_Mvc_ProblemDetails_Instance
    /// </summary>
    public class RequestError
    {
        public Nullable<int> Status { get; set; }

        public string Instance { get; set; }
        public string Title { get; set; }
    }
}
