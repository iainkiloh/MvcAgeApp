using Domain.Interfaces;
using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Domain.Repositories
{
    public class FakeLoginAttemptRepository : ILoginAttemptRepository
    {


        public IQueryable<LoginAttempt> LoginAttempts => new List<LoginAttempt>
        {
            new LoginAttempt{ Email="kiloh@mail.com", Id=1, LoginSuccess=true, Name="iain", LoginAttemptTime= DateTime.Now },
            new LoginAttempt{ Email="skiloh@mail.com", Id=2, LoginSuccess=true, Name="saul", LoginAttemptTime= DateTime.Now }
        }.AsQueryable<LoginAttempt>();

        public void Save(LoginAttempt loginAttempt)
        {
            throw new NotImplementedException();
        }
    }
}
