using System;
using System.ComponentModel.DataAnnotations.Schema;
using Entities.Framework;
using Entities.MasterSignal;

namespace API.Models
{
    public class AnalysisMsgDto
    {
        public string Content { get; set; }
        public string Title { get; set; }
        public string Tags { get; set; }
        public string Side { get; set; }
        public int Views { get; set; }
        public int? UserId { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; }
        public string DateTime { get; set; }
        public string Type { get; set; }
        public int Id { get; set; }
        public string ImageLink { get; set; }
    }
}
