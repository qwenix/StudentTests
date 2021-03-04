using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentTestsWeb.Models.Entities {
    public partial class Student {
        public Student() {
            Results = new HashSet<Result>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int StudentId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FatherName { get; set; }
        public string Group { get; set; }
        public string Mail { get; set; }
        public string Password { get; set; }

        public virtual ICollection<Result> Results { get; set; }
    }
}
