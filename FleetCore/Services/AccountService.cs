﻿using FleetCore.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace FleetCore.Services
{
    public interface IAccountService
    {
        Dictionary<int, string> Authenticate(LoginModel model);
        Task<bool> Create(CreateUserModel model);
        IEnumerable<AppUser> GetAll();
        IEnumerable<Log> GetLogs();
        Task<bool> ChangePassword(ChangePasswordModel model);
        Task<bool> DeleteUser(string fullname);
        Task<bool> ResetPassword(string fullname);
        Task<bool> ChangeRole(string fullname);
    }
    public class AccountService : IAccountService
    {
        private readonly FleetCoreDbContext _dbContext;

        public AccountService(FleetCoreDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public Dictionary<int, string> Authenticate(LoginModel model)
        {
            var user = _dbContext
                .Users
                .FirstOrDefault(x => x.UserName == model.UserName && x.Password == model.Password);

            if (user is null)
            {
                return null;
            }

            Dictionary<int, string> datas = new Dictionary<int, string>
                {
                    { 0, user.FullName},
                    { 1, user.Role },
                    { 2, user.Id.ToString() }
                };
            return datas;

        }
        public async Task<bool> Create(CreateUserModel model)
        {
            string username = ($"{model.FirstName.Substring(0, 1)}{model.LastName}").ToLower();
            var dbCheck = _dbContext
                .Users
                .FirstOrDefault(x=>x.UserName== username);

            bool registerSuccess = false;
            if (dbCheck is null)
            {
                var user = new AppUser
                {
                    UserName = username,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    FullName = $"{model.FirstName} {model.LastName}",
                    Role = "User",
                    Password = "Prima123."
                };
                _dbContext.Users.Add(user);
                _dbContext.SaveChanges();
                registerSuccess = true;
            }
            return registerSuccess;
        }
        public IEnumerable<AppUser> GetAll()
        {
            return _dbContext.Users.ToList();
        }
        public async Task<bool> ChangePassword(ChangePasswordModel model)
        {
            var user = _dbContext.Users.FirstOrDefault(x => x.Id.ToString().Equals(model.userId));
            if (user == null) return false;
            
            user.Password = model.Password;
            _dbContext.Update(user);
            _dbContext.SaveChanges();
            return true;
        }
        public async Task<bool> DeleteUser(string fullname)
        {
            var user = _dbContext.Users.FirstOrDefault(x=>x.FullName.Equals(fullname));
            if(user is null) return false;
            else
            {
                _dbContext.Remove(user);
                _dbContext.SaveChanges();
                return true;
            }
        }
        public async Task<bool> ResetPassword(string fullname)
        {
            var user = _dbContext.Users.FirstOrDefault(x => x.FullName.Equals(fullname));
            if (user is null) return false;
            else
            {
                user.Password = "Prima123";
                _dbContext.SaveChanges();
                return true;
            }
        }

        public async Task<bool> ChangeRole(string fullname)
        {
            var user = _dbContext.Users.FirstOrDefault(x => x.FullName.Equals(fullname));
            if (user is null) return false;
            else
            {
                if(user.Role.Equals("User"))
                {
                    user.Role = "Moderator";
                }
                else if(user.Role.Equals("Moderator"))
                {
                    user.Role = "User";
                }

                _dbContext.SaveChanges();
                return true;
            }
        }

        public IEnumerable<Log> GetLogs()
        {
            var oldDate = DateTime.Now.AddDays(-7);
            var dates = _dbContext.Logs.Where(x => x.Date < oldDate);
            _dbContext.Logs.RemoveRange(dates);
            _dbContext.SaveChanges();

            return _dbContext.Logs.OrderByDescending(x=>x.Date).ToList();
        }
        
    }
}
