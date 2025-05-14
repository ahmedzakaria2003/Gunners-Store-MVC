using Gunner.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Gunner.DataAccess.Repository.IRepository
{
    public interface ICategoryRepoistory : IRepository<Category>
    {

        void Update(Category obj);

        void Save();


    }
}
