using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using AutoMapper;
using Grillber.C2.Controllers;

namespace Grillber.C2.App_Start
{
    public static class MappingRegistrations
    {
        public static IMapper mapper { get; set; }

        public static void BuildRegistrations()
        {
            var mapperConfiguration = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<FullTask, TaskOutV1>();
                cfg.CreateMap<FullTask, TaskOutV2>();

            });

            mapper = mapperConfiguration.CreateMapper();
        }
    }
}