using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITC.Core.Enum
{
	/// <summary>
	/// Trạng thái tổng thể của Job (không liên quan đến thanh toán từng BPDV)
	/// </summary>
	public enum JobStatus
	{
		Open = 0,             // Mới tạo, chưa có ai apply
		Recruiting = 1,       // Đang tuyển BPDV (có người apply)
		InProgress = 2,       // Có ít nhất 1 BPDV đang làm việc
		PartiallyCompleted = 3, // Một số BPDV đã hoàn thành
		Completed = 4,        // Tất cả BPDV đã hoàn thành
		Canceled = 5,         // Job bị hủy
		FullyRecruited = 6    // Đã đủ số lượng BPDV cần tuyển
	}

	/// <summary>
	/// Trạng thái công việc của từng BPDV trong một job
	/// </summary>
	public enum InterpreterWorkStatus
	{
		NotStarted = 0,       // Chưa bắt đầu (mới được chọn)
		AwaitingPayment = 1,  // Chờ thanh toán cho BPDV này
		Paid = 2,             // Đã thanh toán cho BPDV này
		InProgress = 3,       // BPDV này đang làm việc
		Submitted = 4,        // BPDV này đã nộp kết quả
		Completed = 5         // BPDV này đã hoàn thành
	}

}
