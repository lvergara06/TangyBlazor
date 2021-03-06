using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tangy_DataAccess
{
    public class OrderHdr
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string UserId { get; set; }
        // add Navigation property : #TODO
        [Required]
        [Display(Name = "Order Total")]
        public double OrderTotal { get; set; }
        [Required]
        public DateTime OrderDate { get; set; }
        [Required]
        [Display(Name = "Shipping Date")]
        public DateTime ShippingDate { get; set; }
        public string Status { get; set; }
        public string? SessionId { get; set; }
        public string? PaymentIntentId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string StreetAddress { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string PostalCode { get; set; }
		public string Tracking { get; set; }
		public string Carrier { get; set; }
	}
}
