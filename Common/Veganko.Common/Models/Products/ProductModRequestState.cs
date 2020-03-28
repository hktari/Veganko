using System.ComponentModel;

namespace Veganko.Common.Models.Products
{
    public enum ProductModRequestState 
    {
        [Description("V ČAKANJU")]
        Pending,

        [Description("ODOBRENO")]
        Approved,

        [Description("ZAVRNJENO")]
        Rejected,

        /// <summary>
        /// The product to be edited was missing
        /// </summary>

        [Description("IZGINJENO")]
        Missing
    }
}
