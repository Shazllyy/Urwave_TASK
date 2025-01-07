using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data_Access_Layer.Entities
{
    public class ActivityLog
    {
        public Guid Id { get; set; }
        public string? ActivityType { get; set; }
        public string? Description { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
