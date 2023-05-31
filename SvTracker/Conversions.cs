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

    // TODO ADD SEM CONVERSION
    //    // SEM CONVERSIONS
    //    public Dictionary<int, Satellite> SEMFileToSVConstellation(string fileName)
    //    {
    //        Dictionary<int, Satellite> allSats = new Dictionary<int, Satellite>();
    //        List<string> allLines = new List<string>(File.ReadAllLines(fileName));
    //        for (int i = 0; i < allLines.Count; i++)
    //        {
    //            Match match = Constants.YUMA_REGEX.Match(allLines[i]);
    //            if (match.Success)
    //            {
    //                int prn = -1;

    //                try
    //                {
    //                    Satellite sat = ParseSEMSV(allLines.GetRange(i, 14), out prn);
    //                    i += 14;

    //                    if (prn != -1)
    //                    {
    //                        allSats.Add(prn, sat);
    //                    }
    //                }

    //                catch (ArgumentException e)
    //                {
    //                    Console.WriteLine($"Invalid Almanac File for lines {i} - {i + 14}");
    //                    break;
    //                }
    //            }

    //            else
    //            {
    //                continue;
    //            }
    //        }

    //        return allSats;
    //    }

    //    private Satellite ParseSEMSV(List<string> svLines, out int prn)
    //    {

    //        // Parameters per the YUMA file standards
    //        Dictionary<string, string> yumaValues = new Dictionary<string, string>(){
    //            {"ID", ""},
    //            {"Health", ""},
    //            {"Eccentricity", ""},
    //            {"Time of Applicability(s)", ""},
    //            {"Orbital Inclination(rad)", ""},
    //            {"Rate of Right Ascen(r/s)", ""},
    //            {"SQRT(A)  (m 1/2)", ""},
    //            {"Right Ascen at Week(rad)", ""},
    //            {"Argument of Perigee(rad)", ""},
    //            {"Mean Anom(rad)", ""},
    //            {"Af0(s)", ""},
    //            {"Af1(s/s)", ""},
    //            {"week", ""},
    //            };

    //        foreach (string line in svLines)
    //        {
    //            string[] sections = line.Split(":");

    //            if (yumaValues.ContainsKey(sections[0]))
    //            {
    //                yumaValues[sections[0]] = sections[1].Trim();
    //            }
    //        }

    //        prn = Int32.Parse(yumaValues["ID"]);

    //        return new Satellite(health: Int32.Parse(yumaValues["Health"]),
    //            eccentricity: Double.Parse(yumaValues["Eccentricity"]),
    //            timeOfApplicability_s: Double.Parse(yumaValues["Time of Applicability(s)"]),
    //            inclination_rad: Double.Parse(yumaValues["Orbital Inclination(rad)"]),
    //            rateRightAscen_radps: Double.Parse(yumaValues["Rate of Right Ascen(r/s)"]),
    //            sqrtSemiMajor: Double.Parse(yumaValues["SQRT(A)  (m 1/2)"]),
    //            rightAscenAtWeek_rad: Double.Parse(yumaValues["Right Ascen at Week(rad)"]),
    //            argOfPerigee_rad: Double.Parse(yumaValues["Argument of Perigee(rad)"]),
    //            meanAnom_rad: Double.Parse(yumaValues["Mean Anom(rad)"]),
    //            af0_s: Double.Parse(yumaValues["Af0(s)"]),
    //            af1_s: Double.Parse(yumaValues["Af1(s/s)"]),
    //            week: Int32.Parse(yumaValues["week"])
    //            );
    //    }

    //    // END SEM CONVERSIONS
    //}

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
