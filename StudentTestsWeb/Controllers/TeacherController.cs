using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentTestsWeb.Models;
using StudentTestsWeb.Models.Entities;
using StudentTestsWeb.Models.LoginModels;

namespace StudentTestsWeb.Controllers {
    public class TeacherSearch {
        List<Test> _tests;
        List<Discipline> _disc;

        public string SearchQuery;
        public bool FullNameOnly;
        public bool DepartmentsOnly;

        public bool PosOnly;
        public bool MailOnly;
        public bool testOnly;

        public TeacherSearch() { }

        public TeacherSearch(string s, bool f, bool g, 
            bool pos, bool test, bool mail, StudentTestsContext context) {
            SearchQuery = s;
            FullNameOnly = f;
            DepartmentsOnly = g;
            PosOnly = pos;
            MailOnly = mail;
            testOnly = test;
            _tests = context.Tests.ToList();
            _disc = context.Disciplines.ToList();

            if (f == false && g == false && PosOnly == false)
                FullNameOnly = true;
        }

        public Predicate<Teacher> GetPredicate() {
            return s => {
                bool mail = s.Mail.Contains("@nure.ua");
                bool test = _tests.Any(t => _disc.Any(d => d.TeacherId == s.TeacherId && d.Abbr == t.Abbr));

                if (SearchQuery == null)
                    return mail && testOnly ? test : true;
                else {
                    string query = SearchQuery.Trim().ToLower();
                    bool f = $"{s.LastName} {s.FirstName} {s.FatherName}".ToLower().Contains(query);
                    bool g = s.Department.ToLower().Contains(query);
                    bool p = s.Position.ToLower().Contains(query);

                    bool res = FullNameOnly && f || DepartmentsOnly && g || PosOnly && p;
                    return res && mail && testOnly ? test : true;
                }
            };
        }
    }

    public class TeacherController : Controller {
        private UserContext usersDb;
        private StudentTestsContext mainDb;

        public TeacherController(UserContext usersContext, StudentTestsContext mainContext) {
            usersDb = usersContext;
            mainDb = mainContext;
        }

        [HttpGet]
        public async Task<IActionResult> TeachersList(string sq, bool gOnly,  
            bool pOnly, bool tOnly, bool kOnly, bool fOnly = true) {
            TeacherSearch ts = new TeacherSearch(sq, fOnly, gOnly, pOnly, tOnly, kOnly, mainDb);
            return View(await mainDb.Teachers.Where(t => ts.GetPredicate()(t)).ToListAsync());
        }

        public async Task<IActionResult> TeacherInfo(int id) {
            Teacher teacher = mainDb.Teachers.FirstOrDefault(t => t.TeacherId == id);
            var disciplines = mainDb.Disciplines.Where(d => d.TeacherId == teacher.TeacherId);
            foreach (var d in disciplines) {
                d.Teacher = teacher;
                d.Tests = await mainDb.Tests.Where(t => t.Abbr == d.Abbr).ToListAsync();
            }

            return View(await disciplines.ToListAsync());
        }

        public async Task<IActionResult> DeleteTeacher(int id) {
            var teacher = mainDb.Teachers.First(t => t.TeacherId == id);
            var user = usersDb.Users.First(u => u.Email == teacher.Mail);
            if (teacher != null) {
                try {
                    mainDb.Teachers.Remove(teacher);
                    usersDb.Users.Remove(user);
                    await mainDb.SaveChangesAsync();
                    await usersDb.SaveChangesAsync();
                }
                catch {
                    // TODO
                }
            }
            return RedirectToAction("TeachersList");
        }
    }
}