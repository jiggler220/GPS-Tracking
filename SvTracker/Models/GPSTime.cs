using System;

namespace SvTracker.Models
{
    class GPSTime
    {
        public GPSTime(DateTime referenceTime, AppConfig appConfig)
        {
            this.ReferenceTime = referenceTime;
            GetTimeParams(appConfig);
        }

        private void GetTimeParams(AppConfig appConfig)
        {

            // Get the current reference time and add leap seconds
            DateTime currentTime = ReferenceTime.AddSeconds(appConfig.LeapSeconds);

            // Zeroize Hours, Seconds, Minutes, Millis
            DateTime currentDay = new DateTime(currentTime.Year, currentTime.Month, currentTime.Day, 0, 0, 0, 0);

            // Total Seconds from start of reference day to reference time
            double totalDaySecs = (currentTime - currentDay).TotalSeconds;

            // Get Seconds in week
            this.GPSSecondOfWeek = 86400 * (int)currentTime.DayOfWeek + totalDaySecs;

            // Used for calculating rollover to calculate week number
            double totalDaysSinceOrigin = (currentTime - Constants.GPS_ORIGIN).TotalDays;

            this.Rollover = (int)Math.Floor(totalDaysSinceOrigin / 7 / 1024);

            this.WeekNumber = (int)Math.Floor(totalDaysSinceOrigin / 7 - this.Rollover * 1024);

        }

        public DateTime ReferenceTime { get; set; }

        public int WeekNumber { get; set; }

        public int Rollover { get; set; }

        public double GPSSecondOfWeek { get; set; }

    }
}
