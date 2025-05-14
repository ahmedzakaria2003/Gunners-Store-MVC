using Gunner.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Gunner.DataAccess.Repository.IRepository
{
    public interface IOrderHeaderRepoistory : IRepository<OrderHeader>
    {

        void Update(OrderHeader obj);
        void UpdateStatus(int id, string orderStatus, string? paymentStatus = null);
        void UpdateStripePaymentId(int id, string sessionId, string paymentIntentId);
        int Count(Expression<Func<OrderHeader, bool>> filter = null);
        Task<int> CountAsync(Expression<Func<OrderHeader, bool>> filter = null);

        void Save();
    }
}
