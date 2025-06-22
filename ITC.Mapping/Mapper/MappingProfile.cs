using AutoMapper;
using ITC.BusinessObject.Entities;
using ITC.BusinessObject.Identity;
using ITC.BusinessObject.Response;
using ITC.Core;
using ITC.Core.Contracts;
using ITC.Repositories.PaggingItems;
using ITC.Services.DTOs;
using ITC.Services.DTOs.subplan;

namespace ITC.Mapping.Mapper
{
	public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<ApplicationUser, UserResponse>();
			CreateMap<Job, JobDTO>().ReverseMap();
			CreateMap<CreateJobRequest, Job>();
			//	CreateMap<List<Job>, List<JobDTO>>();
			//	CreateMap<PaginatedList<JobDTO>, PaginatedList<Job>>().ReverseMap();
			CreateMap<TranslatorCertificate, TranslatorCertificateDto>().ReverseMap();
			CreateMap<TranslatorCertificateCreateUpdateDto, TranslatorCertificate>();

			CreateMap<SubscriptionPlan, SubscriptionPlanDto>().ReverseMap();
		}
    }
}
