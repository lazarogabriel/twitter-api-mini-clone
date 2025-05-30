using System.Collections.Generic;
using System.Threading.Tasks;
using twitter.api.application.Models.Users;
using twitter.api.domain.Models;

namespace twitter.api.application.Services.Abstractions
{
    public interface IUserService
    {
        /// <summary>
        /// Creates a user.
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        Task<User> CreateUser(string username);

        /// <summary>
        /// Gets all users.
        /// </summary>
        /// <returns></returns>
        Task<List<BasicUserQueryResponse>> GetAllUsers();
    }
}
