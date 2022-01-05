using FoodsStore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FoodsStore.WebUI.Models {
    public class UserListEditModel {
        public IEnumerable<Role> Roles { get; set; }
        public IEnumerable<User> Users { get; set; }
        public string SelectedRoleId { get; set; }
        public PagingInfo PagingInfo { get; set; }
    }
}