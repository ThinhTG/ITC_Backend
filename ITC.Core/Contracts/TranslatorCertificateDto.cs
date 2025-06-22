using ITC.Core.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITC.Core.Contracts
{
	public class TranslatorCertificateDto : TranslatorCertificateCreateUpdateDto
	{
		public Guid Id { get; set; }
		public Guid ApplicationUserId { get; set; }

		public CertificateStatus Status { get; set; }
		public string? RejectReason { get; set; }
		public DateTimeOffset? ApprovedAt { get; set; }
		public Guid? ApprovedBy { get; set; }
	}

}
