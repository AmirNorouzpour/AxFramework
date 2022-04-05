using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Entities.Tracking;
using WebFramework.Api;
using ValidationContext = System.ComponentModel.DataAnnotations.ValidationContext;

namespace API.Models.Tracking
{
    public class ProductInstanceHistoryDto : BaseDto<ProductInstanceHistoryDto, ProductInstanceHistory, int>, IValidatableObject
    {
        public int ProductInstanceId { get; set; }
        public string ProductInstanceCode { get; set; }
        public int UserId { get; set; }
        public string Username { get; set; }
        public int PersonnelId { get; set; }
        public string PersonnelName { get; set; }
        public string OpName { get; set; }
        public int Day { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public int ShiftId { get; set; }
        public string ShiftName { get; set; }
        public string EnterType { get; set; }
        public DateTime EnterTime { get; set; }
        public DateTime? ExitTime { get; set; }
        public string Code { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            throw new NotImplementedException();
        }
        public override void CustomMappings(IMappingExpression<ProductInstanceHistory, ProductInstanceHistoryDto> mapping)
        {
            mapping.ForMember(
                dest => dest.ProductInstanceCode,
                config => config.MapFrom(src => $"{src.ProductInstance.Code} ")
            );
            mapping.ForMember(
                dest => dest.Username,
                config => config.MapFrom(src => $"{src.User.UserName} ")
            );
            mapping.ForMember(
                dest => dest.Code,
                config => config.MapFrom(src => src.ProductInstance.Code)
            );
            mapping.ForMember(
                dest => dest.PersonnelName,
                config => config.MapFrom(src => $"{src.User.FirstName} {src.User.LastName}")
            );
            mapping.ForMember(
                dest => dest.OpName,
                config => config.MapFrom(src => $"{src.Machine.OperationStation.Name} ")
            );
            mapping.ForMember(
                dest => dest.ShiftName,
                config => config.MapFrom(src => $"{src.Shift.Name} ")
            );
        }

    }
}
