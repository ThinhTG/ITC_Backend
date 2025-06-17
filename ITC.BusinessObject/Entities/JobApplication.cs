using ITC.BusinessObject.Identity;
using ITC.Core.Enum;
using ITC.Core.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITC.BusinessObject.Entities
{
	public class JobApplication
	{
		public Guid Id { get; set; } = Guid.NewGuid();
		public Guid JobId { get; set; }
		public Job? Job { get; set; }

		public Guid InterpreterId { get; set; }
		public ApplicationUser? Interpreter { get; set; }

		public string Message { get; set; } = string.Empty;
		public DateTimeOffset CreatedAt { get; set; } = CoreHelper.SystemTimeNow;
		public DateTimeOffset LastUpdatedAt { get; set; } = CoreHelper.SystemTimeNow;
		
		// Trạng thái ứng tuyển
		public string ApplicationStatus { get; set; } = "0"; // 0: pending, 1: accepted, 2: rejected
		
		// Trạng thái công việc của BPDV này
		public int WorkStatus { get; set; } = (int)InterpreterWorkStatus.NotStarted; // 0: NotStarted, 1: AwaitingPayment, 2: Paid, 3: InProgress, 4: Submitted, 5: Completed
		
		// Thông tin thanh toán cho BPDV này
		public bool IsPaid { get; set; } = false;
		public decimal? IndividualFee { get; set; } // Phí riêng cho BPDV này
		public DateTimeOffset? PaidAt { get; set; }
		
		// Thông tin công việc của BPDV này
		public string? IndividualResultFileUrl { get; set; } // File kết quả riêng của BPDV này
		public DateTimeOffset? StartedAt { get; set; }
		public DateTimeOffset? CompletedAt { get; set; }
		public int? CompletionOffsetMinutes { get; set; } // Độ trễ hoàn thành của BPDV này
	}

}
