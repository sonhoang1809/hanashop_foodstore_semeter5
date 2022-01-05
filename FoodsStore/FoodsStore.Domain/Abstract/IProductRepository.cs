using FoodsStore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace FoodsStore.Domain.Abstract
{
    public interface IProductRepository
    {
        IEnumerable<Product> Products { get; }

        void SaveProduct(Product product);

        Product GetProduct(int productID);

        bool UpdateOrderedProduct(Product pro);

        string GetProductName(int productID);

        int GetQuantity(int productID);

        int CountNumberProductHasCategory(IEnumerable<int> listCateID);

        int CountNumberProductHasCategory(int cateID);

        bool CheckProductBelongToCategory(int productID, int categoryID);

        IEnumerable<int> GetListIDProductHasCategory(int categoryID);

        double GetUnitPrice(int productID);

        string GetShortDescription(int productID);

        string GetStatus(int productID);

        byte[] GetImgData(int productID);

        string GetMimeType(int productID);

        bool CheckProductIsAvailableInStock(int productID);
    }
}
