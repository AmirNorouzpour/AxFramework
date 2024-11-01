﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using AutoMapper;
using Entities.Framework;
using WebFramework.Api;
using ValidationContext = System.ComponentModel.DataAnnotations.ValidationContext;

namespace API.Models
{
    public class UserDto : BaseDto<UserDto, User, int>, IValidatableObject
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string RePassword { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public string LastLoginDate { get; set; }
        public bool IsActive { get; set; }
        public GenderType GenderType { get; set; }
        public DateTime? BirthDate { get; set; }
        public DateTime? ExpireDateTime { get; set; }
        public bool OutOfOrgAccess { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (UserName != null && UserName.Equals("test", StringComparison.OrdinalIgnoreCase))
                yield return new ValidationResult("نام کاربری نمیتواند Test باشد", new[] { nameof(UserName) });
            if (Password != null && Password.Equals("123456"))
                yield return new ValidationResult("رمز عبور نمیتواند 123456 باشد", new[] { nameof(Password) });
        }

        public override void CustomMappings(IMappingExpression<User, UserDto> mapping)
        {
            mapping.ForMember(
                dest => dest.FullName,
                config => config.MapFrom(src => $"{src.FirstName} {src.LastName}")
            );
        }
    }
}
