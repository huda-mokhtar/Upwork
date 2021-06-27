using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Upwork.Models.DbModels
{
    public class JobQuestions
    {
        [ForeignKey("Jobs")]
        public int JobsId { get; set; }
        public Jobs Jobs { get; set; }

        [ForeignKey("ReviewJobQuestion")]
        public int ReviewJobQuestionId { get; set; }
        public ReviewJobQuestion ReviewJobQuestion { get; set; }
    }
}
