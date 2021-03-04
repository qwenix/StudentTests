using System;
using System.Collections.Generic;

namespace StudentTestsWeb.Models.Entities {
    public partial class Discipline {
        public Discipline() {
            Tests = new HashSet<Test>();
        }

        public string Abbr { get; set; }
        public string FullName { get; set; }
        public string Description { get; set; }
        public int TeacherId { get; set; }

        public virtual Teacher Teacher { get; set; }
        public virtual ICollection<Test> Tests { get; set; }
    }
}
