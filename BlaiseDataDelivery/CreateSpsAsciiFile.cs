using StatNeth.Blaise.API.DataLink;
using StatNeth.Blaise.API.DataRecord;
using StatNeth.Blaise.API.Meta;
using System;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using static DDE.CommonDDE;
using static DDE.Constants;

namespace DDE
{
    public static class SpsAsciiFile
    {
        static void WriteAsciiLine(HashSpsUniqFldList spsFldList, IDataRecord2 dr, ref StreamWriter fileSpsAsc)
        {
            StringBuilder perLine = new StringBuilder("");
            StringBuilder line = new StringBuilder("");
            foreach (var fld in spsFldList.UniqFldList.ToArray())
            {
                string val = dr.GetField(fld.FullName).DataValue.ValueAsText;
                if (val.Length > fld.MaxLen)
                {
                    // Console.WriteLine(val + " too long by "+ (val.Length - fld.MaxLen) + " - chopped for field " + fld.spsFieldName);
                    // Trim data to the FieldMaxLen - TODO need to address this with user.
                    val = val.Substring(0, fld.MaxLen);
                }

                if (fld.TypeStructure == TypeStructure.Set && val.Length > fld.MaxLen)
                {
                    Console.WriteLine(fld.FullName + " val is too long by val " + val + " - will be trimmed : " + val.Substring(0,1));
                    val = val.Substring(0, 1);
                }
                if (fld.TypeStructure == TypeStructure.Date)
                {
                    try
                    {
                        string[] formats = {"ddMMyyyy","ddMMyy","dd-MM-yyyy","dd-MM-yy","dd/MM/yyyy","dd/MM/yy","dd.MM.yy","dd.MM.yyyy"};
                        DateTime dateValue;

                        if(DateTime.TryParseExact(val, formats,
                                                       new CultureInfo("en-US"),  // TODO - need to check Date culture
                                                       DateTimeStyles.None,
                                                       out dateValue))
                        {
                            // JulianCalendar jc = new JulianCalendar();
                            DateTime myDate = new DateTime(dateValue.Year, dateValue.Month, dateValue.Day, 0, 0, 0, 0);
                            val = myDate.ToString("ddMMyyyy");
                        }
                    }
                    catch
                    {
                        Console.WriteLine("DATE TRYPARSE FAILED FOR " + fld.spsFieldName);
                    }
                }

                // Right, Left justify data in ASC and pad values to the correct field width.
                // THIS IS IMPORTANT, to make the SPS work with the ASC - otherwise data will not be in the correct fixed locations.
                val = fld.LeftRightJustify == "R" ? val.PadLeft(fld.MaxLen) : val.PadRight(fld.MaxLen);

                line.Append(val);
            }

            fileSpsAsc.WriteLine(line);
        }

        public static bool CreateAsciiRemarks(HashSpsUniqFldList spsFldList,string dbixFileName, string dbixSurveyFile)
        {
            int caseCounter = 0;
            string fileSpsAscName = Path.ChangeExtension(dbixSurveyFile, "ASC");
            string fileRemarksName = Path.ChangeExtension(dbixSurveyFile, "RMK");

            StreamWriter fileSpsAsc = new StreamWriter(fileSpsAscName);
            //StreamWriter fileSpsRmk = new StreamWriter(fileRemarksName);

            IDataLink2 dl2 = (IDataLink2)DataLinkManager.GetDataLink(dbixFileName);
            //IDataLink3 dl3 = (IDataLink3)DataLinkManager.GetDataLink(dbixFileName);
            IDataSet ds2 = dl2.Read("");
            //IDataSet ds3 = dl3.Read("");ls 
            for (int i = 1; i < 7; i++ )  //TODO - LIMIT TO 3 cases
            while (!ds2.EndOfSet)
            {
                caseCounter++;
                    if (DebugDDE) if (caseCounter > DebugNumberOfCasesToSample) break;
                // Each dataset is a Survey Case
                IDataRecord2 dr2 = (IDataRecord2)ds2.ActiveRecord;
                //IDataRecord3 dr3 = (IDataRecord3)ds3.ActiveRecord;
                if (spsFldList.UniqFldList.Count > 0) { 
                    WriteAsciiLine(spsFldList, dr2, ref fileSpsAsc);
                    //WriteRemarks(spsFldList, dr2, ref fileSpsRmk);
                    //fileSpsRmk.Close();
                    //Encrypt_DDE_File(fileRemarksName);
                }
                ds2.MoveNext();
            }
            fileSpsAsc.Close();
            Encrypt_DDE_File(fileSpsAscName);
            Console.WriteLine($"{caseCounter} cases processed!");
            return true;
        }

        public static int CreateRemarksFile(HashSpsUniqFldList spsFldList, string dbixFileName, string dbixSurveyFile)
        {
            int caseCounter = 0;
            string fileRemarksName = Path.ChangeExtension(dbixSurveyFile, "RMK");

            StreamWriter fileSpsRmk = new StreamWriter(fileRemarksName);

            IDataLink3 dl3 = (IDataLink3)DataLinkManager.GetDataLink(dbixFileName);
            IDataSet ds3 = dl3.Read("");
            while (!ds3.EndOfSet)
            {
                caseCounter++;
                if (DebugDDE) if (caseCounter > DebugNumberOfCasesToSample) break;
                // Each dataset is a Survey Case
                IDataRecord3 dr3 = (IDataRecord3)ds3.ActiveRecord;
                if (spsFldList.UniqFldList.Count > 0)
                {
                    WriteRemarks(spsFldList, dr3, ref fileSpsRmk);
                }
                ds3.MoveNext();
            }
            fileSpsRmk.Close();
            Encrypt_DDE_File(fileRemarksName);
            return caseCounter;
        }
        static void WriteRemarks(HashSpsUniqFldList spsFldList, IDataRecord2 dr2, ref StreamWriter fileRemarksName)
        {
            StringBuilder line = new StringBuilder("");
            string remarks = @"c:\rem\remarks.csv";
            if (File.Exists(remarks))
            {
                var opnFields = File.ReadAllLines(remarks)
                        .Where(a => a.StartsWith("OPN"))
                        .Select(b => b.Split(','));

                line.Append(string.Join(",", opnFields.Select(header => header[2])) + Environment.NewLine); // Add headers
                foreach (var fld in spsFldList.UniqFldList.ToArray())
                {
                    string remark = dr2.GetField(fld.FullName).FieldProperties.GetItem("remark").Value.StringValue;

                    if (!string.IsNullOrEmpty(remark))
                    {
                        foreach(var f in opnFields)
                        {
                            if (f[1] == "*")
                            {
                                switch (f[2])
                                {
                                    case ("BLOCK"):
                                        string blk = fld.FullName.Contains(".") ? fld.FullName.Substring(0, fld.FullName.IndexOf('.') - 1) : fld.FullName;
                                        line.Append(blk + ",");
                                        break;
                                    case ("FIELD"):
                                        line.Append(fld.spsFieldName + ",");
                                        break;
                                    case ("RESPONSE"):
                                        line.Append(fld.MaxLen + ",");
                                        break;
                                    case ("FIELDTEXT"):
                                        line.Append(dr2.GetField(fld.FullName).DataValue.ValueAsText + ",");
                                        break;
                                    case ("REMARKTEXT"):
                                        line.Append(remark);
                                        break;
                                    default:
                                        break;
                                }
                            }
                            else
                            {
                                line.Append(dr2.GetField(f[1]).DataValue.ValueAsText + ",");
                            }
                        }
                        line.Append(Environment.NewLine);
                    }
                }
                fileRemarksName.WriteLine(line);
            }
        }

    }
}