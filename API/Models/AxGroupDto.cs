using System.Collections.Generic;
using AutoMapper;
using Entities.Framework;
using WebFramework.Api;

namespace API.Models
{
    public class AxGroupDto : BaseDto<AxGroupDto, AxGroup, int>
    {
        public string GroupName { get; set; }
        public string Description { get; set; }
        public int UsersCount { get; set; }
        public int GroupUsersId { get; set; }


        public override void CustomMappings(IMappingExpression<AxGroup, AxGroupDto> mapping)
        {
            mapping.ForMember(
                dest => dest.UsersCount,
                config => config.MapFrom(src => src.GroupUsers.Count)
            );
        }
    }

    public class AxGroupUsersDto : BaseDto<AxGroupUsersDto, UserGroup, int>
    {
        public List<int> UserIds { get; set; }
        public int GroupId { get; set; }
    }

}
