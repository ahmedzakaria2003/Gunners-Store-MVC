using Gunner.DataAccess.Data;
using Gunner.DataAccess.Repository.IRepository;
using Gunner.Models.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Gunner.DataAccess.Repository
{
    public class OrderHeaderRepoistory : Repository<OrderHeader>, IOrderHeaderRepoistory
    {
        private readonly ApplicationDbContext _db;
        public OrderHeaderRepoistory(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Save()
        {
            _db.SaveChanges();
        }

        public void Update(OrderHeader obj)
        {

            _db.OrderHeaders.Update(obj);

        }

        public void UpdateStatus(int id, string orderStatus, string? paymentStatus = null)
        {

            var orderFromDb = _db.OrderHeaders.FirstOrDefault(u => u.Id == id);
            if (orderFromDb != null)
            {
                orderFromDb.OrderStatus = orderStatus;
                if (paymentStatus != null)
                {
                    orderFromDb.PaymentStatus = paymentStatus;
                }
            }
        }

        public void UpdateStripePaymentId(int id, string sessionId, string paymentIntentId)
        {

            var orderFromDb = _db.OrderHeaders.FirstOrDefault(u => u.Id == id);
            if (!string.IsNullOrEmpty(sessionId))
            {
                orderFromDb.SessionId = sessionId;
            }
            if (!string.IsNullOrEmpty(paymentIntentId))
            {
                orderFromDb.PaymentIntentId = paymentIntentId;
                orderFromDb.PaymentDate = DateTime.Now;
            }
        }


        public int Count(Expression<Func<OrderHeader, bool>> filter = null)
        {
            IQueryable<OrderHeader> query = _db.OrderHeaders;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            return query.Count();
        }

        public async Task<int> CountAsync(Expression<Func<OrderHeader, bool>> filter = null)
        {
            IQueryable<OrderHeader> query = _db.OrderHeaders;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            return await query.CountAsync();
        }
    }
}
