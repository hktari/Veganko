using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace VegankoService.Models.ErrorHandling
{
    public class DuplicateProblemDetails : ProblemDetails
    {
        public DuplicateProblemDetails(object duplicateItem, params string[] conflictingFields)
        {
            Status = (int)HttpStatusCode.Conflict;
            ConflictingItem = duplicateItem;
            ConflictingFields = conflictingFields;
        }

        /// <summary>
        /// The duplicate item
        /// </summary>
        public object ConflictingItem { get; set; }
        
        public string[] ConflictingFields { get; set; }
    }
}
