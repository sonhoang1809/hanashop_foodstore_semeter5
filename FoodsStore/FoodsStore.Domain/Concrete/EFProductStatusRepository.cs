using FoodsStore.Domain.Abstract;
using FoodsStore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodsStore.Domain.Concrete
{
    public class EFProductStatusRepository : IProductStatusRepository
    {
        EFDbContext context = new EFDbContext();
        public IEnumerable<ProductStatus> ProductStatus => context.ProductStatus;

    }
}
