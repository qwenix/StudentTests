using StudentTestsWeb.Models.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace StudentTestsWeb.Models.LoginModels {
    public class RegisterModel {
        public IEnumerable<Discipline> Disciplines;

        public RegisterModel(IEnumerable<Discipline> disciplines) {
            Disciplines = disciplines ?? new List<Discipline>();
        }

        public RegisterModel() { }

        public Teacher Teacher { get; set; }

        public int AdminPassword { get; set; }

        [Required(ErrorMessage = "Не указан пароль")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Пароль введен неверно")]
        public string ConfirmPassword { get; set; }
    }
}
