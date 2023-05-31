using System;
using System.Collections.Generic;
using System.IO;

namespace SvTracker.Models
{
    public class Constellation
    {

        public Constellation(DateTime referenceTime, string fileName) {
            this.RecentReferenceTime = referenceTime;
            this.AppConfig = new AppConfig();
            PopulateSVsFromAlmanac(fileName);
        }

        public Constellation(GeodeticCoordinate refCoord, double elevationMaskAngle, DateTime referenceTime, string fileName)
        {
            this.RecentReferenceTime = referenceTime;
            this.AppConfig = new AppConfig();
            this.AllSVs = PopulateSVsFromAlmanac(fileName);
            SetConstellationVisibilities(refCoord.GeodeticToECEF(), elevationMaskAngle) ;
        }

        private Dictionary<int, Satellite> PopulateSVsFromAlmanac(string fileName)
        {
            FileInfo fileInfo = new FileInfo(fileName);
            Dictionary<int, Satellite> svs = new Dictionary<int, Satellite>();

            //YUMA
            if (fileInfo.Extension == ".alm")
            {
                Conversions conversion = new Conversions();
                svs = conversion.YumaFileToSVConstellation(fileName);
                SetConstellationCoords(this.RecentReferenceTime);
            }

            //SEM
            else if (fileInfo.Extension == ".al3")
            {
                // DO LATER
            }

            else
            {
                throw new Exception("Invalid File Format");
            }

            return svs;
        }

        public void UpdateAlmanac(string fileName)
        {
            Dictionary<int, Satellite> updatedSvs = PopulateSVsFromAlmanac(fileName);
            foreach (var sat in updatedSvs)
            {
                if (AllSVs.ContainsKey(sat.Key))
                {
                    AllSVs[sat.Key] = sat.Value;
                }
            }
        }

        public void SetConstellationCoords(DateTime dateTime)
        {
            foreach (var sat in AllSVs)
            {
                sat.Value.EcefCoord = sat.Value.ComputeCoordinates(dateTime, this.AppConfig);
            }
        }

        public void SetConstellationVisibilities(ECEFCoordinate refCoord, double elevationMaskAngle)
        {
            foreach (var sat in AllSVs)
            {
                sat.Value.SetVisibilityAndAngle(refCoord, sat.Value.EcefCoord, elevationMaskAngle);
            }
        }

        public void ComputeConstellationCoords(DateTime dateTime)
        {
            foreach (var sat in AllSVs)
            {
                sat.Value.ComputeCoordinates(dateTime, this.AppConfig);
            }
        }

        public Dictionary<int, Dictionary<DateTime, ECEFCoordinate>> ComputeConstellationECEFCoordsOverGivenPeriod(DateTime startDate, DateTime endDate)
        {
            Dictionary<int, Dictionary<DateTime, ECEFCoordinate>> ecefPeriodCoords = new Dictionary<int, Dictionary<DateTime, ECEFCoordinate>>();
            foreach (var sat in AllSVs)
            {
                Dictionary<DateTime, ECEFCoordinate> ecefDict = new Dictionary<DateTime, ECEFCoordinate>();
                DateTime tempStartDate = startDate;
                while (tempStartDate <= endDate)
                {
                    ECEFCoordinate ecefCoord = sat.Value.ComputeCoordinates(startDate, this.AppConfig);
                    ecefDict.Add(tempStartDate, ecefCoord);
                    tempStartDate = tempStartDate.AddSeconds(1);
                }
                ecefPeriodCoords.Add(sat.Key, ecefDict);
            }

            return ecefPeriodCoords;
        }

        public Dictionary<int, Dictionary<DateTime, GeodeticCoordinate>> ComputeConstellationGeoCoordsOverGivenPeriod(DateTime startDate, DateTime endDate)
        {
            Dictionary<int, Dictionary<DateTime, GeodeticCoordinate>> geoPeriodCoords = new Dictionary<int, Dictionary<DateTime, GeodeticCoordinate>>();
            foreach (var sat in AllSVs)
            {
                Dictionary<DateTime, GeodeticCoordinate> ecefDict = new Dictionary<DateTime, GeodeticCoordinate>();
                DateTime tempStartDate = startDate;
                while (tempStartDate <= endDate)
                {
                    GeodeticCoordinate geoCoord = sat.Value.ComputeCoordinates(startDate, this.AppConfig).ECEFToGeodetic();
                    ecefDict.Add(tempStartDate, geoCoord);
                    tempStartDate = tempStartDate.AddSeconds(1);
                }
                geoPeriodCoords.Add(sat.Key, ecefDict);
            }

            return geoPeriodCoords;
        }

        public void ComputeRiseSetTimes(DateTime startTime, DateTime endTime, GeodeticCoordinate refCoord, double increment_s=60, double elevationMaskAngle=5)
        {
            ECEFCoordinate ecef = refCoord.GeodeticToECEF();
            foreach (var sat in AllSVs)
            {
                sat.Value.CalculateRiseSetTimes(startTime, endTime, this.AppConfig, ecef, increment_s, elevationMaskAngle);
            }
        }

        public void ComputeVisibility(GeodeticCoordinate receiverCoord, double elevationMaskAngle)
        {
            foreach (var sat in AllSVs)
            {
                sat.Value.CalculateVisibility(receiverCoord.GeodeticToECEF(), sat.Value.EcefCoord, elevationMaskAngle);
            }
        }

        public void PrintConstellationInfo()
        {
            foreach (var sat in AllSVs)
            {
                sat.Value.PrintInformation(sat.Key);
            }
        }

        public Dictionary<int, Satellite> AllSVs { get; set; }

        public DateTime RecentReferenceTime { get; set; }

        public AppConfig AppConfig { get; set; }
    }
}
