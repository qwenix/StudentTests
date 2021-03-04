using Microsoft.EntityFrameworkCore;
using StudentTestsWeb.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudentTestsWeb.Models {
    public static class EntitiesExtensions {
        public static void RemoveQuestions(this Test test, StudentTestsContext context) {
            if (test == null && context == null)
                throw new ArgumentNullException();

            if (context is StudentTestsContext db) {
                db.Questions.RemoveRange(db.Questions
                    .Where(q => q.TestId == test.TestId));
            }
        }

        public static void Update(this Test test, Test other) {
            if (test != null && other != null) {
                test.Abbr = other.Abbr;
                test.MaxMark = other.MaxMark;
                test.MaxScore = other.MaxScore;
                test.Questions = other.Questions;
                test.Time = other.Time;
                test.Topic = other.Topic;
            }
            else
                throw new ArgumentNullException();
        }

        public static List<T> Shuffle<T>(this List<T> seq) {
            int length = seq == null ? 0 : seq.Count;
            if (length > 1) {
                Random r = new Random();
                List<T> t = new List<T>();
                while (t.Count < length) {
                    T next = seq[r.Next(length)];
                    if (!t.Contains(next))
                        t.Add(next);
                }
                return t;
            }
            return seq;
        }
    }
}
