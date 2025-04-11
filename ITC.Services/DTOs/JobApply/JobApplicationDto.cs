using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITC.Services.DTOs.JobApply
{
	public class JobApplicationDto
	{
		public Guid Id { get; set; }
		public string Message { get; set; }
		public DateTime CreatedAt { get; set; }
		public int Status { get; set; }

		public Guid JobId { get; set; }
		public string JobTitle { get; set; }

		public Guid InterpreterId { get; set; }
		public string InterpreterName { get; set; }
	}
}
