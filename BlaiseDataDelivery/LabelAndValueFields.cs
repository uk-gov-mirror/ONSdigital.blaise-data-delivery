using StatNeth.Blaise.API.DataRecord;
using StatNeth.Blaise.API.Meta;
using StatNeth.Blaise.API.Meta.Constants;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using static DDE.CommonDDE;
using static DDE.Constants;

namespace DDE
{
    public class GetLabelAndValue 
    {
        public string GetMissingValues(HashSpsUniqFldList hashSps)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("MISSING VALUES" + Environment.NewLine);

            var aMissingVals = hashSps.UniqFldList.Where(x => x.TypeStructure != TypeStructure.Date).ToArray();

            foreach (var item in aMissingVals)
            {
                string actualValue;
                actualValue = "(" + CommonDDE.RightExt(SPS.Refused, item.MaxLen) + ", " + CommonDDE.RightExt(SPS.DontKnow, item.MaxLen) + ")";

                builder.AppendFormat("{0,-10} {1,-50} {2,0}", " ", item.spsFieldName, actualValue);
                builder.AppendFormat(Environment.NewLine);
            }
            return FormatBlockEnd(builder).ToString();
        }
        public string GetVarLabels(HashSpsUniqFldList hashSps, IDataRecord2 dr2)
        {
            StringBuilder builder = new StringBuilder();
            string ItemName = string.Empty;
            builder.Append("VAR LABELS" + Environment.NewLine);

            foreach (var item in hashSps.UniqFldList)
            {
                IFieldInformation fi = dr2.Datamodel.GetField(item.FullName);
                IRoleTextsCollection rtc = fi.RoleTexts;

                if (rtc.Contains(TextRoles.Question))
                {
                    ItemName = item.spsFieldName;
                    IRoleTexts qt = rtc.GetItem(TextRoles.Question);
                    string questionTextString = !string.IsNullOrEmpty(FilterWhiteSpaces(qt.Texts[0].Text)) ? FilterWhiteSpaces(qt.Texts[0].Text) : ItemName ;

                    questionTextString = "'" + CleanString(questionTextString) + "'";

                    builder.AppendFormat("{0,-10} {1,-50} {2,-20}", " ", ItemName, questionTextString.Replace("\r\n", " "));
                    builder.Append(Environment.NewLine);
                }
            }

            return FormatBlockEnd(builder).ToString();
        }
        public string GetAddValueLabels(HashSpsUniqFldList hashSps)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("ADD VALUE LABELS" + Environment.NewLine);

            foreach (var item in hashSps.UniqFldList.Where(x => x.TypeStructure == TypeStructure.Date))
            {
                builder.AppendFormat(" ".PadLeft(18));
                builder.Append( "/" + item.spsFieldName + Environment.NewLine);

                string refusedString = CommonDDE.RightExt(SPS.Refused, item.MaxLen);
                string dontknowString = CommonDDE.RightExt(SPS.DontKnow, item.MaxLen);
                // Thur - a . but no / to finish a section.
                builder.AppendFormat("{0,0}".PadLeft(40), refusedString + " 'Refusal'" + Environment.NewLine);
                builder.AppendFormat("{0,0}".PadLeft(40), dontknowString + " 'Dont Know'" + Environment.NewLine);
            }
            builder = FormatBlockEnd(builder);

            return builder.ToString();
        }
        public string GetValueLabels(HashSpsUniqFldList hashSps, IDataRecord2 dr2)
        {
            StringBuilder builder = new StringBuilder();
            string builtString;
            builder.Append("VALUE LABELS" + Environment.NewLine);


            foreach (var item in hashSps.UniqFldList.Where(x => x.TypeStructure == TypeStructure.Enumeration))
            {
                IFieldInformation fi = dr2.Datamodel.GetField(item.FullName);
                IEnumerable<ICategory> categories = fi.Type.Categories;
                builder.AppendFormat("{0, 18}", item.spsFieldName);

                var lastCategoryItem = categories.Last();
                foreach (var category in categories)
                {
                    builtString = (category.Texts.Any() && category.Texts != null ? category.Texts[0].Text : category.Name);
                    builtString = CleanString(builtString);

                    builder.AppendFormat("{0,33} '", category.Code.ToString());
                    builder.AppendFormat("{0,0}'", builtString);

                    if (category.Equals(lastCategoryItem)) { builder.Append("/"); }
                    builder.Append(Environment.NewLine);
                }
            }

            return FormatBlockEnd(builder).ToString();
        }
        private static string CleanString(string str)
        {
            str = str.Trim('\'');
            str = str.Trim('\"');
            
            if (str.Contains("\'")) str = str.Replace("\'", "`");
            if (str.Contains("\r\n")) str = str.Replace("\r\n", "");
            if (str.Contains("\\")) str = str.Replace("\\", "/");
            if (str.Contains("  ")) str = Regex.Replace(str, "[ ]{2,}", " ");
            if (str.Contains("^")) str = str.Replace("^","");
            if (str.Contains("@B")) str = str.Replace("@B","");
            if (str.Contains("@")) str = str.Replace("@","");
            if (str.Contains("^")) str = str.Replace("^","");
            if (str.Contains("(")) str = str.Replace("(","");
            if (str.Contains(")")) str = str.Replace(")","");
            if (str.Length > 104) str = str.Substring(0, 104);
            if (str.Contains("\"\"")) str = str.Replace("\"\"", "");
            return str;
        }
        private static string FilterWhiteSpaces(string input)
        {
            if (input == null)
                return string.Empty;

            StringBuilder stringBuilder = new StringBuilder(input.Length);
            for (int i = 0; i < input.Length; i++)
            {
                char c = input[i];
                if (i == 0 || c != ' ' || (c == ' ' && input[i - 1] != ' '))
                    stringBuilder.Append(c);
            }
            return stringBuilder.ToString();
        }
    }
}
