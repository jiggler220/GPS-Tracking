using SvTracker.Models;
using System;

namespace SvTracker
{
    class Program
    {
        static void Main(string[] args)
        {
            string filename = "C:\\Users\\metca\\source\\repos\\GPSTracking\\SvTracker\\sampleAlm.alm";
            int elevationMaskAngle = 5;
            var watch = new System.Diagnostics.Stopwatch();
            watch.Start();

            GeodeticCoordinate receiverCoords = new GeodeticCoordinate(latitude: 33, -118, 0);

            Constellation constellation = new Constellation(receiverCoords, elevationMaskAngle, DateTime.UtcNow, filename);
            constellation.ComputeRiseSetTimes(DateTime.UtcNow.AddDays(-1), DateTime.UtcNow.AddDays(1), receiverCoords);
            constellation.PrintConstellationInfo();
            watch.Stop();
            Console.WriteLine($"Execution Time: {watch.ElapsedMilliseconds} ms");

            //DebugTest(constellation);
        }

        public static void DebugTest(Constellation constellation)
        {
            var watch = new System.Diagnostics.Stopwatch();
            watch.Start();

            for (int i = 0; i <= 86400 * 30; i += 60)
            {
                constellation.ComputeConstellationCoords(DateTime.UtcNow.AddSeconds(i));
                constellation.PrintConstellationInfo();
            }

            watch.Stop();

            Console.WriteLine($"Execution Time: {watch.ElapsedMilliseconds} ms");
        }
    }
}
