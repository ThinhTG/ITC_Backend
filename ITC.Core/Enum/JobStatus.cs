using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITC.Core.Enum
{
	public enum JobStatus
	{
		Open = 0,             // Mới tạo, chưa có ai apply
		AwaitingPayment = 1,  // Khách đã chọn interpreter, chờ thanh toán
		Paid = 2,             // Khách đã thanh toán
		InProgress = 3,       // Interpreter đang làm việc
		Submitted = 4,        // BPDV đã nộp kết quả – chờ khách xác nhận
		Completed = 5,        // Đã hoàn thành
		Canceled = 6,         // Bị huỷ
		FullyRecruited = 7    // Đã đủ số lượng người cần tuyển
	}

}
