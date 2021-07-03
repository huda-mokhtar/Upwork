using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Upwork.Models.MessageModels
{
    public class Message
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        [Required]
        [StringLength(4096, MinimumLength = 1)]
        public string Text { get; set; }
        public DateTime When { get; set; }
        [ForeignKey("ApplicationUser")]
        public String UserId { get; set; }
        [ForeignKey("ApplicationUser")]
        public string ReceiverId { get; set; }
        public virtual ApplicationUser Sender { get; set; }
    }
}
