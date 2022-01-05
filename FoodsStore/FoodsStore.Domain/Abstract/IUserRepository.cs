using FoodsStore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodsStore.Domain.Abstract {
    public interface IUserRepository {

        IEnumerable<User> Users { get; }

        void InsertUser(User user);

        User GetUser(string username);

        string GetFullName(string username);

        bool CheckLoginDefault(string username, string password);

        bool CheckLoginAnotherAccount(string username, string email);

        string GetRoleName(string username);

        void UpdateUser(User user);

        bool CheckExistUsername(string username);

        bool CheckExistEmail(string email);
    }
}
