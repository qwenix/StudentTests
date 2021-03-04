using StudentTestsWeb.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudentTestsWeb.Models.MainModels {
    public class StudentTestInfoModel {
        public Test Test { get; set; }
        public Result Result { get; set; }

        public StudentTestInfoModel() { }

        public StudentTestInfoModel(Test test, Result result) {
            Test = test;
            Result = result;
        }
    }
}
