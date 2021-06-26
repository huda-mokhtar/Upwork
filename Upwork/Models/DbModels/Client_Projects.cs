using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Upwork.Models.DbModels
{
    public class Client_Projects
    {
        [ForeignKey("Project")]
        public int ProjectId { get; set; }

        [ForeignKey("Client")]
        public string ClientId { get; set; }

        public bool? IsSaved { get; set; } = false;

        [Range(0, 5)]
        public int Rate { get; set; }
        public Project Project { get; set; }
        public Client Client { get; set; }
    }
}
