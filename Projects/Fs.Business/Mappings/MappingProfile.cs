using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using AngularPOC.Core.Contracts;
using AngularPOC.Data.Models;

namespace AngularPOC.Business.Mappings
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
