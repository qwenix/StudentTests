using StudentTestsWeb.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudentTestsWeb.Models.MainModels {
    public class StudentInfoModel {
        public Student Student { get; set; }

        public StudentInfoModel() { }

        public StudentInfoModel(List<Test> teacherTests, Student student) {
            Student = student;
        }
    }
}
