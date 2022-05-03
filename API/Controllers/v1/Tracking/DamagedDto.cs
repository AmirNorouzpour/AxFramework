using System;
using System.ComponentModel.DataAnnotations.Schema;
using AutoMapper;
using Entities.Tracking;
using WebFramework.Api;

namespace API.Controllers.v1.Tracking
{
    public class DamagedDto : BaseDto<DamagedDto, Damaged, int>
    {
        public int ProductInstanceId { get; set; }
        [ForeignKey("ProductInstanceId")]
        public ProductInstance ProductInstance { get; set; }
        public int PersonnelId { get; set; }
        [ForeignKey("PersonnelId")]
        public Personnel Personnel { get; set; }
        public DateTime DateTime { get; set; }
        public string UserName { get; set; }
        public string Code { get; set; }

        public override void CustomMappings(IMappingExpression<Damaged, DamagedDto> mapping)
        {
            mapping.ForMember(
                dest => dest.UserName,
                config => config.MapFrom(src => $"{src.Personnel.User.FirstName} {src.Personnel.User.LastName}")
            );

            mapping.ForMember(
                dest => dest.Code,
                config => config.MapFrom(src => $"{src.ProductInstance.Code}")
            );
        }
    }
}
