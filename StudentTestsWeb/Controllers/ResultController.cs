using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentTestsWeb.Models;
using StudentTestsWeb.Models.Entities;
using StudentTestsWeb.Models.LoginModels;
using StudentTestsWeb.Models.MainModels;

namespace StudentTestsWeb.Controllers {
    public class ResultSearch {
        public string SearchQuery;
        public bool StudentsOnly;
        public bool GroupsOnly;
        public int sFrom;
        public int sTo;
        public int mFrom;
        public int mTo;

        private List<Test> tests;
        private List<Student> students;

        public ResultSearch() { }

        public ResultSearch(string s, bool st, bool gr, int sf, int se, int mf, int me,
            StudentTestsContext context) {
            SearchQuery = s;
            StudentsOnly = st;
            GroupsOnly = gr;
            sFrom = sf;
            sTo = se;
            mFrom = mf;
            mTo = me;

            tests = context.Tests.ToList();
            students = context.Students.ToList();
        }

        public Predicate<Result> GetPredicate() {
            Func<Result, bool> checkMarks = (res) => {
                int maxMark = tests.First(t => t.TestId == res.TestId).MaxMark;
                int maxScore = tests.First(t => t.TestId == res.TestId).MaxScore;

                int mark = res.Score * maxMark / maxScore;

                return res.Score >= sFrom && res.Score <= sTo &&
                    mark >= mFrom && mark <= mTo;
            };

            if (SearchQuery == null)
                return r => checkMarks(r);
            string s = SearchQuery.Trim().ToLower();
            return r => {
                Student student = students.First(e => e.StudentId == r.StudentId);
                string fio = $"{student.LastName} {student.FirstName} {student.FatherName}";

                bool stud = fio.ToLower().Contains(s);
                bool group = student.Group.ToLower().Contains(s);

                bool res = StudentsOnly && stud || GroupsOnly && group;
                return res && checkMarks(r);
            };

        }
    }

    public class ResultController : Controller {
        private UserContext usersDb;
        private StudentTestsContext mainDb;

        private Teacher Teacher {
            get {
                var teacher = mainDb.Teachers.First(t => t.Mail == User.Identity.Name);
                var disc = mainDb.Disciplines.Where(d => d.TeacherId == teacher.TeacherId);
                teacher.Disciplines = disc.ToHashSet();

                return teacher;
            }
        }

        public ResultController(UserContext usersContext, StudentTestsContext mainContext) {
            usersDb = usersContext;
            mainDb = mainContext;
        }

        [HttpGet]
        public async Task<IActionResult> ShowResults(int testId, int studentId, string sq, bool sOnly,
            bool gOnly, int sFrom, int mFrom, int sEnd, int mEnd) {
            ResultSearch rs = new ResultSearch(sq, sOnly, gOnly, sFrom, sEnd, mFrom, mEnd, mainDb);
            Test test = mainDb.Tests.FirstOrDefault(t => t.TestId == testId);
            test.Questions = await mainDb.Questions.Where(q => q.TestId == test.TestId).ToListAsync();

            var results = mainDb.Results.Where(r => testId == 0 ? true : r.TestId == testId &&
                (studentId == 0 ? true : r.StudentId == studentId) && rs.GetPredicate()(r));

            return View(test == null ? new ResultsModel() :
                new ResultsModel {
                    Test = test,
                    Results = await results.ToListAsync(),
                    Students = await mainDb.Students.ToListAsync()
                });
        }

        public async Task<IActionResult> DeleteResult(int testId, int studentId, bool st) {
            Test test = mainDb.Tests.First(t => t.TestId == testId);
            var res = mainDb.Results.First(r => r.TestId == testId && r.StudentId == studentId);
            if (res != null) {
                try {
                    mainDb.Results.Remove(res);
                    await mainDb.SaveChangesAsync();
                }
                catch {
                    // TODO
                }
            }
            if (st) {
                return RedirectToAction("StudentInfo", "Student", new { id = studentId });
            }
            else {
                return RedirectToAction("ShowResults", new {
                    testId,
                    sOnly = true,
                    sEnd = test.MaxScore,
                    mEnd = test.MaxMark
                });
            }
        }
    }
}