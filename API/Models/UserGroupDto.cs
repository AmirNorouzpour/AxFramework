using System;

namespace API.Models
{
    public class UserGroupDto
    {
        public string Name { get; set; }
        public int Id { get; set; }
        public string GroupLabel { get; set; }
        public UgType Type { get; set; }
        public DateTime InsertDateTime { get; set; }
    }

    public enum UgType
    {
        User = 1,
        Group = 2
    }
}
