using System;

namespace SvTracker.Models
{
    public class GeodeticCoordinate
    {
        public GeodeticCoordinate(double latitude, double longitude ,double altitude)
        {
            this.Latitude = latitude;
            this.Longitude = longitude;
            this.Altitude = altitude;
        }

        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Altitude { get; set; }

        // Convert Lat, Lon, Altitude to Earth-Centered-Earth-Fixed (ECEF)
        // From https://danceswithcode.net/engineeringnotes/geodetic_to_ecef/geodetic_to_ecef.html
        // https://en.wikipedia.org/wiki/Geographic_coordinate_conversion#From_geodetic_to_ECEF_coordinates
        public ECEFCoordinate GeodeticToECEF()
        {
            double latitude = this.Latitude * Constants.DEG2RAD;
            double longitude = this.Longitude * Constants.DEG2RAD;

            double n = Constants.EARTH_RADIUS / Math.Sqrt(1 - Constants.E2 * Math.Sin(latitude) * Math.Sin(latitude));
            double ecefX = (n + this.Altitude) * Math.Cos(latitude) * Math.Cos(longitude);
            double ecefY = (n + this.Altitude) * Math.Cos(latitude) * Math.Sin(longitude);
            double ecefZ = (n * (1 - Constants.E2) + this.Altitude) * Math.Sin(latitude);

            return new ECEFCoordinate(ecefX, ecefY, ecefZ);
        }
    }
}
