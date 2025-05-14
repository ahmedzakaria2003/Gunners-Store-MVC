using Gunner.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gunner.DataAccess.Repository.IRepository
{
    public interface IOrderDetailRepoistory : IRepository<OrderDetail>
    {

        void Update(OrderDetail obj);

        void Save();
    }
}
