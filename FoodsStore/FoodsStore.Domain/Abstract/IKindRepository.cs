using FoodsStore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodsStore.Domain.Abstract {
    public interface IKindRepository {

        IEnumerable<Kind> Kinds { get; }
    }
}
