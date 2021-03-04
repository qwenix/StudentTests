using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StudentTestsWeb.Models;
using StudentTestsWeb.Models.Entities;
using StudentTestsWeb.Models.LoginModels;

namespace StudentTestsWeb.Controllers
{
    public class AccountController : Controller {
        private UserContext usersDb;
        private StudentTestsContext mainDb; 

        public AccountController(UserContext usersContext, StudentTestsContext mainContext) {
            usersDb = usersContext;
            mainDb = mainContext;
        }

        [HttpGet]
        public IActionResult Login() {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model) {
            if (ModelState.IsValid) {
                User user = await usersDb.Users
                    .Include(u => u.Role)
                    .FirstOrDefaultAsync(u => u.Email == model.Email && u.Password == model.Password);
                if (user != null) {
                    await Authenticate(user); // аутентификация

                    if (user.Role.Name == "teacher")
                        return RedirectToAction("TeacherTests", "Test", new { dOnly = true, tOnly = true });
                    else if (user.Role.Name == "admin")
                        return RedirectToAction("TeachersList", "Teacher");
                    else
                        return RedirectToAction("StudentTests", "Test", new { dOnly = true, tOnly = true });
                }
                ModelState.AddModelError("", "Некорректные логин и(или) пароль");
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult Register() {
            return View(new RegisterModel(mainDb.Disciplines));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterModel model) {
            // Валідація моделі
            if (ModelState.IsValid) {
                // Запит до бази даних користувачів
                User user = await usersDb.Users.FirstOrDefaultAsync(u => u.Email == model.Teacher.Mail);
                if (user == null) {
                    // Додаємо користувача в бд
                    Role userRole = await usersDb.Roles.FirstOrDefaultAsync(r => r.Name == "teacher");
                    user = new User { Email = model.Teacher.Mail, Password = model.Password };
                    if (userRole != null)
                        user.Role = userRole;

                    usersDb.Users.Add(user);
                    await usersDb.SaveChangesAsync();

                    // Додаємо викладача
                    if (!mainDb.Teachers.Select(t => t.TeacherId).Contains(model.Teacher.TeacherId)) {
                        model.Teacher.Disciplines = model.Teacher.Disciplines.Where(d => d != null).ToList();
                        mainDb.Teachers.Add(model.Teacher);
                        await mainDb.SaveChangesAsync();
                    }

                    await Authenticate(user); // Аутентификация

                    return RedirectToAction("TeacherTests", "Test");
                }
                else
                    ModelState.AddModelError("", "Некорректные логин и(или) пароль");
            }
            return View(model);
        }

        private async Task Authenticate(User user) {
            // создаем один claim
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.Email),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, user.Role?.Name),

            };
            // создаем объект ClaimsIdentity
            ClaimsIdentity id = new ClaimsIdentity(claims, "ApplicationCookie", 
                ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
            // установка аутентификационных куки
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, 
                new ClaimsPrincipal(id));
        }

        public async Task<IActionResult> Logout() {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Account");
        }
    }
}