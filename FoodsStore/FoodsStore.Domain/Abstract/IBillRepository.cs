using FoodsStore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodsStore.Domain.Abstract
{
    public interface IBillRepository
    {
        IEnumerable<Bill> Bills { get; }

        Bill GetBillDetails(int IDBill);

        Bill CreateBillForUser(string username);

        Bill GetUsersLastBillIsNotPay(string username);

        bool InsertBill(Bill b);

        bool UpdateBill(Bill b);

        bool UpdateBillStatus(int BillID, string BillStatusCode);

        bool DeleteBill(int idBill);

        double GetTotal(int idBill);

        double GetTax(int idBill);

        double GetSubTotal(int idBill);

        int GetNumberOfBillIsOrdered();

        int GetNumberOfBillIsOrderedOnDay(DateTime date);

        double GetRevenueOnDay(DateTime date);

        IEnumerable<int> GetListBillIdIsPayed();

        IEnumerable<Bill> GetListBillOfUser(string username);
    }
}
