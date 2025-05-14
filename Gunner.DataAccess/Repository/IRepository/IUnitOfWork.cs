using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gunner.DataAccess.Repository.IRepository
{
   public interface IUnitOfWork
    {


        ICategoryRepoistory CategoryReporistory { get; }
        IProductRepoistory ProductReporistory { get; }
        IshoppingCartRepoistory ShoppingCartReporistory { get; }
        IApplicationUserRepistory ApplicationUser { get; }

        IOrderDetailRepoistory orderDetail { get; }

        IOrderHeaderRepoistory orderHeader { get; }


        void Save();


    }
}
