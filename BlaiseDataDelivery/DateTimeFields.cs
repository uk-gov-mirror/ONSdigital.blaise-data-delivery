using StatNeth.Blaise.API.Meta;
using System;
using System.IO;
using System.Linq;
using System.Text;
using static DDE.CommonDDE;

namespace DDE
{
    public class GetDateTimeFormat 
    {
        public string SpsDates(HashSpsUniqFldList hashSps)
        {
            StringBuilder builder = new StringBuilder();

            foreach (var item in hashSps.UniqFldList.Where(x => x.TypeStructure == TypeStructure.Date))
            {
                string fieldUpperCase = item.spsFieldName;
                builder.Append("DO IF NOT MISSING(" + fieldUpperCase + ")." + Environment.NewLine);
                builder.Append("COMPUTE" + Environment.NewLine);
                builder.Append("    #DD = TRUNC(" + fieldUpperCase + " / 1000000)." + Environment.NewLine);
                builder.Append("COMPUTE" + Environment.NewLine);
                builder.Append("    #RM = MOD(" + fieldUpperCase + ", 1000000)." + Environment.NewLine);
                builder.Append("COMPUTE" + Environment.NewLine);
                builder.Append("    #MM = TRUNC(#RM / 10000)." + Environment.NewLine);
                builder.Append("COMPUTE" + Environment.NewLine);
                builder.Append("    #YY = MOD(#RM, 10000)." + Environment.NewLine);
                builder.Append("COMPUTE" + Environment.NewLine);
                builder.Append("    " + fieldUpperCase + " = DATE.DMY(#DD, #MM, #YY)." + Environment.NewLine);
                builder.Append("END IF" + Environment.NewLine + "." + Environment.NewLine);
            }

            return builder.ToString();
        }

        public string GetDateFormats(HashSpsUniqFldList hashSps)
        {
            StringBuilder builder = new StringBuilder();

            var a = hashSps.UniqFldList.Where(x => x.TypeStructure == TypeStructure.Date).Select(x => x.spsFieldName).ToArray();
            
            foreach (var batch in a.Batch(22).Select(i => i.Join(",")).ToArray())
            {
                builder.Append("FORMATS" + Environment.NewLine);
                builder.Append(batch + " (EDATE10)." + Environment.NewLine);
            }
            return builder.ToString();
        }

        public string GetTimeFormats(HashSpsUniqFldList hashSps)
        {
            StringBuilder builder = new StringBuilder();

            var a = hashSps.UniqFldList.Where(x => x.TypeStructure == TypeStructure.Time).Select(x => x.spsFieldName).ToArray();

            foreach (var batch in a.Batch(22).Select(i => i.Join(",")).ToArray())
            {
                builder.Append("FORMATS" + Environment.NewLine);
                builder.Append(batch + " (TIME10.0)." + Environment.NewLine);
            }

            return builder.ToString();
        }
    }
}
