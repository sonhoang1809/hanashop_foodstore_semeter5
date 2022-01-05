using FoodsStore.Domain.Abstract;
using FoodsStore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodsStore.Domain.Concrete
{
    public class EFBillRepository : IBillRepository
    {
        EFDbContext context = new EFDbContext();

        public IEnumerable<Bill> Bills => context.Bills;        

        public IEnumerable<Bill> GetListBillOfUser(string username)
        {
            IEnumerable<Bill> result = null;
            if (Bills.Any())
            {
                result = Bills.Where(b => b.Username.Equals(username));
            }
            return result;
        }

        public int GetNumberOfBillIsOrdered()
        {
            int result = 0;
            if (Bills.Any())
            {
                result = Bills.Where(b => b.BillStatusCode.Equals("PAY")).Count();
            }
            return result;
        }

        public int GetNumberOfBillIsOrderedOnDay(DateTime date)
        {
            int result = 0;
            if (Bills.Any())
            {
                result = Bills.Where(b => b.BillStatusCode.Equals("PAY") &&
                (b.LastTimeChange >= date && b.LastTimeChange < date.AddDays(1))).Count();
            }
            return result;
        }

        public double GetRevenueOnDay(DateTime date)
        {
            double result = 0;
            if (Bills.Any())
            {
                 result = (from b in Bills
                               where b.BillStatusCode.Equals("PAY")
                               && b.LastTimeChange >= date
                               && b.LastTimeChange < date.AddDays(1)
                               select b.Total).Sum();
            }
            return result;
        }

        public double GetSubTotal(int idBill)
        {
            double result = 0;
            if (Bills.Any())
            {
                result = Bills.FirstOrDefault(b => b.BillID == idBill).SubTotal;
            }
            return result;
        }

        public double GetTax(int idBill)
        {
            double result = 0;
            if (Bills.Any())
            {
                result = Bills.FirstOrDefault(b => b.BillID == idBill).Tax;
            }
            return result;
        }

        public double GetTotal(int idBill)
        {
            double result = 0;
            if (Bills.Any())
            {
                result = Bills.FirstOrDefault(b => b.BillID == idBill).Total;
            }
            return result;
        }

        public Bill CreateBillForUser(string username)
        {
            Bill result = new Bill();
            result.Username = username;
            result.SubTotal = 0;
            result.Tax = 0;
            result.Total = 0;
            result.DateCreated = DateTime.Now;
            result.LastTimeChange = DateTime.Now;
            result.BillStatusCode = "NPA";
            return result;
        }

        public bool InsertBill(Bill b)
        {
            context.Bills.Add(b);
            context.SaveChanges();
            return true;
        }

        public bool UpdateBill(Bill b)
        {
            bool result = false;
            if (Bills.Any())
            {
                Bill bill = Bills.FirstOrDefault(bi => bi.BillID == b.BillID);
                if(bill != null)
                {
                    bill.SubTotal = b.SubTotal;
                    bill.Tax = b.Tax;
                    bill.Total = b.Total;
                    bill.LastTimeChange = b.LastTimeChange;
                    bill.BillStatusCode = b.BillStatusCode;
                    context.SaveChanges();
                    result = true;
                }
            }
            return result;
        }

        public Bill GetUsersLastBillIsNotPay(string username)
        {
            Bill result = null;
            if (Bills.Any())
            {
                result = Bills.FirstOrDefault(b => b.Username.Equals(username) && b.BillStatusCode.Equals("NPA"));
            }
            return result;
        }

        public Bill GetBillDetails(int IDBill)
        {
            Bill result = null;
            if (Bills.Any())
            {
                result = Bills.FirstOrDefault(b => b.BillID == IDBill);
            }
            return result;
        }

        public bool DeleteBill(int idBill)
        {
            bool result = false;
            Bill bi = Bills.FirstOrDefault(b => b.BillID == idBill);
            if (bi != null)
            {
                context.Bills.Remove(bi);
                result = true;
            }
            return result;
        }

        public bool UpdateBillStatus(int BillID, string BillStatusCode)
        {
            bool result = false;
            if (Bills.Any())
            {
                Bill bill = GetBillDetails(BillID);
                if (bill != null)
                {
                    bill.BillStatusCode = BillStatusCode;
                    context.SaveChanges();
                    result = true;
                }
            }
            return result;
        }

        public IEnumerable<int> GetListBillIdIsPayed()
        {
            return (from b in Bills
                    where b.BillStatusCode.Equals("PAY")
                    select b.BillID);
        }
    }
}
