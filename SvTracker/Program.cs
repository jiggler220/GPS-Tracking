using SvTracker.Models;
using System;

namespace SvTracker
{
    class Program
    {
        static void Main(string[] args)
        {
            string filename = "C:\\Users\\metca\\source\\repos\\SvTracker\\SvTracker\\sampleAlm.alm";
            int elevationMaskAngle = 5;
            GeodeticCoordinate receiverCoords = new GeodeticCoordinate(latitude: 34, 118, 0);

            Constellation constellation = new Constellation(DateTime.UtcNow, filename);
            constellation.ComputeVisibility(receiverCoords, elevationMaskAngle);
            constellation.PrintConstellationInfo();

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
