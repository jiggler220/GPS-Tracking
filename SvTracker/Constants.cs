using System;
using System.Text.RegularExpressions;

namespace SvTracker
{
    public class Constants
    {
        public static readonly double EARTH_RADIUS = 6378137.0;

        public static readonly double OMEGADOT = 7.2921151467e-5;

        public static readonly double MU = 3.986004418e14;

        public static readonly double E2 = 6.6943799901377997e-3;

        public static readonly double a1 = 4.2697672707157535e+4;  //a1 = a*e2
        public static readonly double a2 = 1.8230912546075455e+9;  //a2 = a1*a1
        public static readonly double a3 = 1.4291722289812413e+2;  //a3 = a1*e2/2
        public static readonly double a4 = 4.5577281365188637e+9;  //a4 = 2.5*a2
        public static readonly double a5 = 4.2840589930055659e+4;  //a5 = a1+a3
        public static readonly double a6 = 9.9330562000986220e-1;  //a6 = 1-e2

        public static readonly double RAD2DEG = 180 / Math.PI;

        public static readonly double DEG2RAD = Math.PI / 180;

        public static readonly DateTime GPS_ORIGIN = new DateTime(1980, 01, 06);

        public static readonly Regex YUMA_REGEX = new Regex(@"^\*{8} Week \d{1,4} almanac for [A-Z]{3}-\d{2} \*{8}$");
    }
}
