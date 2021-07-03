using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Upwork.Models.ViewModels.Projects
{
    public class ImageModel
    {
        public int id { get; set; }
        public IFormFile Image { get; set; }
        public string imageName { get; set; }
    }
}
