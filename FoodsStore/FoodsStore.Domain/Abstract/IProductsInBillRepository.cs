using FoodsStore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodsStore.Domain.Abstract
{
    public interface IProductsInBillRepository
    {
        IEnumerable<ProductsInBill> ProductsInBills { get; }

        bool InsertProductToBill(ProductsInBill prod);

        bool UpdateProductInBill(ProductsInBill dto);

       // bool UpdateProductUnitPriceInBill(ProductsInBill dto);
        //bool UpdateProductNameInBill(ProductsInBill dto);

        bool RemoveAProductInBill(int idBill, int idProduct);

        bool CheckBillContainProduct(int idBill, int idProduct);

        IEnumerable<ProductsInBill> GetListProductInBill(int idBill);

        IEnumerable<int> GetListProductIdInBill(int idBill);

        IEnumerable<ProductsInBill> GetListProductIdInListOrderedBill(IEnumerable<int> listBillID);

        ProductsInBill GetDetailsAProductInBill(int idBill, int idProduct);

        int GetNumProductIsSoldHasCate(IEnumerable<int> listIdBillOrdered, IEnumerable<int> listIDProductHasCate);

        int GetQuantityAProductInBill(int idBill, int idProduct);

        double GetUnitPriceAProductInBill(int idBill, int idProduct);

        double GetTotalAProductInBill(int idBill, int idProduct);

        double GetToTalOfABill(int idBill);
    }
}
