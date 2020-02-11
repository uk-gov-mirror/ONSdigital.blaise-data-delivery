namespace DDE
{
    public static class Constants
    {
        // The following only used to debug issues with formating and data.
        public const bool DebugDDE = false;
        public static string[] aFilterFields = {//"WEEKDAYS"
                                        //"APPOINTT","DMNAME1","DMAGE1",
                                        //"DATESTAR","WHOMADE",
                                        //"PREM1",
                                        //"POSTCODE"
            };

        public const bool DebugFilterDuplicateFields = false; // Process all occurances of Filter fields = false or true, if you only want a single occurance of a field that appears more than once
        public const string DebugOutputFieldsAfter = "DATA MODEL";
        public const string DebugOutputFieldsBefore = "DMN AME";
            public const int DebugNumberFieldNumber = 0;
            public static int DebugNumberFieldsToShow = 20;
            public static bool DebugFieldAfterThis = false;
        public const int DebugNumberOfCasesToSample = 27;

        public const bool CreateSAVFile = true;
        public static bool OutputFullFieldName = false;  // Full field or trimmed and uppered to SpsFieldMaxLen
        public const int SpsFieldMaxLen = 8;
        public const int SetLenDefault = 20;
        public const int DateFieldLength = 8;
        public const int DefaultFieldLengthIfNoneFound = 254;
        public const int MinFieldLen = 7;
        public const int MaxFieldLen = 40;
        public const string FieldListTypePadding = "      ";
        public const string DDEJulianDate = "14121585";
        public static string[] dateFormats = { "ddMMyyyy", "ddMMyy", "dd-MM-yyyy", "dd-MM-yy", "dd/MM/yyyy", "dd/MM/yy", "dd.MM.yy", "dd.MM.yyyy" };

        public static class SPSHeader
        {
            public const string Title = "TITLE ";
            public const string FileHandle = "FILE HANDLE ";
            public const string FileHandleNameText = " /NAME = ";
            public const string RecordLength = "/LRECL = ";
            public const string DataListFileNameText = "DATA LIST FILE = ";
        }

        public static string SPSFooter(string fileName)
        {
            return @$"SAVE / OUTFILE
    '{fileName}'.";
        }

        public static class SPS
        {
            public static readonly string Refused = CommonDDE.RepeatString('9', 122) + "8"; //TODO - check what upper max should be - 122 only used because SPSS enable to cope with wider file
            public static readonly string DontKnow = CommonDDE.RepeatString('9', 122);
        }
    }
}