using System;
using System.Collections.Generic;

namespace StudentTestsWeb.Models.Entities {
    public partial class Result {
        public int StudentId { get; set; }
        public int TestId { get; set; }
        public int Score { get; set; }
        public int Time { get; set; }

        public virtual Student Student { get; set; }
        public virtual Test Test { get; set; }
    }
}
