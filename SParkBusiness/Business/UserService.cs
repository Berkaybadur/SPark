using MongoDB.Driver;
using SParkBusiness.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Driver;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace SParkBusiness.Business
{
    public class UserService
    {
        private readonly IMongoCollection<User> _users;
        private readonly PasswordHasher<User> _hasher = new();

        public UserService(IConfiguration config)
        {
            var client = new MongoClient(config.GetConnectionString("MongoDb"));
            var database = client.GetDatabase("UserDb");
            _users = database.GetCollection<User>("Users");
        }

        public async Task<User> Register(Register model)
        {
            var user = new User { Username = model.Username };
            user.PasswordHash = _hasher.HashPassword(user, model.Password);

            await _users.InsertOneAsync(user);
            return user;
        }

        public async Task<User?> Authenticate(Login model)
        {
            var user = await _users.Find(u => u.Username == model.Username).FirstOrDefaultAsync();
            if (user == null) return null;

            var result = _hasher.VerifyHashedPassword(user, user.PasswordHash, model.Password);
            return result == PasswordVerificationResult.Success ? user : null;
        }

        public async Task<List<User>> GetAllUsers() =>
            await _users.Find(_ => true).ToListAsync();
    }
}

