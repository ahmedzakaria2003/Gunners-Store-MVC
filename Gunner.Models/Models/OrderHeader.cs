using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gunner.Models.Models
{
    public class OrderHeader
    {
        public int Id { get; set; }

        public string ApplicationUserId { get; set; }

        [ValidateNever]
        [ForeignKey("ApplicationUserId")]
        public ApplicationUser ApplicationUser { get; set; }

        public DateTime OrderDate { get; set; }


        public DateTime ShippingDate { get; set; }

        public string? OrderStatus { get; set; }


        public string? PaymentStatus { get; set; }

        public string? TrackingNumber { get; set; }

        public string? Carrier { get; set; }


        public DateTime PaymentDate { get; set; }

        public string? SessionId { get; set; }
        public string? PaymentIntentId { get; set; }

        [Required]
        public string Name { get; set; }
        [Required]

        public string StreetAddress { get; set; }
        [Required]

        public string City { get; set; }
        [Required]

        public string State { get; set; }
        [Required]

        public string PostalCode { get; set; }

        [Required]
        public string PhoneNumber { get; set; }
        [ValidateNever]
        public ICollection<OrderDetail> OrderDetails { get; set; }

        [NotMapped]
        public double OrderTotal => OrderDetails?.Sum(od => od.Price * od.Count) ?? 0;


    }
}
