using FoodsStore.Domain.Abstract;
using FoodsStore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodsStore.Domain.Concrete
{
    public class EFCategoryRepository : ICategoryRepository {
        EFDbContext context = new EFDbContext();

        public IEnumerable<Category> Categories => context.Categories;

        public IEnumerable<int> GetListCategoryBelongToKind(int kindID)
        {
            return (from c in Categories
                    where c.KindID == kindID
                    select c.CategoryID);
        }
    }
}
