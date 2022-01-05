using FoodsStore.Domain.Abstract;
using FoodsStore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodsStore.Domain.Concrete
{
    public class EFKindRepository : IKindRepository
    {
        EFDbContext context = new EFDbContext();
        public IEnumerable<Kind> Kinds => context.Kinds;

    }
}
