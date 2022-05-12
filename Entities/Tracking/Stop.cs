using System.ComponentModel.DataAnnotations.Schema;
using Entities.Framework;

namespace Entities.Tracking
{
    public class Stop : BaseEntity
    {
        public int MachineId { get; set; }
        [ForeignKey("MachineId")]
        public Machine Machine { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }

    }

    public class StopDetail : BaseEntity
    {
        public int StopId { get; set; }
        public string Description { get; set; }
        public StopDetailType StopDetailType { get; set; }
        public int PersonnelId { get; set; }
        [ForeignKey("PersonnelId")]
        public Personnel Personnel { get; set; }

    }

    public enum StopDetailType
    {
        BeginStop = 1,
        BeginRepair = 2,
        EndRepair = 3,
        EndStop = 4
    }
}
