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
    public class CategoryRepoistory : Repository<Category>, ICategoryRepoistory
    {
        private readonly ApplicationDbContext _db;
        public CategoryRepoistory(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Save()
        {
            _db.SaveChanges();
        }

        public void Update(Category obj)
        {

            _db.categories.Update(obj); 

        }

     

       
    }
}
