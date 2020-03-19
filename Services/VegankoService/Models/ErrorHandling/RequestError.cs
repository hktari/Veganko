using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace VegankoService.Models.ErrorHandling
{
    public class RequestError
    {
        public RequestError()
        {
            Errors = new Dictionary<string, List<string>>();
        }

        public RequestError(string fieldName, string errorMessage)
        {
            Errors = new Dictionary<string, List<string>>();
            Errors.Add(fieldName, new List<string>{ errorMessage });
        }

        //public RequestError(IEnumerable<IdentityError> errors)
        //{
        //    Errors = errors.Select(idErr => new KeyValuePair<string, List<string>>(idErr.Code, ))    
        //}

        /// <summary>
        /// Errors per field name.
        /// </summary>
        public Dictionary<string, List<string>> Errors { get; set; }

        public int StatusCode { get; private set; } = (int)HttpStatusCode.BadRequest;

        public RequestError SetStatusCode(int statusCode)
        {
            StatusCode = statusCode;
            return this;
        }

        public void Add(string fieldName, string errorMessage)
        {
            if (!Errors.TryGetValue(fieldName, out List<string> fieldErrs))
            {
                Errors[fieldName] = fieldErrs = new List<string>();
            };

            fieldErrs.Add(errorMessage);
        }

        public void Add(IEnumerable<KeyValuePair<string, string>> errors)
        {
            foreach (var err in errors)
            {
                Add(err.Key, err.Value);
            }
        }

        public IActionResult ToActionResult()
        {
            ValidationProblemDetails probDetail = ToValidProblemDetails();
            return new BadRequestObjectResult(probDetail);
        }

        public ValidationProblemDetails ToValidProblemDetails()
        {
            var probDetail = new ValidationProblemDetails
            {
                Status = StatusCode,
            };
            foreach (var err in Errors)
            {
                probDetail.Errors.Add(err.Key, err.Value.ToArray());
            }

            return probDetail;
        }
    }
}
