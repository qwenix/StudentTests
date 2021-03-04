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
using System.Net.Mail;
using System.Net;

namespace StudentTestsWeb.Controllers {
    public class StudentSearch {
        public string SearchQuery;
        public bool FullNameOnly;
        public bool GroupsOnly;
        public bool HasResultOnly;
        public bool FiveOnly;
        public bool TwoOnly;

        private List<Result> results;

        public StudentSearch() { }

        public StudentSearch(string s, bool f, bool g, bool h, bool fv, bool tw, StudentTestsContext context) {
            SearchQuery = s;
            FullNameOnly = f;
            GroupsOnly = g;
            HasResultOnly = h;
            FiveOnly = fv;
            TwoOnly = tw;
            if (f == false && g == false)
                FullNameOnly = true;

            results = context.Results.ToList();
            foreach (var r in results) {
                r.Test = context.Tests.First(t => t.TestId == r.TestId);
            }
        }

        public Predicate<Student> GetPredicate() {
            if (SearchQuery == null)
                return s => {
                    bool r = results.Any(re => re.StudentId == s.StudentId);
                    var sr = results.Where(rez => rez.StudentId == s.StudentId);
                    bool fv = false;
                    bool tw = true;
                    if (sr?.Count() > 0) {
                        fv = sr.All(re => re.Score * 100 / re.Test.MaxScore >= 90);
                        tw = sr.All(re => re.Score * 100 / re.Test.MaxScore < 60);
                    }

                    bool c = (HasResultOnly ? r : true) && ((TwoOnly ? tw : true) && TwoOnly ? true : (FiveOnly ? fv : true));
                    return c;
                };
            string query = SearchQuery.Trim().ToLower();
            return s => {
                bool f = $"{s.LastName} {s.FirstName} {s.FatherName}".ToLower().Contains(query);
                bool g = s.Group.ToLower().Contains(query);
                bool r = results.Any(re => re.StudentId == s.StudentId);
                bool tw = results.Where(rez => rez.StudentId == s.StudentId)
                    .All(re => re.Score * 100 / re.Test.MaxScore >= 90);
                bool fv = results.Where(rez => rez.StudentId == s.StudentId)
                    .All(re => re.Score * 100 / re.Test.MaxScore < 60);

                bool res = FullNameOnly && f || GroupsOnly && g;
                return res && (HasResultOnly ? r : true) && (TwoOnly ? tw : true) && (FiveOnly ? fv : true);
            };
        }
    }

    public class StudentController : Controller {
        private UserContext usersDb;
        private StudentTestsContext mainDb;

        public StudentController(UserContext usersContext, StudentTestsContext mainContext) {
            usersDb = usersContext;
            mainDb = mainContext;
        }

        [HttpGet]
        public async Task<IActionResult> StudentsList(string sq, bool gOnly, bool fvOnly, bool twOnly, bool hOnly, bool fOnly = true) {
            StudentSearch ss = new StudentSearch(sq, fOnly, gOnly, hOnly, fvOnly, twOnly, mainDb);
            return View(await mainDb.Students.Where(s => ss.GetPredicate()(s)).ToListAsync());
        }

        [HttpGet]
        public async Task<IActionResult> StudentInfo(int id) {
            Student student = mainDb.Students.FirstOrDefault(s => s.StudentId == id);
            var results = mainDb.Results.Where(r => r.StudentId == id);

            if (User.IsInRole("teacher")) {
                Teacher teacher = mainDb.Teachers.FirstOrDefault(t => t.Mail == User.Identity.Name);
                var disciplines = mainDb.Disciplines.Where(d => d.TeacherId == teacher.TeacherId);
                var tests = mainDb.Tests.Where(t => disciplines.Select(d => d.Abbr).Contains(t.Abbr));
                results = results.Where(r => tests.Select(t => t.TestId).Contains(r.TestId));
            }

            foreach (var res in results) {
                res.Test = mainDb.Tests.First(t => t.TestId == res.TestId);
            }
            student.Results = results == null ? new List<Result>() : await results.ToListAsync();

            return View(student);
        }

[HttpPost]
public async Task<IActionResult> DeleteStudent(int id) {
    var student = mainDb.Students.First(s => s.StudentId == id);
    var user = usersDb.Users.First(u => u.Email == student.Mail);
    if (student != null) {
        try {
            mainDb.Students.Remove(student);
            usersDb.Users.Remove(user);
            await mainDb.SaveChangesAsync();
            await usersDb.SaveChangesAsync();
        }
        catch {
            throw new Exception();
        }
    }
    return RedirectToAction("StudentsList");
}

        [HttpGet]
        public IActionResult Edit(int id) {
            Student student = new Student();
            if (id != 0) {
                student = mainDb.Students.First(s => s.StudentId == id);
            }
            return View(student);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Student student) {
            if (ModelState.IsValid) {
                User user = await usersDb.Users.FirstOrDefaultAsync(u => u.Email == student.Mail);
                if (student.StudentId == 0) {
                    // добавляем пользователя в бд
                    Role userRole = await usersDb.Roles.FirstOrDefaultAsync(r => r.Name == "student");
                    user = new User { Email = student.Mail, Password = GeneratePass(student.Mail) };
                    if (userRole != null)
                        user.Role = userRole;

                    student.Password = user.Password;

                    usersDb.Users.Add(user);
                    mainDb.Students.Add(student);
                    await usersDb.SaveChangesAsync();
                    await mainDb.SaveChangesAsync();

                    return RedirectToAction("StudentsList", "Student");
                }
                else {
                    Student st = mainDb.Students.First(s => s.StudentId == student.StudentId);
                    user = usersDb.Users.First(u => u.Email == st.Mail);

                    st.FirstName = student.FirstName;
                    st.LastName = student.LastName;
                    st.FatherName = student.FatherName;
                    st.Mail = student.Mail;
                    st.Group = student.Group;
                    st.Password = user.Password;
                    user.Email = student.Mail;

                    await mainDb.SaveChangesAsync();
                    await usersDb.SaveChangesAsync();

                    return RedirectToAction("StudentsList", "Student");
                }
            }
            return View(student);
        }

        private string GeneratePass(string mail) {
            string pass = "123";
            try {
                MailAddress from = new MailAddress("kdeniis2088@gmail.com", "Open Test 3.0");
                MailAddress to = new MailAddress(mail);
                MailMessage m = new MailMessage(from, to);
                m.Subject = "Пароль от учетной записи";
                m.Body = $"<h2>{pass}</h2>";
                m.IsBodyHtml = true;
                SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
                // логин и пароль
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential("kdeniis2088@gmail.com", "ujlpts5E2088");
                smtp.EnableSsl = true;
                smtp.Send(m);
            }
            catch {
                return "123";
            }
            return pass;
        }
    }
}