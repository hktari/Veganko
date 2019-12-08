using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace VegankoService.Models.Products
{
    public class ProductImageInput
    {
        [Required]
    	public IFormFile DetailImage { get; set; }

        [Required]
        public IFormFile ThumbImage { get; set; }
    }
}
