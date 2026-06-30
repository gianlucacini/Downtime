using System;

namespace Downtime.Common.DTOs
{
    public class StatusResponseDto
    {
        public int Status { get; set; }
        public Double? Interval { get; set; }
        public DateTime TimeStamp { get; set; }
    }
}
