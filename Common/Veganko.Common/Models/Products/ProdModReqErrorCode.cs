using System;
using System.Collections.Generic;
using System.Text;

namespace Veganko.Common.Models.Products
{
    public enum ProdModReqErrorCode
    {
        Unknown,

        /// <summary>
        /// The request is not in state <see cref="ProductModRequestState.Pending"/>. It has already been handled.
        /// </summary>
        AlreadyHandled
    }
}
