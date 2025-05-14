using Gunner.Models.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gunner.Models.ViewModels
{
    public class OrderVM
    {

        public OrderHeader orderHeader { get; set; }    

        public IEnumerable<OrderDetail> OrderDetails { get; set; }      


    }
}
