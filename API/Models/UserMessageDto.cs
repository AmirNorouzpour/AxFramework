using System;
using AutoMapper;
using Common.Utilities;
using Entities.Framework;
using WebFramework.Api;

namespace API.Models
{
    public class UserMessageDto : BaseDto<UserMessageDto, UserMessage, int>
    {
        public string Title { get; set; }
        public string SenderName { get; set; }
        public DateTime SentDate { get; set; }
        public bool HaveAttachment { get; set; }

        public override void CustomMappings(IMappingExpression<UserMessage, UserMessageDto> mapping)
        {
            mapping.ForMember(
                dest => dest.SenderName,
                config => config.MapFrom(src => src.Sender.FirstName + " " + src.Sender.LastName)
            );

            mapping.ForMember(
                dest => dest.SentDate,
                config => config.MapFrom(src => src.InsertDateTime)
            );
        }
    }
}
