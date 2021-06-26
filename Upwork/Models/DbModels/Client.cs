using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Upwork.Models.DbModels
{
    public class Client
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [ForeignKey("User")]
        public string ClientId { get; set; }

        //public List<Jobs> Jobs { get; set; }
        public virtual ApplicationUser User { get; set; }
        public List<Client_Projects> client_Projects { get; set; }
    }
}
