using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentTestsWeb.Models.Entities {
    public partial class Test {
        public Test() {
            Questions = new HashSet<Question>();
            Results = new HashSet<Result>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TestId { get; set; }
        public string Abbr { get; set; }
        public string Topic { get; set; }
        public int MaxScore { get; set; }
        public int MaxMark { get; set; }
        public int Time { get; set; }

        public virtual Discipline AbbrNavigation { get; set; }
        public virtual ICollection<Question> Questions { get; set; }
        public virtual ICollection<Result> Results { get; set; }
    }
}
