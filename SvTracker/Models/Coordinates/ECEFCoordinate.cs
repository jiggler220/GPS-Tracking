using System;

namespace SvTracker.Models
{
    public class ECEFCoordinate
    {
        public ECEFCoordinate(double x,double y,double z)
        {
            SetEcefCoord(x, y, z);
        }

        public void SetEcefCoord(double x, double y, double z)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }

        // Convert Earth-Centered-Earth-Fixed (ECEF) to lat, Lon, Altitude
        // From https://danceswithcode.net/engineeringnotes/geodetic_to_ecef/geodetic_to_ecef.html
        // https://en.wikipedia.org/wiki/Geographic_coordinate_conversion#From_geodetic_to_ECEF_coordinates
        public GeodeticCoordinate ECEFToGeodetic()
        {
            double zp = Math.Abs(this.Z);
            double w2 = this.X * this.X + this.Y * this.Y;
            double w = Math.Sqrt(w2);
            double r2 = w2 + this.Z * this.Z;
            double r = Math.Sqrt(r2);
            double s2 = this.Z * this.Z / r2;
            double c2 = w2 / r2;
            double u = Constants.a2 / r;
            double v = Constants.a3 - Constants.a4 / r;
            double latitude;
            double ss;
            double c;
            double s;
            if (c2 > 0.3)
            {
                s = (zp / r) * (1.0 + c2 * (Constants.a1 + u + s2 * v) / r);
                latitude = Math.Asin(s);      //Lat
                ss = s * s;
                c = Math.Sqrt(1.0 - ss);
            }
            else
            {
                c = (w / r) * (1.0 - s2 * (Constants.a5 - u - c2 * v) / r);
                latitude = Math.Acos(c);      //Lat
                ss = 1.0 - c * c;
                s = Math.Sqrt(ss);
            }
            double g = 1.0 - Constants.E2 * ss;
            double rg = Constants.EARTH_RADIUS / Math.Sqrt(g);
            double rf = Constants.a6 * rg;
            u = w - rg * c;
            v = zp - rf * s;
            double f = c * u + s * v;
            double m = c * v - s * u;
            double p = m / (rf / g + f);
            latitude = latitude + p;      //Lat
            if (this.Z < 0.0) { latitude *= -1.0; }
            double longitude = Math.Atan2(this.Y, this.X);
            double altitude = f + m * p / 2.0;     //Altitude

            // Convert from radians to degrees
            latitude *= Constants.RAD2DEG;
            longitude *= Constants.RAD2DEG;

            return new GeodeticCoordinate(latitude, longitude, altitude);

        }
    }
}
