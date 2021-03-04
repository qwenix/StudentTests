using StudentTestsWeb.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudentTestsWeb.Models.MainModels {
    public class ResultsModel {
        public Test Test { get; set; }
        public IEnumerable<Result> Results { get; set; }
        public IEnumerable<Student> Students { get; set; }

        public ResultsModel() { }

        public ResultsModel(IEnumerable<Result> results, IEnumerable<Student> students, Test test) {
            Test = test;
            Results = results;
            Students = students;
        }
    }
}
