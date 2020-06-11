using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using Fs.Core.Contracts;
using Fs.Data.Models;

namespace Fs.Business.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            AllowNullCollections = true;
            CreateMap<ErrorLog, ErrorLogDto>();
            CreateMap<ErrorLogDto, ErrorLog>();
            CreateMap<Order, OrderDto>();
            CreateMap<OrderDto, Order>();
            CreateMap<Report, ReportDto>();
            CreateMap<ReportDto, Report>();
            //CreateMap<IEnumerable<Order>, IEnumerable<OrderDto>();

        }
    }
}
