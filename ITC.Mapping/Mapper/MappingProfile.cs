﻿using AutoMapper;
using ITC.BusinessObject.Entities;
using ITC.BusinessObject.Identity;
using ITC.BusinessObject.Response;
using ITC.Core.Contracts;
using ITC.Services.DTOs;

namespace ITC.Mapping.Mapper
{
	public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<ApplicationUser, UserResponse>();
			CreateMap<Job, JobDTO>();
			CreateMap<CreateJobRequest, Job>();
		}
    }
}
