using AutoMapper;
using PublicAPI.Responses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebApplication.Migrations;
using WebApplication.Models.HospitalModels;

namespace WebApplication.Models.Automapper
{
    public class GeneralProfile : Profile
    {
        public GeneralProfile()
        {
            CreateMap<Hospital, HospitalView>();
            CreateMap<MrRecord, RecordView>();
        }
    }
}
