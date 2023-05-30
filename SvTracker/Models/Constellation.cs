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
            PopulateSVsFromAlmanac(fileName);
            SetConstellationVisibilities(refCoord.GeodeticToECEF(), elevationMaskAngle) ;
        }

        private void PopulateSVsFromAlmanac(string fileName)
        {
            FileInfo fileInfo = new FileInfo(fileName);

            if (fileInfo.Extension == ".alm")
            {
                Conversions conversion = new Conversions();
                AllSVs = conversion.YumaFileToSVConstellation(fileName);
                SetConstellationCoords(this.RecentReferenceTime);
            }

            else if (fileInfo.Extension == ".al3")
            {
                // DO LATER
            }

            else
            {
                throw new Exception("Invalid File Format");
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
