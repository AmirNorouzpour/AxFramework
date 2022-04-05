using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AutoMapper;
using Entities.Tracking;
using WebFramework.Api;
using ValidationContext = System.ComponentModel.DataAnnotations.ValidationContext;

namespace API.Models.Tracking
{
    public class ProductInstanceDto : BaseDto<ProductInstanceDto, ProductInstance, int>, IValidatableObject
    {

        public string Code { get; set; }
        public string UserName { get; set; }
        public bool IsActive { get; set; }
        public bool IsMachine { get; set; }
        public int MachineId { get; set; }
        public int PersonnelId { get; set; }
        public int ProductLineId { get; set; }
        public bool IsEnter { get; set; }
        public DateTime InsertDateTime { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (string.IsNullOrWhiteSpace(Code))
                yield return new ValidationResult("کد نباید خالی باشد", new[] { nameof(Code) });
        }

        public override void CustomMappings(IMappingExpression<ProductInstance, ProductInstanceDto> mapping)
        {
            mapping.ForMember(
                dest => dest.UserName,
                config => config.MapFrom(src => $"{src.Personnel.User.FirstName} {src.Personnel.User.LastName}")
            );
        }
    }
}
