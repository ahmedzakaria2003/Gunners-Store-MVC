using Gunner.DataAccess.Data;
using Gunner.DataAccess.Repository.IRepository;
using Gunner.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gunner.DataAccess.Repository
{
    public class OrderDetailRepoistory : Repository<OrderDetail>, IOrderDetailRepoistory
    {
        private readonly ApplicationDbContext _db;
        public OrderDetailRepoistory(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Save()
        {
            _db.SaveChanges();
        }

        public void Update(OrderDetail obj)
        {

            _db.orderDetails.Update(obj);

        }




    }
}
