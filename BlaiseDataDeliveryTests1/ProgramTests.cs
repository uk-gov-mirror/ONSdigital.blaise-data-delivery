using Microsoft.VisualStudio.TestTools.UnitTesting;
using BlaiseDataDelivery;
using System;
using System.Collections.Generic;
using System.Text;
using DDE.Tests;

namespace BlaiseDataDelivery.Tests
{
    [TestClass()]
    public class ProgramTests
    {
        [TestMethod()]
        public void MainDDETest()
        {
            //string SurveyIn = @"C:\aSurvey\opn-jan-b5";
            //string SpsOut = @"C:\dde\dde_out\";
            string dbixFileDirectory = @"C:\dde\dde_out\";
            
            // string dbixFileName = @"C:\Blaise5\Surveys\opn1911a\OPN1911A.bdix";
            string dbixFileName = @"C:\aSurvey\opn-jan-b5";
            string[] aFilterFields = { "APPOINTT",
                                        "DATESTAR",
                                        "TIMESTAR",
                                        "WEEKDAY1",
                                        "NROFCALL",
                                        "FIRSTDAY",
                                        "DAYNUMBE",
                                        "DIALTIME",
                                        "GENCOM  ",
                                        "COUNTDET",
                                        "PREM1   ",
                                        "POSTCODE", }; //To filter which fields are processed, add a full or partial field name in uppercase
            //Program.MainDDE(dbixFileName, dbixFileDirectory, aFilterFields);
        }
    }
}