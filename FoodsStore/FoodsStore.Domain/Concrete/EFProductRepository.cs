using FoodsStore.Domain.Abstract;
using FoodsStore.Domain.Entities;

using System.Collections.Generic;
using System.Linq;


namespace FoodsStore.Domain.Concrete {

    public class EFProductRepository : IProductRepository {

        EFDbContext context = new EFDbContext();

        public IEnumerable<Product> Products => context.Products;

        public byte[] GetImgData(int productID)
        {
           return Products.FirstOrDefault(p => p.ProductID == productID).ImageData;
        }

        public Product GetProduct(int productID)
        {
            return Products.FirstOrDefault(p => p.ProductID == productID);
        }

        public string GetProductName(int productID)
        {
            return (from p in Products
                    where p.ProductID == productID
                    select  p.ProductName).SingleOrDefault();
        }

        public int GetQuantity(int productID)
        {
            int quantity = (from p in Products
                            where p.ProductID == productID
                            select p.Quantity).Sum();
            return quantity;
        }

        public string GetShortDescription(int productID)
        {
            string result = Products.FirstOrDefault(p => p.ProductID == productID).Description;
            if (result.Length > 90)
            {
                result = result.Substring(0, 90)+ "...";
            }
            return result;
        }

        public string GetStatus(int productID)
        {
            string result = Products.FirstOrDefault(p => p.ProductID == productID).StatusCode;
            return result;
        }

        public string GetMimeType(int productID)
        {
            return Products.FirstOrDefault(p => p.ProductID == productID).ImageMimeType;
        }

        public double GetUnitPrice(int productID)
        {
            double result = (from p in Products
                             where p.ProductID == productID
                             select p.Price).Sum();
            return result;
        }

        public void SaveProduct(Product product) //I need to check product is valid or not in AdminController.
        {
            if (product.ProductID == 0) {
                //Add new product
                context.Products.Add(product);
            } else {
                //Update product
                Product prod = Products.FirstOrDefault(p => p.ProductID == product.ProductID);
                if (prod != null) {
                    prod.ProductName = product.ProductName;
                    prod.Description = product.Description;
                    prod.Quantity = product.Quantity;
                    prod.Price = product.Price;
                    prod.CategoryID = product.CategoryID;
                    prod.StatusCode = product.StatusCode;
                    //if (product.DateCreated != null) { // comment for prohibit admin change DateCreated
                    //    prod.DateCreated = product.DateCreated;
                    //}
                    if (product.DateModified != null) {
                        prod.DateModified = product.DateModified;
                    }
                    if(product.UserModified != null) {
                        prod.UserModified = product.UserModified;
                    }
                    if(product.ImageData != null) {
                        prod.ImageData = product.ImageData;
                        prod.ImageMimeType = product.ImageMimeType;
                    }
                }
            }
            context.SaveChanges();
        }

        public bool UpdateOrderedProduct(Product product)
        {
            bool result = false;
            Product prod = GetProduct(product.ProductID);
            if (prod != null)
            {
                prod.Quantity = product.Quantity;
                prod.StatusCode = product.StatusCode;
                context.SaveChanges();
                result = true;
            }
            return result;
        }

        public IEnumerable<int> GetListIDProductHasCategory(int categoryID)
        {
            return (from p in Products
                    where p.CategoryID == categoryID
                    select p.ProductID);
        }

        public int CountNumberProductHasCategory(IEnumerable<int> listCateID)
        {
            return (from p in Products
                    where listCateID.Contains(p.CategoryID)
                    select p.ProductID).Count();
        }

        public bool CheckProductBelongToCategory(int productID, int categoryID)
        {
            return Products.Any(p => p.ProductID == productID && p.CategoryID == categoryID);
        }

        public int CountNumberProductHasCategory(int cateID)
        {
            return (from p in Products
                    where p.CategoryID==cateID
                    select p.ProductID).Count();
        }

        public bool CheckProductIsAvailableInStock(int productID)
        {
            return Products.Any(p => p.ProductID == productID && p.StatusCode.Equals("STOC"));
        }
    }
}
