using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITC.Services.DTOs.JobApply
{
	public class SelectInterRequest
	{
		public Guid JobId { get; set; }
		public Guid InterpreterId { get; set; }
	}
}
