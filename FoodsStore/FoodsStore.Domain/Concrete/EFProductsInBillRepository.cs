using FoodsStore.Domain.Abstract;
using FoodsStore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodsStore.Domain.Concrete
{
    public class EFProductsInBillRepository : IProductsInBillRepository
    {
        EFDbContext context = new EFDbContext();

        public IEnumerable<ProductsInBill> ProductsInBills => context.ProductsInBills;

        public bool CheckBillContainProduct(int idBill, int idProduct)
        {
            return ProductsInBills.Where(p => p.BillID == idBill && p.ProductID == idProduct).Any();
        }

        public ProductsInBill GetDetailsAProductInBill(int idBill, int idProduct)
        {
            return ProductsInBills.FirstOrDefault(p => p.BillID == idBill && p.ProductID == idProduct);
        }

        public IEnumerable<int> GetListProductIdInBill(int idBill)
        {
            IEnumerable<int> result = (from p in ProductsInBills
                                          where p.BillID == idBill
                                          select p.ProductID);
            return result;
        }

        public IEnumerable<ProductsInBill> GetListProductIdInListOrderedBill(IEnumerable<int> listBillID)
        {
            return (from p in ProductsInBills
                    where listBillID.Contains(p.BillID)
                    select p);
        }

        public IEnumerable<ProductsInBill> GetListProductInBill(int idBill)
        {
            IEnumerable<ProductsInBill> result = null;
            if (ProductsInBills.Any())
            {
                result = ProductsInBills.Where(p => p.BillID == idBill);
            }
            return result;
        }

        public int GetNumProductIsSoldHasCate(IEnumerable<int> listIdBillOrdered, IEnumerable<int> listIDProductHasCate)
        {
            return (from p in ProductsInBills
                    where listIdBillOrdered.Contains(p.BillID) && listIDProductHasCate.Contains(p.ProductID)
                    select p.Quantity).Count();
        }

        public int GetQuantityAProductInBill(int idBill, int idProduct)
        {
            int quantity = (from prod in ProductsInBills
                            where prod.BillID == idBill && prod.ProductID == idProduct
                            select prod.Quantity).Sum();
            return quantity;
        }

        public double GetTotalAProductInBill(int idBill, int idProduct)
        {
            double total = (from prod in ProductsInBills
                            where prod.BillID == idBill && prod.ProductID == idProduct
                            select prod.Total).Sum();
            return total;
        }

        public double GetToTalOfABill(int idBill)
        {
            double total = (from prod in ProductsInBills
                            where prod.BillID == idBill
                            select prod.Total).Sum();
            return total;
        }

        public double GetUnitPriceAProductInBill(int idBill, int idProduct)
        {
            double price = (from prod in ProductsInBills
                               where prod.BillID == idBill && prod.ProductID == idProduct
                               select prod.Price).Sum();
            return price;
        }

        public bool InsertProductToBill(ProductsInBill prod)
        {
            if (CheckBillContainProduct(prod.BillID, prod.ProductID))
            {
                ProductsInBill productsInBill = GetDetailsAProductInBill(prod.BillID, prod.ProductID);
                int newQuantity = GetQuantityAProductInBill(prod.BillID, prod.ProductID) + prod.Quantity;
                productsInBill.Quantity = newQuantity;
                productsInBill.Total = newQuantity * GetUnitPriceAProductInBill(prod.BillID, prod.ProductID);
            }
            else
            {
                context.ProductsInBills.Add(prod);
            }            
            context.SaveChanges();
            return true;
        }

        public bool RemoveAProductInBill(int idBill, int idProduct)
        {
            ProductsInBill prod = ProductsInBills.FirstOrDefault(p => p.BillID == idBill && p.ProductID == idProduct);
            if (prod != null)
            {
                context.ProductsInBills.Remove(prod);
                context.SaveChanges();
            }        
            return true;
        }

        public bool UpdateProductInBill(ProductsInBill pr)
        {
            bool result = false;
            if (ProductsInBills.Any())
            {
                ProductsInBill prod = ProductsInBills.FirstOrDefault(p => p.BillID == pr.BillID && p.ProductID == pr.ProductID);
                if (prod != null)
                {
                    prod.ProductName = pr.ProductName;
                    prod.Quantity = pr.Quantity;
                    prod.Price = pr.Price;
                    prod.Total = pr.Total;
                    context.SaveChanges();
                }
            }
            return result;
        }
    }
}
