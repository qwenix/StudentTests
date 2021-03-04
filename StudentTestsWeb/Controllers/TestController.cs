using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentTestsWeb.Models;
using StudentTestsWeb.Models.Entities;
using StudentTestsWeb.Models.LoginModels;
using StudentTestsWeb.Models.MainModels;

namespace StudentTestsWeb.Controllers {
    public class TestSearch {
        public string SearchQuery;
        public bool DisciplinesOnly;
        public bool TopicsOnly;
        public bool HasResultOnly;

        private List<Result> results;
        private Student student;


        public TestSearch() { }

        public TestSearch(string s, bool d, bool t, bool r, 
            StudentTestsContext context, Student student = null) {
            SearchQuery = s;
            DisciplinesOnly = d;
            TopicsOnly = t;
            HasResultOnly = r;
            results = context.Results.ToList();

            this.student = student;
        }

        public Predicate<Test> GetPredicateForTeacher() {
            if (SearchQuery == null)
                return t => HasResultOnly ? results.Any(re => re.TestId == t.TestId) : true;
            string s = SearchQuery.Trim().ToLower();
            return t => {
                bool d = t.Abbr.ToLower().Contains(s);
                bool tp = t.Topic.ToLower().Contains(s);
                bool r = results.Any(re => re.TestId == t.TestId);

                bool res = DisciplinesOnly && d || TopicsOnly && tp;
                return HasResultOnly ? res & r : res;
            };
        }

        public Predicate<Test> GetPredicateForStudent() {
            if (SearchQuery == null)
                return t => HasResultOnly ? !results.Any(re => re.TestId == t.TestId &&
                    re.StudentId == student.StudentId) : true;
            string s = SearchQuery.Trim().ToLower();
            return t => {
                bool d = t.Abbr.ToLower().Contains(s);
                bool tp = t.Topic.ToLower().Contains(s);
                bool r = results.Any(re => re.TestId == t.TestId && re.StudentId == student.StudentId);

                bool res = DisciplinesOnly && d || TopicsOnly && tp;
                return HasResultOnly ? res && !r: res;
            };
        }
    }

    public class TestController : Controller {
        private UserContext usersDb;
        private StudentTestsContext mainDb;

        private Teacher Teacher {
            get {
                Teacher teacher = mainDb.Teachers.First(t => t.Mail == User.Identity.Name);
                var disc = mainDb.Disciplines.Where(d => d.TeacherId == teacher.TeacherId);
                teacher.Disciplines = disc.ToHashSet();

                return teacher;
            }
        }

        private Student Student {
            get {
                Student student = mainDb.Students.First(s => s.Mail == User.Identity.Name);
                var results = mainDb.Results.Where(r => r.StudentId == student.StudentId);
                student.Results = results.ToList();

                return student;
            }
        }

        public TestController(UserContext usersContext, StudentTestsContext mainContext) {
            usersDb = usersContext;
            mainDb = mainContext;
        }

        [HttpGet]
        public async Task<IActionResult> TeacherTests(string sq, bool rOnly, bool dOnly = true, bool tOnly = true) {
            TestSearch sp = new TestSearch(sq, dOnly, tOnly, rOnly, mainDb);

            var result = await mainDb.Tests.Where(t => Teacher.Disciplines
                .Select(d => d.Abbr).Contains(t.Abbr) && sp.GetPredicateForTeacher()(t))
                .ToListAsync();

            return View(result);
        }

        [HttpGet]
        public async Task<IActionResult> StudentTests(string sq, bool rOnly, bool dOnly = true, bool tOnly = true) {
            TestSearch sp = new TestSearch(sq, dOnly, tOnly, rOnly, mainDb, Student);

            var result = await mainDb.Tests
                .Where(t => sp.GetPredicateForStudent()(t))
                .ToListAsync();

            return View(result);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteTest(int id) {
            var test = mainDb.Tests.First(t => t.TestId == id);
            string abbr = test.Abbr;

            if (test != null) {
                try {
                    mainDb.Tests.Remove(test);
                    await mainDb.SaveChangesAsync();
                }
                catch {
                    // TODO
                }
            }

            if (User.IsInRole("teacher"))
                return RedirectToAction("TeacherTests", "Test");
            else
                return RedirectToAction("TeachersList", "Teacher");
        }

        [HttpGet]
        public IActionResult TeacherTestInfo(int id) {
            Test test = mainDb.Tests.FirstOrDefault(t => t.TestId == id);
            AddQuestionsAndAnswers(test);

            return View(test);
        }

        public IActionResult StudentTestInfo(int id) {
            Test test = mainDb.Tests.FirstOrDefault(t => t.TestId == id);
            Result result = mainDb.Results.FirstOrDefault(r => r.TestId == id &&
            r.StudentId == Student.StudentId);
            return View(new StudentTestInfoModel(test, result));
        }

        [HttpGet]
        public IActionResult EditTest(int id) {
            Test test = mainDb.Tests.FirstOrDefault(t => t.TestId == id);

            if (test != null && test.TestId != 0)
                AddQuestionsAndAnswers(test);

            return View(new TestEditModel() { Test = test ?? new Test(), Teacher = Teacher });
        }

[HttpPost]
public async Task<IActionResult> EditTest(TestEditModel model) {
    // Валідація.
    if (ModelState.IsValid) {
        // Викладач, що створює тест.
        Teacher teacher = mainDb.Teachers.FirstOrDefault(t => t.Mail == User.Identity.Name);
        // Додаємо питання, відповіді, макс. оцінку.
        model.Test.Questions = model.Test.Questions.Where(q => q != null).ToList();
        foreach (var q in model.Test.Questions) {
            q.Answers = q.Answers.Where(a => a != null).ToList();
        }
        model.Test.MaxScore = model.Test.Questions.Count;

        // Перевіряємо: редагується чи створюється тест.
        if (model.Test.TestId != 0) {
            Test test = mainDb.Tests.First(t => t.TestId == model.Test.TestId);
            test.RemoveQuestions(mainDb);
            test.Update(model.Test);
        }
        else
            mainDb.Tests.Add(model.Test);

        await mainDb.SaveChangesAsync();
        return RedirectToAction("TeacherTests", "Test");
    }
    else
        return View(model);
}

        [HttpGet]
        public IActionResult PlayTest(int id) {
            Test test = mainDb.Tests.FirstOrDefault(t => t.TestId == id);
            bool r = mainDb.Results.Any(res => res.TestId == id && res.StudentId == Student.StudentId);

            if (test != null && test.TestId != 0) {
                AddQuestionsAndAnswers(test);
                test.Questions = test.Questions.ToList().Shuffle();
                foreach (var q in test.Questions) {
                    q.Answers = q.Answers.ToList().Shuffle();
                }
            }
            return View(test);
        }

        [HttpPost]
        public async Task<IActionResult> PlayTest(Test test, int time, int id) {
            Result result = new Result() {
                StudentId = Student.StudentId,
            };
            Test comparing = mainDb.Tests.First(t => t.TestId == (id == 0 ? test.TestId : id));
            AddQuestionsAndAnswers(comparing);

            if (id != 0) {
                result.TestId = id;
                result.Time = 0;

                foreach (Question q in comparing.Questions) {
                    q.TotalAnswers++;
                }
            }
            else {
                result.TestId = test.TestId;
                result.Time = time;

                foreach (Question q in test.Questions) {
                    Question c = comparing.Questions.First(qw => qw.Content == q.Content);
                    c.TotalAnswers++;

                    Answer trueAnsw = c.Answers.First(a => a.IsTrue);
                    Answer userAnsw = q.Answers.FirstOrDefault(a => a.IsTrue);

                    if (userAnsw != null && trueAnsw.Content == userAnsw.Content) {
                        result.Score++;
                        c.TrueAnswers++;
                    }
                }
            }

            mainDb.Results.Add(result);
            await mainDb.SaveChangesAsync();

            test.Results.Add(result);
            return id == 0 ? View("CheckTest", comparing) : null;
        }

        private void AddQuestionsAndAnswers(Test test) {
            test.Questions = mainDb.Questions.Where(q => q.TestId == test.TestId).ToList();
            foreach (var quest in test.Questions) {
                quest.Answers = mainDb.Answers.Where(a => a.QuestionId == quest.QuestionId).ToList();
            }
        }
    }
}