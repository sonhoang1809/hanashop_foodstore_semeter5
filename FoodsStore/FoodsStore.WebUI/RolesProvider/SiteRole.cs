using FoodsStore.Domain.Concrete;
using FoodsStore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
/// <summary>
/// Provide roles for authorize user
/// Setting at tag <roleManager></roleManager> in Web.config
/// </summary>
namespace FoodsStore.WebUI.RolesProvider {
    public class SiteRole : RoleProvider {
        public override string ApplicationName { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public override void AddUsersToRoles(string[] usernames, string[] roleNames) {
            throw new NotImplementedException();
        }

        public override void CreateRole(string roleName) {
            throw new NotImplementedException();
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole) {
            throw new NotImplementedException();
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch) {
            throw new NotImplementedException();
        }

        public override string[] GetAllRoles() {
            throw new NotImplementedException();
        }

        public override string[] GetRolesForUser(string username) {
            EFDbContext context = new EFDbContext();
            string roleID = context.Users.FirstOrDefault(u => u.Username.Equals(username)).RoleID;
            string roleName = context.Roles.FirstOrDefault(r => r.RoleID.Equals(roleID)).RoleName;
            string[] result = { roleName };
            return result;
        }

        public override string[] GetUsersInRole(string roleName) {
            throw new NotImplementedException();
        }

        public override bool IsUserInRole(string username, string roleName) {
            throw new NotImplementedException();
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames) {
            throw new NotImplementedException();
        }

        public override bool RoleExists(string roleName) {
            throw new NotImplementedException();
        }
    }
}