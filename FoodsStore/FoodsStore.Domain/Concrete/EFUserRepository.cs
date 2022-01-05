using FoodsStore.Domain.Abstract;
using FoodsStore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodsStore.Domain.Concrete {
    public class EFUserRepository : IUserRepository {

        EFDbContext context = new EFDbContext();

        public IEnumerable<User> Users => context.Users;
        //phải gọi từ context mới truy vấn đc ???
        public bool CheckExistEmail(string email)
        {
            return context.Users.Any(u => u.Email.Equals(email));
        }

        public bool CheckExistUsername(string username)
        {
            return context.Users.Any(u => u.Username.Equals(username));
        }

        public bool CheckLoginAnotherAccount(string username, string email)
        {
            return context.Users.Any(u => u.Username.Equals(username) && u.Email.Equals(email));
        }

        public bool CheckLoginDefault(string username, string password)
        {
            return context.Users.Any(u => u.Username.Equals(username) && u.Password.Equals(password));
        }

        public string GetFullName(string username)
        {
            string fullname = Users.FirstOrDefault(u => u.Username.Equals(username)).FullName;
            return fullname;
        }

        public string GetRoleName(string username) {
            string roleID = context.Users.FirstOrDefault(u => u.Username.Equals(username)).RoleID;
            string roleName = context.Roles.FirstOrDefault(r => r.RoleID.Equals(roleID)).RoleName;
            return roleName;
        }

        public User GetUser(string username)
        {
            return Users.FirstOrDefault(u => u.Username.Equals(username));
        }

        public void InsertUser(User user)
        {
            context.Users.Add(user);
            context.SaveChanges();
        }
        /// <summary>
        /// Don't update user.Username & user.Password
        /// </summary>
        /// <param name="user"></param>
        public void UpdateUser(User user) { 
            User entity = context.Users.FirstOrDefault(u => u.Username.Equals(user.Username));
            if(entity != null) {
                entity.FullName = user.FullName;
                entity.Description = user.Description;
                entity.Email = user.Email;
                entity.RoleID = user.RoleID;
                if(user.ImageData != null && !string.IsNullOrEmpty(user.ImageMimeType)) {
                    entity.ImageData = user.ImageData;
                    entity.ImageMimeType = user.ImageMimeType;
                }
                context.SaveChanges();
            }
        }
    }
}
