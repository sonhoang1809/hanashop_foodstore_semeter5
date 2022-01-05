using FoodsStore.Domain.Abstract;
using FoodsStore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodsStore.Domain.Concrete {
    public class EFRoleRepository : IRoleRepository {
        EFDbContext context = new EFDbContext();
        public IEnumerable<Role> Roles => context.Roles;
    }
}
