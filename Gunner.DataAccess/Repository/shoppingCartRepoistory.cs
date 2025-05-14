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
    public class shoppingCartRepoistory : Repository<shoppinCart>, IshoppingCartRepoistory
    {
        private readonly ApplicationDbContext _db;
        public shoppingCartRepoistory(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }
        public void Update(shoppinCart obj)
        {
             _db.shoppingCarts.Update(obj);
        }
        public void Save()
        {
            _db.SaveChanges();
        }
    
    }
}
