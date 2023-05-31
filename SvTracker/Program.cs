using SvTracker.Models;
using System;
using System.Collections.Generic;

namespace SvTracker
{
    class Program
    {
        static void Main(string[] args)
        {

            // TODOS
            // -------------------------------------------------------------------------------------------------------
            // Add SOF File to Rise Set Times
            // Can probably just multiply by some constant in computing coordinates over a period of time to save time
            // Update Almanac program implementation
            // Can probably just do Geo coords for ComputeConstellationGeoCoordsOverGivenPeriod instead of ecef to geo
            // Add SEM parser
            // -------------------------------------------------------------------------------------------------------


            string filename = "C:\\Users\\metca\\source\\repos\\GPSTracking\\SvTracker\\sampleAlm.alm";
            int elevationMaskAngle = 5;
            var watch = new System.Diagnostics.Stopwatch();
            watch.Start();

            // Set Receiver Location
            GeodeticCoordinate receiverCoords = new GeodeticCoordinate(latitude: 33, -118, 0);

            // Create Constellation relative to time specified from the file specified
            Constellation constellation = new Constellation(receiverCoords, elevationMaskAngle, DateTime.UtcNow, filename);
            
            // Compute the ecef coordinates for the time specified
            Dictionary<int, Dictionary<DateTime, ECEFCoordinate>> ecefCoords = constellation.ComputeConstellationECEFCoordsOverGivenPeriod(DateTime.UtcNow, DateTime.UtcNow.AddHours(12));
            
            // Compute the geo coordinates for the time specified
            Dictionary<int, Dictionary<DateTime, GeodeticCoordinate>> geoCoords = constellation.ComputeConstellationGeoCoordsOverGivenPeriod(DateTime.UtcNow, DateTime.UtcNow.AddHours(12));
            
            // Compute the rise and set times from start to end date, for the given receiver
            constellation.ComputeRiseSetTimes(DateTime.UtcNow.AddDays(-1), DateTime.UtcNow.AddDays(1), receiverCoords);

            // Print General Constellation Info
            constellation.PrintConstellationInfo();
            watch.Stop();
            Console.WriteLine($"Execution Time: {watch.ElapsedMilliseconds} ms");
        }
    }
}
