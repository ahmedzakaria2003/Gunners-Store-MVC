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
   public class ProductRepoistory : Repository<Product>, IProductRepoistory
    {

        private readonly ApplicationDbContext _db;

        public ProductRepoistory(ApplicationDbContext db) : base(db)
        {
            _db = db;   
        }

        public void Update(Product obj)
        {
          var objFromDb = _db.products.FirstOrDefault(u => u.ProductID == obj.ProductID);
            if (objFromDb != null)
            {
                objFromDb.ProductName = obj.ProductName;
                objFromDb.Description = obj.Description;
                objFromDb.Brand = obj.Brand;
                objFromDb.ListPrice = obj.ListPrice;
                objFromDb.Price = obj.Price;
                objFromDb.Price50 = obj.Price50;
                objFromDb.Price100 = obj.Price100;
                if (obj.ImgUrl != null)
                {
                    objFromDb.ImgUrl = obj.ImgUrl;
                }
            }




        }

        public void Save()
        {
            _db.SaveChanges();
        }


    }
}
