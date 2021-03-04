using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentTestsWeb.Models;
using StudentTestsWeb.Models.Entities;

namespace StudentTestsWeb.Controllers {
    public class HomeController : Controller {
        private StudentTestsContext db;

        public HomeController(StudentTestsContext context) {
            db = context;
        }

        [Authorize]
        public IActionResult Index() {
            string controllerName = "Teacher";
            string actionName = "TeachersList";

            if (User.IsInRole("teacher")) {
                controllerName = "Test";
                actionName = "TeacherTests";
            }
            else if (User.IsInRole("student")) {
                controllerName = "Test";
                actionName = "StudentTests";
            }

            return RedirectToAction(actionName, controllerName);
        }

        public IActionResult Create() {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Student student) {
            db.Students.Add(student);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
