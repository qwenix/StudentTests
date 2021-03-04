using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudentTestsWeb.Models {
    public static class PassGenerator {
        public static string GetPass(int x = 8) {
            // х - довжина паролю.

            string pass = "";
            var r = new Random();
            while (pass.Length < x) {
                // Випадковим чином отримуємо символ з таблиці ASCII,
                // номер якого лежить в діапазоні (33; 125).
                char c = (char)r.Next(33, 125);

                // Беремо тільки цифру або літеру.
                if (char.IsLetterOrDigit(c))
                    pass += c;
            }
            return pass;
        }
    }
}
