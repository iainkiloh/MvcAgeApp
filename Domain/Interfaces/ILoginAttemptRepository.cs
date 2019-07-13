using Domain.Models;
using System.Linq;

namespace Domain.Interfaces
{
    public interface ILoginAttemptRepository
    {
        IQueryable<LoginAttempt> LoginAttempts { get; }

        void Save(LoginAttempt loginAttempt);

    }
}
