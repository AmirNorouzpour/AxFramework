﻿using System.ComponentModel.DataAnnotations.Schema;

namespace Entities.Framework
{
    public class UserSetting : BaseEntity
    {
        public string Theme { get; set; }
        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; }
        public int? DefaultSystemId { get; set; }
        [ForeignKey("DefaultSystemId")]
        public Menu DefaultSystem { get; set; }
    }
}
