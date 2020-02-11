// SPS header
/*
 
TITLE		'OPN2001a'.
FILE HANDLE OPN2001A /NAME = 
'D:\_DATADELIVERY\OPN2001A.ASC'
/LRECL = 105236.
DATA LIST FILE = OPN2001A
/

*/
using System;
using System.IO;
using static DDE.Constants;

namespace BlaiseDataDelivery
{
    public class SpsHeader
    {
        public string ComposeSpsHeader(string fullFile, string fieldsCount)
        {
            string filename = Path.GetFileNameWithoutExtension(fullFile);
            string boilerplate = SPSHeader.Title + filename + "." + Environment.NewLine;
            boilerplate += SPSHeader.FileHandle + " " + filename + SPSHeader.FileHandleNameText + Environment.NewLine;
            boilerplate += "'" + fullFile.Replace("SPS","ASC") + "'" + Environment.NewLine;
            boilerplate += SPSHeader.RecordLength + fieldsCount + "." + Environment.NewLine;
            boilerplate += SPSHeader.DataListFileNameText + filename + Environment.NewLine;
            boilerplate += "/" + Environment.NewLine;

            return boilerplate;
        }
    }
}
