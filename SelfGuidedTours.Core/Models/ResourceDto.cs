using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SelfGuidedTours.Core.Models
{
    public class ResourceDto
    {
        [Required]
        public IFormFile File { get; set; }

        public string FileName { get; set; }
       
    }
}
