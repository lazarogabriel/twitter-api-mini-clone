using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using twitter.api.application.Models.Users;
using twitter.api.application.Services.Abstractions;
using twitter.api.data.DbContexts;
using twitter.api.domain.Constants;
using twitter.api.domain.Exceptions;
using twitter.api.domain.Models;

namespace twitter.api.application.Services
{
    public class UserService : IUserService
    {
        #region Fields

        private readonly ITwitterApiDbContext _dbContext;

        #endregion

        #region Constructor

        public UserService(ITwitterApiDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        #endregion

        /// <inheritdoc/>
        public async Task<User> CreateUser(string username)
        {
            var user = new User(username);

            var exists = await _dbContext.Users.AnyAsync(u => u.Username == username);

            if (exists)
            {
                throw new RepeatedElementException(Errors.UsernameAlreadyExists);
            }

            await _dbContext.Users.AddAsync(user);
            await _dbContext.CommitAsync();

            return user;
        }

        /// <inheritdoc/>
        public async Task<List<BasicUserQueryResponse>> GetAllUsers()
        {
            var users = await _dbContext.Users
                .AsNoTracking()
                .Select(u => new BasicUserQueryResponse(u.Id, u.Username))
                .ToListAsync();

            return users;
        }
    }
}
