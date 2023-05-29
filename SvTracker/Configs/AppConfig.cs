using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SvTracker
{
    public class AppConfig : IAppConfig
    {
        public AppConfig()
        {
            SetLeapSeconds(18);
        }

        public AppConfig(int leapSeconds)
        {
            SetLeapSeconds(leapSeconds);
        }

        public int LeapSeconds { get; private set; }

        public void SetLeapSeconds(int  leapSeconds)
        {
            this.LeapSeconds = leapSeconds;
        }

    }
}
