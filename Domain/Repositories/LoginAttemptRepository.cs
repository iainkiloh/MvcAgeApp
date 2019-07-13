using Domain.Interfaces;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Domain.Repositories
{
    public class LoginAttemptRepository : ILoginAttemptRepository
    {

        private readonly ApplicationDbContext context;

        public LoginAttemptRepository(ApplicationDbContext ctx)
        {
            context = ctx;
        }

        public IQueryable<LoginAttempt> LoginAttempts => context.LoginAttempts;

        public void Save(LoginAttempt loginAttempt)
        {
            if(loginAttempt.Id == 0) {
                context.LoginAttempts.Add(loginAttempt);
            } else {
                LoginAttempt dbEntry = context.LoginAttempts.FirstOrDefault(x => x.Id == loginAttempt.Id);
                if(dbEntry != null) {
                    dbEntry.Email = loginAttempt.Email;
                    dbEntry.LoginAttemptTime = DateTime.Now;
                    dbEntry.LoginSuccess = loginAttempt.LoginSuccess;
                    dbEntry.Name = loginAttempt.Name;
                }
            }
            context.SaveChanges();
        }
    }
}
