using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Upwork.Models.ViewModels.Register
{
    public class ProfilePhotoViewModel
    {
        public IFormFile File { get; set; }

        public string Image { get; set; }
    }
}
