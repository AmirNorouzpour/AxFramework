using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Entities.Tracking;
using WebFramework.Api;

namespace API.Models.Tracking
{
    public class StopDto : BaseDto<StopDto, Stop, int>
    {
        public string MachineName { get; set; }
        public string MachineCode { get; set; }
        public string Code { get; set; }
        public string LastStatus { get; set; }
        public bool Description { get; set; }
        public string InsertDateTime { get; set; }
        ICollection<StopDetail> StopDetails { get; set; }
        public override void CustomMappings(IMappingExpression<Stop, StopDto> mapping)
        {
            mapping.ForMember(
                dest => dest.MachineName,
                config => config.MapFrom(src => $"{src.Machine.Name} ")
            );
            mapping.ForMember(
                dest => dest.LastStatus,
                config => config.MapFrom((stop, dto) => dto.StopDetails.OrderByDescending(x => x.InsertDateTime).FirstOrDefault()!.StopDetailType)
            );
        }
    }
}
