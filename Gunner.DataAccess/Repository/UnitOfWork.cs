using Gunner.DataAccess.Data;
using Gunner.DataAccess.Repository.IRepository;
using Gunner.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gunner.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _db;
        public ICategoryRepoistory CategoryReporistory { get; private set; }

        public IProductRepoistory ProductReporistory { get; private set; }


        public IApplicationUserRepistory ApplicationUser { get; private set; }

        public IshoppingCartRepoistory ShoppingCartReporistory { get; private set; }

        public IOrderDetailRepoistory orderDetail { get; private set; }

        public IOrderHeaderRepoistory orderHeader { get; private set; }

        public UnitOfWork(ApplicationDbContext db) 
        {
            _db = db;
            CategoryReporistory = new CategoryRepoistory(_db);
            ProductReporistory = new ProductRepoistory(_db);
            ShoppingCartReporistory = new shoppingCartRepoistory(_db);
            ApplicationUser = new ApplicationUserRepoistory(_db);
            orderDetail = new OrderDetailRepoistory(_db);
            orderHeader = new OrderHeaderRepoistory(_db);

        }



        public void Save()
        {
            _db.SaveChanges();

        }
    }
}
