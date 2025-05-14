using Gunner.DataAccess.Data;
using Gunner.DataAccess.Repository.IRepository;
using Gunner.Models;
using Gunner.Models.Models;

namespace Gunner.DataAccess.Repository
{
    public class ApplicationUserRepoistory : Repository<ApplicationUser>, IApplicationUserRepistory
    {
        private readonly ApplicationDbContext _db;
        public ApplicationUserRepoistory(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

    }


}