/*  SPS file definition is set out below.  Each section is created by the function denoted after >>>>> below
 *  
TITLE		'OPN1904a'.
FILE HANDLE OPN1904A /NAME = 
'C:\dde_SPS_org\OPN1904A.ASC'
/LRECL = 9.
DATA LIST FILE = OPN1904A
/
                                                                                >>>>>  Create a Unique list of Fields - hashSps = SpsFldList() 
		APPOINTT                                     1 -      1                    
		DATESTAR                                     2 -      9
		TIMESTAR                                    10 -     17		(TIME)
		NROFCALL                                    51 -     52
		FIRSTDAY                                    53 -     60
		DAYNUMBE                                    71 -     73
		DIALTIME                                    74 -     81		(TIME)
		PREM1                                    10910 -  10944		(A)
		POSTCODE                                 11120 -  11126		(A).
                        
        >>>>>>>   genDateTimeFields.GetDateFormats(hashSps);    //4th Block in Orig SPS - FORMATS (DATE)

DO IF NOT MISSING(DATESTAR).

COMPUTE
    #DD = TRUNC(DATESTAR / 1000000).
COMPUTE
    #RM = MOD(DATESTAR, 1000000).
COMPUTE
    #MM = TRUNC(#RM / 10000).
COMPUTE
    #YY = MOD(#RM, 10000).
COMPUTE
    DATESTAR = DATE.DMY(#DD, #MM, #YY).
END IF.
DO IF NOT MISSING(FIRSTDAY).
COMPUTE
    #DD = TRUNC(FIRSTDAY / 1000000).
COMPUTE
    #RM = MOD(FIRSTDAY, 1000000).
COMPUTE
    #MM = TRUNC(#RM / 10000).
COMPUTE
    #YY = MOD(#RM, 10000).
COMPUTE
    FIRSTDAY = DATE.DMY(#DD, #MM, #YY).
END IF.


>>>>>>>   fileSps.Write(genDateTimeFields.GetTimeFormats(hashSps));  //5th Block in Orig SPS - FORMATS (TIME)


FORMATS
DATESTAR, FIRSTDAY (EDATE10).
FORMATS
TIMESTAR, DIALTIME (TIME10.0).
        >>>>>>>   fileSps.Write(genLabelsAndValues.GetMissingValues(hashSps));                             //6th Block in Orig SPS - MISSING VALUES
MISSING VALUES
		APPOINTT            	(8, 9)
		NROFCALL            	(98, 99)
		FIRSTDAY            	(99999998, 99999999)
		DAYNUMBE            	(998, 999)
		DIALTIME            	(99999998, 99999999)
		PREM1               	(99999999999999999999999999999999998, 99999999999999999999999999999999999)
		POSTCODE            	(9999998, 9999999).
>>>>>>>   fileSps.Write(genLabelsAndValues.GetVarLabels(hashSps, dr2));                            //7th Block in Orig SPS - VAR LABELS
VAR LABELS
		APPOINTT               	'When can we call you back ?'
		DATESTAR               	'Start date'
		TIMESTAR               	'Start time'
		NROFCALL               	'Number of calls'
		FIRSTDAY               	'Date first call'
		DAYNUMBE               	'Day number relative to FirstDay'
		DIALTIME               	'Time of last dial in call'
		PREM1                  	'Address Field 1'
		POSTCODE               	'Address Field 7 - Postcode '.
>>>>>>>   fileSps.Write(genLabelsAndValues.GetValueLabels(hashSps, dr2));                          //8th Block in Orig SPS - VALUE LABELS
VALUE LABELS
		APPOINTT               	1 'No preference'
		                    	2 'Appointment for date and time'
		                    	3 'Preference for a period'
		                    	4 'Preference for days of the week'.
 >>>>>>>   fileSps.Write(genLabelsAndValues.GetAddValueLabels(hashSps));                            //9th Block in Orig SPS - ADD VALUE LABELS
ADD VALUE LABELS
DATESTAR            			99999998 'Refusal'
		                    	99999999 'Don''t Know'
		/POSTCODE            			                    	9999998 'Refusal'
		                    	9999999 'Don''t Know'
		/FIRSTDAY            			                    	99999998 'Refusal'
		                    	99999999 'Don''t Know'.
SAVE /OUTFILE 
'C:\dde_SPS_org\OPN1904A.SAV'.
*/
using DDE;
using StatNeth.Blaise.API.DataLink;
using StatNeth.Blaise.API.DataRecord;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using static DDE.CommonDDE;
using static DDE.Constants;

namespace BlaiseDataDelivery
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Start DDE : " + DateTime.Now);
            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();

            string spsDirectory = @"C:\dde_sps_output\";
            string dbixFileName = @"C:\aSurvey\opn-jan-b5\OPN2001A.bdix";
            string fileSpsName = Path.Combine(spsDirectory, $"{Path.GetFileNameWithoutExtension(dbixFileName)}.SPS");
            
            MainDDE(dbixFileName, fileSpsName);

            string savDir = Path.GetDirectoryName(fileSpsName);
            // Run the following to validate the SPS and create the SAV file, add -h for help i.e. use -o and -e for error handling to output file
            // For additional sav file logging add '-o {savDir}\out.txt -e {savDir}\errors.txt' to the following
            string strCmdCreateSAVFile = $@"/c C:\Apps\PSPP\bin\pspp.exe {fileSpsName} -o {savDir}\out.txt";
            try
            {
                if(CreateSAVFile) Process.Start("cmd.exe", strCmdCreateSAVFile);
            }
            catch (Exception)
            {
                throw;
            }

            stopWatch.Stop();
            TimeSpan ts = stopWatch.Elapsed;
            Console.WriteLine("End DDE " + string.Format("{0:00}:{1:00}:{2:00}.{3:00}",
                ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10));
        }

        public static void MainDDE(string dbixSurveyFile, string fileSpsName)
        {
            System.Console.WriteLine($"Start processing survey {dbixSurveyFile} at {System.DateTime.Now} out to {fileSpsName}");
            
            IDataLink2 dl2 = (IDataLink2)DataLinkManager.GetDataLink(dbixSurveyFile);
            IDataSet ds2 = dl2.Read("");
            IDataRecord2 dr2 = (IDataRecord2)ds2.ActiveRecord;

            StreamWriter fileSps = new StreamWriter(fileSpsName);

            (HashSpsUniqFldList hashSps, int totalFieldWidth) = FieldList.GetFieldList(dr2, aFilterFields);

            if(hashSps.UniqFldList.Count() > 0)
            {
                // HEADER
                SpsHeader spsFooter = new SpsHeader();
                fileSps.Write(spsFooter.ComposeSpsHeader(fileSpsName, totalFieldWidth.ToString()));     //1st Block in Orig SPS - HEADER (File info)

                GetLabelAndValue genLabelsAndValues = new GetLabelAndValue();
                GetDateTimeFormat genDateTimeFields = new GetDateTimeFormat();

                fileSps.Write(FieldList.SpsFldList(hashSps));
                fileSps.Write(genDateTimeFields.SpsDates(hashSps));
                fileSps.Write(genDateTimeFields.GetDateFormats(hashSps));            //4th Block in Orig SPS - FORMATS (DATE)
                fileSps.Write(genDateTimeFields.GetTimeFormats(hashSps));            //5th Block in Orig SPS - FORMATS (TIME)
                fileSps.Write(genLabelsAndValues.GetMissingValues(hashSps));         //6th Block in Orig SPS - MISSING VALUES
                fileSps.Write(genLabelsAndValues.GetVarLabels(hashSps, dr2));        //7th Block in Orig SPS - VAR LABELS
                fileSps.Write(genLabelsAndValues.GetValueLabels(hashSps, dr2));      //8th Block in Orig SPS - VALUE LABELS
                fileSps.Write(genLabelsAndValues.GetAddValueLabels(hashSps));        //9th Block in Orig SPS - ADD VALUE LABELS

                fileSps.Write(Constants.SPSFooter(fileSpsName.Replace("SPS", "SAV")));
                fileSps.Close();

                //Encrypt_DDE_File(fileSpsName);
                SpsAsciiFile.CreateAsciiRemarks(hashSps, dbixSurveyFile, fileSpsName);
                System.Console.WriteLine($"End processing survey {fileSpsName}*.SPS at {System.DateTime.Now}");
            }
        }

    }
}
