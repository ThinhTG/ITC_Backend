using ITC.BusinessObject.Entities;
using ITC.Core.Base;
using ITC.Repositories.Interface;
using ITC.Repositories.Repository;
using ITC.Services.DTOs;
using ITC.Services.DTOs.Job;
using Microsoft.Extensions.Options;





namespace ITC.Services.JobService
{
	public class JobService : IJobService
	{
		private readonly IJobRepository _jobRepo;
		private readonly UploadSettings _uploadSettings;

		public JobService(IJobRepository jobRepo, IOptions<UploadSettings> uploadSettings)
		{
			_jobRepo = jobRepo;
			_uploadSettings = uploadSettings.Value;
		}


		public async Task<bool> PostJobAsync(CreateJobRequest jobDto, Guid customerId)
		{
			try
			{
				// Kiểm tra file upload
				if (jobDto.CompanyPdf == null || jobDto.CompanyPdf.Length == 0)
				{
					throw new Exception("No file uploaded.");
				}

				// Kiểm tra loại file
				var fileExtension = Path.GetExtension(jobDto.CompanyPdf.FileName).ToLower();
				if (fileExtension != ".docx" && fileExtension != ".pdf")
				{
					throw new Exception("Only DOCX and PDF files are allowed.");
				}

				// Tạo thư mục lưu file nếu chưa tồn tại
				var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "Uploads");
				if (!Directory.Exists(uploadsFolder))
				{
					Directory.CreateDirectory(uploadsFolder);
				}

				// Lưu file
				var fileName = Guid.NewGuid().ToString() + fileExtension;
				var filePath = Path.Combine(uploadsFolder, fileName);
				using (var stream = new FileStream(filePath, FileMode.Create))
				{
					await jobDto.CompanyPdf.CopyToAsync(stream);
				}

				// Tạo đối tượng Job
				var job = new Job
				{
					Id = Guid.NewGuid(),
					CustomerId = customerId, // Lấy từ người dùng đang đăng nhập
					FullName = jobDto.FullName,
					Title = jobDto.Title,
					WorkType = jobDto.WorkType,
					TranslationLanguage = jobDto.TranslationLanguage,
					ExperienceRequirement = jobDto.ExperienceRequirement,
					Education = jobDto.Education,
					TranslationForm = jobDto.TranslationForm,
					JobDescription = jobDto.JobDescription,
					RelevantCertificates = jobDto.RelevantCertificates,
					ContactEmail = jobDto.ContactEmail,
					ContactPhone = jobDto.ContactPhone,
					WorkLocation = jobDto.WorkLocation,
					SalaryType = jobDto.SalaryType,
					SalaryAmount = jobDto.SalaryAmount,
					CompanyPdfPath = filePath,
					Status = 0,
					CreatedAt = DateTime.UtcNow
				};

				// Lưu vào database qua repository
				await _jobRepo.AddAsync(job);
				await _jobRepo.SaveChangesAsync();

				return true;
			}
			catch
			{
				throw;
			}
		}

		public async Task<List<JobResponseDto>> GetAllAvailableJobsAsync()
		{
			var jobs = await _jobRepo.GetAllAsync(); // hoặc dùng predicate: x => x.Status == 0

			return jobs
				.Where(j => j.Status == 0)
				.OrderByDescending(j => j.CreatedAt)
				.Select(j => new JobResponseDto
				{
					Id = j.Id,
					FullName = j.FullName,
					Title = j.Title,
					WorkType = j.WorkType,
					TranslationLanguage = j.TranslationLanguage,
					JobDescription = j.JobDescription,
					WorkLocation = j.WorkLocation,
					SalaryType = j.SalaryType,
					SalaryAmount = j.SalaryAmount,
					CreatedAt = j.CreatedAt
				})
				.ToList();
		}

		public async Task<List<Job>> GetJobsByCustomerIdAsync(Guid customerId)
		{
			return await _jobRepo.GetJobsByCustomerIdAsync(customerId);
		}
	}
}

