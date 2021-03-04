using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace StudentTestsWeb.Models.Entities {
    public partial class Teacher {
        public Teacher() {
            Disciplines = new HashSet<Discipline>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TeacherId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FatherName { get; set; }
        public string Department { get; set; }
        public string Position { get; set; }
        public string Mail { get; set; }

        public virtual ICollection<Discipline> Disciplines { get; set; }
    }
}
