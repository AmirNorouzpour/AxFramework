using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AutoMapper;
using Entities.Framework;
using WebFramework.Api;
using ValidationContext = System.ComponentModel.DataAnnotations.ValidationContext;
using System.Linq;

namespace API.Models
{
    public class GeoDto : BaseDto<GeoDto, Geo, int>, IValidatableObject
    {
        public int? ParentId { get; set; }
        public string Title { get; set; }
        public string Path { get; set; }
        public ICollection<GeoDto> Children { get; set; }
        public bool HasChildren { get; set; }

        public override void CustomMappings(IMappingExpression<Geo, GeoDto> mapping)
        {
            mapping.ForMember(
                dest => dest.Children,
                config => config.MapFrom(src => src.Children)
            );
            mapping.ForMember(
                dest => dest.HasChildren,
                config => config.MapFrom(src => src.Children.Any(x => x.ParentId == src.Id))
            );
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (!string.IsNullOrWhiteSpace(Title))
                yield return new ValidationResult("عنوان موقعیت را وارد کنید", new[] { nameof(Title) });
        }
    }
}
