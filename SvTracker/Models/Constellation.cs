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

        private void PopulateSVsFromAlmanac(string fileName)
        {
            FileInfo fileInfo = new FileInfo(fileName);

            if (fileInfo.Extension == ".alm")
            {
                Conversions conversion = new Conversions();
                AllSVs = conversion.YumaFileToSVConstellation(fileName);
                ComputeConstellationCoords(this.RecentReferenceTime);
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

        public void ComputeConstellationCoords(DateTime dateTime)
        {
            foreach (var sat in AllSVs)
            {
                sat.Value.ComputeCoordinates(dateTime, this.AppConfig, sat.Key);
            }
        }

        public void ComputeVisibility(GeodeticCoordinate receiverCoord, double elevationMaskAngle)
        {
            foreach (var sat in AllSVs)
            {
                sat.Value.CalculateVisibility(receiverCoord.GeodeticToECEF(), elevationMaskAngle);
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
