using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using FluentValidation;

namespace Entities.Framework
{
    public class UserMessage : BaseEntity
    {
        public string Title { get; set; }
        public string Body { get; set; }
        public string Number { get; set; }
        public string Number2 { get; set; }
        public bool HaveAttachment { get; set; }
        public int SenderId { get; set; }
        [ForeignKey("SenderId")]
        public User Sender { get; set; }
        public ICollection<UserMessageReceiver> Receivers { get; set; }
        public UserMessagePriority Priority { get; set; }
        public UserMessageConfidentiality Confidentiality { get; set; }
    }

    public class UserMessageReceiver : BaseEntity
    {
        public string Title { get; set; }
        public ReceiverType ReceiverType { get; set; }
        public int PrimaryKey { get; set; }
        public int UserMessageId { get; set; }
        [ForeignKey("UserMessageId")]
        public UserMessage UserMessage { get; set; }
        public bool IsSeen { get; set; }
    }

    public enum ReceiverType
    {
        User,
        Group,
        Post
    }

    public enum UserMessagePriority
    {
        Low,
        Medium,
        High,
        VeryHigh
    }

    public enum UserMessageConfidentiality
    {
        Normal,
        Confidential,
        Secret,
        CompletelySecret
    }

    public class UserMessageValidator : AbstractValidator<UserMessage> { }
    public class UserMessageReceiverValidator : AbstractValidator<UserMessageReceiver> { }
}
