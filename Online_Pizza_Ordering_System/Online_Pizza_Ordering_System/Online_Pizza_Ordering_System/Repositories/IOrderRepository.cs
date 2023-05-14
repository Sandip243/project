using Online_Pizza_Ordering_System.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Online_Pizza_Ordering_System.Repositories
{
    public interface IOrderRepository
    {
        Task UpdateCardAsync(CardPayment cardpayment);
        Task CreateCardAsync(CardPayment cardpayment);

        CardPayment GetById(Guid? id);

        Task<CardPayment> GetByIdAsync(Guid? id);

        void Update(CardPayment card);
        void Remove(CardPayment card);

        void SaveChanges();
        Task SaveChangesAsync();

    }
}
