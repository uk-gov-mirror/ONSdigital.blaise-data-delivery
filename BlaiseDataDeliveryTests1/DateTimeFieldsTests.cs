using Microsoft.VisualStudio.TestTools.UnitTesting;
using StatNeth.Blaise.Administer.Deploy.DataLayer.DataObjects;
using StatNeth.Blaise.API.DataRecord;
using StatNeth.Blaise.API.Meta;
using System.Collections.Generic;
using static DDE.CommonDDE;
using static DDE.FieldList;

namespace DDE.Tests
{
    [TestClass()]
    public class DateTimeFieldsTests
    {
        [TestMethod()]
        public void SpsFldListTest()
        {
            HashSet<SpsFieldProperties> hs = new HashSet<SpsFieldProperties>(new SpsFldComparer());
            HashSpsUniqFldList hsUniqFields = new HashSpsUniqFldList(hs);

            string tstLocalFieldName = "AppointType";

            string spsFieldName = tstLocalFieldName.Replace(".", "").Replace("[", "").Replace("]", "").ToUpper().ToString();

            ITypeJC tt = new TypeTest()
            {
                MaxLength = 10,
                Name = "WeekDays",
                Structure = TypeStructure.Set
            };


            SpsFieldProperties ts1 = new SpsFieldProperties
            {
                spsFieldName = spsFieldName,
                MaxLen = 10,
                TypeStructure = TypeStructure.String,
                FullName = "CatiMana.CatiAppoint.AppointType",
                FldTypeTag = "",
                LeftRightJustify = "R"

            };
            // testField.TypeStructure, spsFieldName, testField.FullName), ref hsUniqFields

            hsUniqFields = AddUniqFld(BuildSpsField(ts1.TypeStructure,ts1.spsFieldName,ts1.MaxLen,ts1.spsFieldName), ref hsUniqFields);
            
            Assert.IsTrue(hsUniqFields.UniqFldList.Contains( > 0);
            Assert.IsTrue(hsUniqFields.UniqFldList.Count > 0);
        }
    }
}