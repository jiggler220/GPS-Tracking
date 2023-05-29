using System;

namespace SvTracker.Models
{

    // 'Health',                   % - health(healthy: 0)
    // 'e',                        % - eccentricity
    // 't0',                       % - time of applicability[s]
    // 'i',                        % - orbital inclination[rad]
    // 'OMEGAdot',                 % - rate of right ascension[rad / s]
    // 'sqrtA',                    % - square root of semi-major axis[m ^ 0.5]
    // 'OMEGA0',                   % - right ascension at week[rad]
    // 'omega',                    % - argument of perigee[rad]
    // 'M0',                       % - mean anomaly
    // 'Af0',                      % - clock parameter 0                [s]
    // 'Af1',                      % - clock parameter 1                [s/s]
    // 'Week',                     % - week

    public class Satellite
    {
        public Satellite(int health, double eccentricity, double timeOfApplicability_s,
            double inclination_rad, double rateRightAscen_radps, double sqrtSemiMajor,
            double rightAscenAtWeek_rad, double argOfPerigee_rad, double meanAnom_rad,
            double af0_s, double af1_s, int week)
        {
            Health = health;
            e = eccentricity;
            t0 = timeOfApplicability_s;
            i = inclination_rad;
            OMEGAdot = rateRightAscen_radps;
            sqrtA = sqrtSemiMajor;
            OMEGA0 = rightAscenAtWeek_rad;
            omega = argOfPerigee_rad;
            M0 = meanAnom_rad;
            Af0 = af0_s;
            Af1 = af1_s;
            Week = week;
        }

        private int Health { get; set; }
        private double e { get; set; }
        private double t0 { get; set; }
        private double i { get; set; }
        private double OMEGAdot { get; set; }
        private double sqrtA { get; set; }
        private double OMEGA0 { get; set; }
        private double omega { get; set; }
        private double M0 { get; set; }
        private double Af0 { get; set; }
        private double Af1 { get; set; }
        private int Week { get; set; }
        public ECEFCoordinate EcefCoord { get; set; } = new ECEFCoordinate(0,0,0);
        public GeodeticCoordinate GeoCoord { get; set; }
        public double ElevationAngle { get; set; }
        public bool IsVisible { get; set; }

        public void ComputeCoordinates(DateTime referenceTime, AppConfig appConfig, int prn)
        {
            GPSTime time = new GPSTime(referenceTime, appConfig);

            double dw = time.WeekNumber - this.Week + 1024 * (this.Week - time.WeekNumber > 1 ? 1 : 0);

            double da = 7 * dw + Math.Floor((time.GPSSecondOfWeek - this.t0) / 86400);

            // Time from Epoch
            double timeFromEpoch = 604800 * dw + time.GPSSecondOfWeek - this.t0;

            // Semi-Major Axis
            double A = this.sqrtA * this.sqrtA;

            // Mean Motion
            double n = Math.Sqrt(Constants.MU) / (this.sqrtA * this.sqrtA * this.sqrtA);

            // Mean Anomaly
            double mk = this.M0 + n * timeFromEpoch;

            // Eccentricity Anomaly
            double Ekp = Double.PositiveInfinity;

            double Ek = Ekf(mk, e, mk);

            while (Math.Abs(Ek-Ekp) > 1e-10)
            {
                Ekp = Ek;
                Ek = Ekf(Ek, e, mk);
            }

            // True Anomaly
            double nuk = Math.Atan2(Math.Sqrt(1 - (e*e)) * Math.Sin(Ek), Math.Cos(Ek) - e);

            // Argument of Latitude
            double phik = omega + nuk;

            // Radius
            double rk = A * (1 - e * Math.Cos(Ek));

            // Positions in orbital plane
            double xkp = rk * Math.Cos(phik);
            double ykp = rk * Math.Sin(phik);

            // Longitude of ascending node
            double OMEGAk = this.OMEGA0 + (this.OMEGAdot - Constants.OMEGADOT) * timeFromEpoch - Constants.OMEGADOT * t0;

            // ECEF X-axis, Y-Axis, and Z-Axis
            double xk = xkp * Math.Cos(OMEGAk) - ykp * Math.Cos(this.i) * Math.Sin(OMEGAk);
            double yk = xkp * Math.Sin(OMEGAk) + ykp * Math.Cos(this.i) * Math.Cos(OMEGAk);
            double zk = ykp * Math.Sin(i);

            // Could do this for readability but much slower
            //this.EcefCoord = new ECEFCoordinate(xk, yk, zk);
            this.EcefCoord.SetEcefCoord(xk, yk, zk);

            this.GeoCoord = this.EcefCoord.ECEFToGeodetic();
        }

        // Newton Method for eccentricity anomaly
        private double Ekf(double E, double e, double Mk)
        {
            return E - ((E - e * (Math.Sin(E)) - Mk) / (1 - e * (Math.Cos(E))));
        }

        public void CalculateVisibility(ECEFCoordinate refCoord, double elevationMaskAngle)
        {
            this.ElevationAngle = CalculateAngleBetween2Points(refCoord, this.EcefCoord);
            this.IsVisible = this.ElevationAngle >= elevationMaskAngle;
        }

        private double CalculateAngleBetween2Points(ECEFCoordinate refCoord, ECEFCoordinate satGeoCoord)
        {
            //Cos(elevation) = (x * dx + y * dy + z * dz) / Sqrt((x ^ 2 + y ^ 2 + z ^ 2) * (dx ^ 2 + dy ^ 2 + dz ^ 2))
            satGeoCoord.X -= refCoord.X;
            satGeoCoord.Y -= refCoord.Y;
            satGeoCoord.Z -= refCoord.Z;

            double elevation = (refCoord.X * satGeoCoord.X + refCoord.Y * satGeoCoord.Y + refCoord.Z * satGeoCoord.Z) /
                Math.Sqrt((refCoord.X * refCoord.X + refCoord.Y * refCoord.Y + refCoord.Z * refCoord.Z) *
                (satGeoCoord.X * satGeoCoord.X + satGeoCoord.Y * satGeoCoord.Y + satGeoCoord.Z * satGeoCoord.Z));

            return 90 - Math.Acos(elevation) * Constants.RAD2DEG;
        }

        public void PrintInformation(int id)
        {
            Console.WriteLine($"PRN: {id}");
            Console.WriteLine($"Latitude: {this.GeoCoord.Latitude}");
            Console.WriteLine($"Longitude: {this.GeoCoord.Longitude}");
            Console.WriteLine($"Altitude: {this.GeoCoord.Altitude}");
            Console.WriteLine($"Elevation Angle: {this.ElevationAngle}");
            Console.WriteLine($"In View: {this.IsVisible}");
            Console.WriteLine();
        }

    }
}
