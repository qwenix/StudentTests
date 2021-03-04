using StudentTestsWeb.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudentTestsWeb.Models.MainModels {
    public class TestEditModel {
        public Test Test { get; set; }
        public Teacher Teacher { get; set; }

        public TestEditModel() { }

        public TestEditModel(Test test, Teacher teacher) {
            Test = test;
            Teacher = teacher;
        }
    }
}
