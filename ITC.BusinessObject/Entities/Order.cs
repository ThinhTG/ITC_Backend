using ITC.BusinessObject.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITC.BusinessObject.Entities
{
		public class Order
		{
		[Key]
		[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
		public int OrderId { get; set; }

		public Guid JobId { get; set; }
			[ForeignKey(nameof(JobId))]
			public Job Job { get; set; }

			public Guid CustomerId { get; set; }
			[ForeignKey(nameof(CustomerId))]
			public ApplicationUser Customer { get; set; }

			public Guid InterpreterId { get; set; }
			[ForeignKey(nameof(InterpreterId))]
			public ApplicationUser Interpreter { get; set; }

			public DateTime StartTime { get; set; }
			public DateTime EndTime { get; set; }

			// 📌 Giá phiên dịch viên đưa ra (khách hàng phải trả)
			public decimal ServicePrice { get; set; }

			// 📌 Phí nền tảng ITC thu (có thể cài mặc định là 10%)
			public decimal PlatformFee { get; set; }

			// 📌 Tổng tiền khách hàng phải trả = ServicePrice + PlatformFee
			public decimal TotalPrice { get; set; }

			public string Status { get; set; } // Pending, Accepted, Completed, Cancelled

			public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
			public DateTime? UpdatedAt { get; set; }

		    public int? OrderCode { get; set; }

		   [Required]
		   public bool PaymentConfirmed { get; set; }

	}


}
