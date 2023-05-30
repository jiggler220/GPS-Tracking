using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SvTracker.Models
{
    public class RiseSetTime
    {
        public RiseSetTime(DateTime riseTime, DateTime setTime)
        {
            this.RiseTime = riseTime;
            this.SetTime = setTime;
        } 

        public DateTime RiseTime { get; set; }

        public DateTime SetTime { get; set; }
    }
}
