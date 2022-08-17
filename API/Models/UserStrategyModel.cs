using System;
using AutoMapper;
using Entities.Framework;
using WebFramework.Api;

namespace API.Models
{
    public class UserStrategyDto : BaseDto<UserStrategyDto, UserStrategy, int>
    {
        public string Name { get; set; }
        public string Data { get; set; }
        public int Version { get; set; }
        public string User { get; set; }
        public DateTime DateTime { get; set; }
        public string Unique { get; set; }


        public override void CustomMappings(IMappingExpression<UserStrategy, UserStrategyDto> mapping)
        {
            mapping.ForMember(
                dest => dest.User,
                config => config.MapFrom(src => src.User.FirstName + " " + src.User.LastName)
            );
            mapping.ForMember(
                dest => dest.DateTime,
                config => config.MapFrom(src => src.ModifiedDateTime.HasValue ? src.ModifiedDateTime : src.InsertDateTime)
            );
        }
    }

}
