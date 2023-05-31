using SvTracker.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace SvTracker
{
    public class Conversions
    {
        public Conversions() {}

        // SEM CONVERSIONS
        public Dictionary<int, Satellite> SEMFileToSVConstellation(string fileName)
        {
            Dictionary<int, Satellite> allSats = new Dictionary<int, Satellite>();
            List<string> allLines = new List<string>(File.ReadAllLines(fileName));

            try
            {
                Match entriesMatch = Constants.SEM_ENTRIES_REGEX.Match(allLines[0]);
                Match timeMatch = Constants.SEM_TIME_REGEX.Match(allLines[1]);
                int parsedNumberEntries = int.Parse(entriesMatch.Groups[1].Value);
                int parsedGPSWeek = int.Parse(timeMatch.Groups[1].Value);
                double parsedTimeOfApplicability = double.Parse(timeMatch.Groups[2].Value);

                int tempNum = 3;
                int prn = -1;
                for (int i = 0; i < parsedNumberEntries; i++)
                {
                    Satellite sat = ParseSEMSV(allLines.GetRange(tempNum, 8), parsedGPSWeek, parsedTimeOfApplicability, out prn );
                    tempNum += 9;

                    if (prn != -1)
                    {
                        allSats.Add(prn, sat);
                    }
                }
            }

            catch
            {
                throw new Exception("Unable To Read SEM File");
            }

            return allSats;
        }

        private Satellite ParseSEMSV(List<string> svLines, int GPSWeek, double timeOfApplicability, out int prn)
        {
            bool error = false;
            prn = Int32.Parse(svLines[0]);
            double eccentricity = 0;
            double inclinationOffset_semi = 0;
            double rateOfRightAcension_semi = 0;
            double squareRootSemiMajorAxis_m = 0;
            double geoLongOfOribtalPlane_semi = 0;
            double argumentOfPerigee_semi = 0;
            double meanAnomaly_semi = 0;
            double zerothOrderClockCorrection_s = 0;
            double firstOrderClockCorrection_sps = 0;

            // Parse Records form GPS ICD 240A
            // Record 4
            string[] r4Array = Array.FindAll(svLines[3].TrimStart().Split(" "), s => !string.IsNullOrEmpty(s));
            if (r4Array.Length == 3)
            {
                eccentricity = double.Parse(r4Array[0]);
                inclinationOffset_semi = double.Parse(r4Array[1]);
                rateOfRightAcension_semi = double.Parse(r4Array[2]);

                if (eccentricity >= 0 && eccentricity <= 3.125E-2 &&
                    inclinationOffset_semi >= -6.25E-2 && inclinationOffset_semi <= 6.25E-2 &&
                    rateOfRightAcension_semi >= -1.1921E-7 && rateOfRightAcension_semi <= 1.1921E-7)
                {
                    // Do nothing
                }
                else { error = true; }
            }

            else { error = true; }

            // Record 5
            string[] r5Array = Array.FindAll(svLines[4].TrimStart().Split(" "), s => !string.IsNullOrEmpty(s));
            if (r5Array.Length == 3)
            {
                squareRootSemiMajorAxis_m = double.Parse(r5Array[0]);
                geoLongOfOribtalPlane_semi = double.Parse(r5Array[1]);
                argumentOfPerigee_semi = double.Parse(r5Array[2]);

                if (squareRootSemiMajorAxis_m >= 0 && squareRootSemiMajorAxis_m <= 8192 &&
                    geoLongOfOribtalPlane_semi >= -1.0 && geoLongOfOribtalPlane_semi <= 1.0 &&
                    argumentOfPerigee_semi >= -1.0 && argumentOfPerigee_semi <= 1.0)
                {
                    // Do nothing
                }
                else { error = true; }
            }

            else { error = true; }


            // Record 6
            string[] r6Array = Array.FindAll(svLines[5].TrimStart().Split(" "), s => !string.IsNullOrEmpty(s));
            if (r6Array.Length == 3)
            {
                meanAnomaly_semi = double.Parse(r6Array[0]);
                zerothOrderClockCorrection_s = double.Parse(r6Array[1]);
                firstOrderClockCorrection_sps = double.Parse(r6Array[2]);

                if (meanAnomaly_semi >= -1.0 && meanAnomaly_semi <= 1.0 &&
                    zerothOrderClockCorrection_s >= -9.7657E-4 && zerothOrderClockCorrection_s <= 9.7657E-4 &&
                    firstOrderClockCorrection_sps >= -3.7253E-9 && firstOrderClockCorrection_sps <= 3.7253E-9)
                {
                    // Do nothing
                }
                else { error = true; }
            }

            else { error = true; }

            if (error)
            {
                throw new Exception("Invalid Almanac File Parameters");
            }

            return new Satellite(health: 1,
                eccentricity: eccentricity,
                timeOfApplicability_s: timeOfApplicability,
                inclination_rad: (inclinationOffset_semi + .3)*Math.PI, // Semi Circle To Rad plus offset as described in GPS ICD 240A
                rateRightAscen_radps: rateOfRightAcension_semi * Math.PI, // Semi Circle To Rad
                sqrtSemiMajor: squareRootSemiMajorAxis_m,
                rightAscenAtWeek_rad: geoLongOfOribtalPlane_semi * Math.PI, // Semi Circle To Rad
                argOfPerigee_rad: argumentOfPerigee_semi * Math.PI, // Semi Circle To Rad
                meanAnom_rad: meanAnomaly_semi * Math.PI, // Semi Circle to Rad
                af0_s: zerothOrderClockCorrection_s,
                af1_s: firstOrderClockCorrection_sps,
                week: GPSWeek
                );
        }

    // END SEM CONVERSIONS

    // YUMA CONVERSIONS
    public Dictionary<int, Satellite> YumaFileToSVConstellation(string fileName)
        {
            Dictionary<int, Satellite> allSats = new Dictionary<int, Satellite>();
            List<string> allLines = new List<string>(File.ReadAllLines(fileName));
            for (int i = 0; i < allLines.Count; i++)
            {
                Match match = Constants.YUMA_REGEX.Match(allLines[i]);
                if (match.Success)
                {
                    int prn = -1;

                    try
                    {
                        Satellite sat = ParseYumaSV(allLines.GetRange(i, 14), out prn);
                        i += 14;

                        if (prn != -1)
                        {
                            allSats.Add(prn, sat);
                        }
                    }

                    catch (ArgumentException e)
                    {
                        Console.WriteLine($"Invalid Almanac File for lines {i} - {i + 14}");
                        break;
                    }
                }

                else
                {
                    continue;
                }
            }

            return allSats;
        }

        private Satellite ParseYumaSV(List<string> svLines, out int prn)
        {

            // Parameters per the YUMA file standards
            Dictionary<string, string> yumaValues = new Dictionary<string, string>(){
                {"ID", ""},
                {"Health", ""},
                {"Eccentricity", ""},
                {"Time of Applicability(s)", ""},
                {"Orbital Inclination(rad)", ""},
                {"Rate of Right Ascen(r/s)", ""},
                {"SQRT(A)  (m 1/2)", ""},
                {"Right Ascen at Week(rad)", ""},
                {"Argument of Perigee(rad)", ""},
                {"Mean Anom(rad)", ""},
                {"Af0(s)", ""},
                {"Af1(s/s)", ""},
                {"week", ""},
                };

            foreach (string line in svLines)
            {
                string[] sections = line.Split(":");

                if (yumaValues.ContainsKey(sections[0]))
                {
                    yumaValues[sections[0]] = sections[1].Trim();
                }
            }

            prn = Int32.Parse(yumaValues["ID"]);

            return new Satellite(health: Int32.Parse(yumaValues["Health"]),
                eccentricity: Double.Parse(yumaValues["Eccentricity"]),
                timeOfApplicability_s: Double.Parse(yumaValues["Time of Applicability(s)"]),
                inclination_rad: Double.Parse(yumaValues["Orbital Inclination(rad)"]),
                rateRightAscen_radps: Double.Parse(yumaValues["Rate of Right Ascen(r/s)"]),
                sqrtSemiMajor: Double.Parse(yumaValues["SQRT(A)  (m 1/2)"]),
                rightAscenAtWeek_rad: Double.Parse(yumaValues["Right Ascen at Week(rad)"]),
                argOfPerigee_rad: Double.Parse(yumaValues["Argument of Perigee(rad)"]),
                meanAnom_rad: Double.Parse(yumaValues["Mean Anom(rad)"]),
                af0_s: Double.Parse(yumaValues["Af0(s)"]),
                af1_s: Double.Parse(yumaValues["Af1(s/s)"]),
                week: Int32.Parse(yumaValues["week"])
                );
        }

        // END YUMA CONVERSIONS
    }


}
