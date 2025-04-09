using AutoMapper;
using ITC.BusinessObject.Entities;
using ITC.Services.DTOs;
using ITC.Services.JobService;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ITC.API.Controllers
{
	[Route("api/job")]
    [ApiController]
    public class JobController : Controller
    {
		private readonly IJobService _jobService;
		private readonly IMapper _mapper;

		public JobController(IJobService jobService, IMapper mapper)
		{
			_jobService = jobService;
			_mapper = mapper;
		}


		/// <summary>
		/// Customer đăng bài / tạo job
		/// </summary>
		/// <param name="dto"></param>
		/// <returns></returns>
		[HttpPost]
		public async Task<IActionResult> Create([FromBody] JobCreateDto dto)
		{
			var job = _mapper.Map<Job>(dto);
			var created = await _jobService.CreateJobAsync(job);
			return CreatedAtAction(nameof(GetById), new { id = created.Id }, _mapper.Map<JobDto>(created));
		}


		/// <summary>
		/// Danh sách Job, có filter , có phân trang
		/// </summary>
		/// <param name="filter"></param>
		/// <returns></returns>
		[HttpGet]
		public async Task<IActionResult> GetAll([FromQuery] JobFilterDto filter)
		{
			var jobs = await _jobService.GetJobsFilteredAsync(filter);
			var result = _mapper.Map<IEnumerable<JobDto>>(jobs);
			return Ok(result);
		}


		/// <summary>
		/// Lấy 1 Job theo Id
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		[HttpGet("{id}")]
		public async Task<IActionResult> GetById(Guid id)
		{
			var job = await _jobService.GetJobByIdAsync(id);
			if (job == null) return NotFound();

			return Ok(_mapper.Map<JobDto>(job));
		}


		/// <summary>
		/// Update Job ( Role Customer)
		/// </summary>
		/// <param name="id"></param>
		/// <param name="job"></param>
		/// <returns></returns>
		[HttpPut("{id}")]
		[Authorize(Roles = "Customer")]
		public async Task<IActionResult> Update(Guid id, [FromBody] Job job)
		{
			if (id != job.Id)
				return BadRequest("ID không khớp");

			var existing = await _jobService.GetJobByIdAsync(id);
			if (existing == null)
				return NotFound();

			var updated = await _jobService.UpdateJobAsync(job);
			return Ok(updated);
		}


		/// <summary>
		/// Xóa 1 Job (Role Customer)
		/// </summary>
		/// <param name="id"></param>
		/// <returns></returns>
		[HttpDelete("{id}")]
		[Authorize(Roles = "Customer")]
		public async Task<IActionResult> Delete(Guid id)
		{
			var success = await _jobService.DeleteJobAsync(id);
			if (!success)
				return NotFound();

			return Ok("Delete Successful");
		}
	}
}
