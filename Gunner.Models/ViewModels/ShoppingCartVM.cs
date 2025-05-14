using Gunner.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gunner.Models.ViewModels
{
    public class ShoppingCartVM
    {
        public IEnumerable<shoppinCart> shoppingCartList { get; set; }

        public OrderHeader orderHeader { get; set; }
        public double OrderTotal { get; set; }

    }
}
