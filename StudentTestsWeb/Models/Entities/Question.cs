using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentTestsWeb.Models.Entities {
    public partial class Question {
        public Question() {
            Answers = new HashSet<Answer>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int QuestionId { get; set; }
        public string Content { get; set; }
        public int TestId { get; set; }
        public int TrueAnswers { get; set; }
        public int TotalAnswers { get; set; }

        public virtual Test Test { get; set; }
        public virtual ICollection<Answer> Answers { get; set; }
    }
}
