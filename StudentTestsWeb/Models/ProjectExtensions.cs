using Microsoft.EntityFrameworkCore;
using StudentTestsWeb.Models.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace StudentTestsWeb.Models {
    public static class ProjectExtensions {
        public static void DetachLocal<T>(this DbContext context, T t, string entryId)
            where T : class, IIdentifier {
            var local = context.Set<T>()
                .Local
                .FirstOrDefault(entry => entry.Id.Equals(entryId));
            if (local != null) {
                context.Entry(local).State = EntityState.Detached;
            }
            context.Entry(t).State = EntityState.Modified;
        }
    }
}
