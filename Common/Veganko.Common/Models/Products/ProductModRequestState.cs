namespace Veganko.Common.Models.Products
{
    public enum ProductModRequestState 
    {
        Pending,
        Approved,
        Rejected,

        /// <summary>
        /// The product to be edited was missing
        /// </summary>
        Missing
    }
}
