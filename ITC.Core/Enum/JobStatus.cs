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
		Completed = 4,        // Đã hoàn thành
		Canceled = 5          // Bị huỷ
	}

}
