using StatNeth.Blaise.API.DataRecord;
using StatNeth.Blaise.API.Meta;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using static DDE.CommonDDE;
using static DDE.Constants;

namespace DDE
{
    public static class FieldList
    {
    
        // Build up the full field list Block
        public static string SpsFldList(HashSpsUniqFldList hashSps)
        {
            int fieldStartPos = 0;

            StringBuilder sbFldListSps = new StringBuilder();
            // Calculate the start/end position of fields - used by associated fixed format ASC file
            foreach (var item in hashSps.UniqFldList)
            {
                int fieldEndPos = fieldStartPos + item.MaxLen;
                var formatedLine = FldListLineFormater(item.spsFieldName, fieldStartPos + 1, fieldEndPos, item.FldTypeTag);
                fieldStartPos = fieldEndPos;
                sbFldListSps.Append(formatedLine);
            }
            return (FormatBlockEnd(sbFldListSps)).ToString();
        }

        // Create a unique hashset of fields and their properties - used as the main driver to define the SPS meta data
        public static (HashSpsUniqFldList fldList, int totalFieldWidth) GetFieldList(IDataRecord2 dr2, string[] aFilterFields)
        {
            IEnumerable<IField> apifields = dr2.GetDataFields();

            HashSet<SpsFieldProperties> hs = new HashSet<SpsFieldProperties>(new SpsFldComparer());
            HashSpsUniqFldList hsUniqFields = new HashSpsUniqFldList(hs);

            bool filterOn = aFilterFields.Count() > 0 ? true : false;

            int fldCounter = 0;
            int totalFieldWidth = 0;
            int fldFound = 0;

            // Cycle through every field 
            foreach (var field in apifields)
            {
                // Filter out none data fields like Auxilary fields
                if (field.Definition.FieldKind != FieldKind.DataField) continue;

                // if (field.Definition.Type.Structure == TypeStructure.Enumeration) continue;
                string spsFieldName = "";
                if (OutputFullFieldName)
                {
                    spsFieldName = field.FullName // Use FullName - includes parent group names
                        .Replace(".", "_")
                        .Replace("[", "").Replace("]", "");
                }
                else
                {
                    // Remove ., [] and set to upper case
                    spsFieldName = field.LocalName // Shorter Local Name
                        .Replace("[", "").Replace("]", "")
                        .Replace(".", "").ToUpper();

                    int trimLen = SpsFieldMaxLen;
                    if (field.Definition.Type.Structure == TypeStructure.String) trimLen++; // Trim extra space for set counter
                        // Trim field is longer
                    if (spsFieldName.Length > trimLen)
                    {
                        spsFieldName = spsFieldName.Substring(0, trimLen);
                    }
                };

                // 'Title' is a reserved Fortran SPS command - not allowed as a field name
                if (spsFieldName.ToUpper() == "TITLE") spsFieldName = spsFieldName+1;

                fldCounter++;

                if (DebugDDE && aFilterFields.Length == 0)
                {
                    //if (fldCounter < 111) continue;
                    //if (fldCounter > 120) break;
                    // Output fields after the field defined by DebugFieldAfter
                    if (fldFound < 1 && DebugOutputFieldsAfter.Contains(spsFieldName.ToUpper())) { 
                        DebugFieldAfterThis = true;
                        fldFound++;
                        DebugNumberFieldsToShow = DebugNumberFieldsToShow + fldCounter;
                    }

                    if (!DebugFieldAfterThis) continue;
                    if (fldCounter > DebugNumberFieldsToShow) break; // Limit number of fields to output
                }

                // Allow Field filter for debugging
                if (filterOn)
                {
                    // Only run for debug purposes - run for a predefined list of fields
                    if (aFilterFields.Any(spsFieldName.Contains))
                    {
                        // Remove from filter - so only shows first occurance of a field.
                        if (DebugFilterDuplicateFields) aFilterFields = aFilterFields.Where((source, index) => source != spsFieldName).ToArray();
                    }
                    else
                    {
                        if (aFilterFields.Count() == 0) break;  // Break if reached the end of the filter array
                        if (filterOn) continue;
                    }
                }

                int getFldLen = GetFieldProps2(field);
                // Field sets like WEEKDAYS needs to be split into fields Weekday1-7
                if (field.Definition.Type.Structure == TypeStructure.Set)
                {
                    // For short field format, remove 'S' from WEEKDAYS - so we get WEEKDAY1, WEEKDAY2 ...for the set counter
                    if (spsFieldName == "WEEKDAYS" && !OutputFullFieldName) spsFieldName = spsFieldName.Replace("S",""); 

                    for (int setSize = 1; setSize <= getFldLen; setSize++)
                    {   
                        // Set field Length equal to the number of items in the set
                        int setFldLen = getFldLen.ToString().Length;
                        totalFieldWidth += setFldLen;
                        // Add a field for each element in a set
                        hsUniqFields = AddUniqFld(BuildSpsField(field.Definition.Type.Structure, spsFieldName, setFldLen, field.FullName), ref hsUniqFields);
                    }
                }
                else // All other fields - add a unique field to the hastset
                {
                    totalFieldWidth += getFldLen;
                    hsUniqFields = AddUniqFld(BuildSpsField(field.Definition.Type.Structure, spsFieldName, getFldLen, field.FullName), ref hsUniqFields);
                }
            }
            if(DebugDDE) Console.WriteLine("Field Count from Blaise Api is : " + fldCounter);

            return (hsUniqFields, totalFieldWidth);
        }

        // Build each SPS field
        public static SpsFieldProperties BuildSpsField(TypeStructure fldStructure, string spsFldName, int fieldLen, string fieldFullName)
        {
            string fldTag = "";

            if (fldStructure == TypeStructure.String) fldTag = "  (A)";  // tag required on Alpha (string) fields
            if (fldStructure == TypeStructure.Time) fldTag = "  (TIME)"; // tag for Time fields

                return new SpsFieldProperties
            {
                spsFieldName = spsFldName, // + val + (fldType.Structure).ToString().Substring(0,1),
                MaxLen = fieldLen,
                TypeStructure = fldStructure,
                FullName = fieldFullName,
                FldTypeTag = fldTag
            };
        }

        public static int GetFieldProps2(IField fld)
        {
            IType fldDefinitionType = fld.Definition.Type;

            int intSize = 0;
            if (fldDefinitionType.MaxLength.HasValue)
            {
                intSize = (int)fldDefinitionType.MaxLength;
            }
            else if (fldDefinitionType.MaxValue.HasValue)
            {
                intSize = (fldDefinitionType.MaxValue.ToString()).Length;
            }
            else if (fldDefinitionType.ClassificationDynamicLength.HasValue)
            {
                intSize = (int)fldDefinitionType.ClassificationDynamicLength;
            }
            else if (fldDefinitionType.ClassificationKeyLength.HasValue)
            {
                intSize = (int)fldDefinitionType.ClassificationKeyLength;
            } else {
                intSize = (fldDefinitionType.Structure == TypeStructure.Date || fldDefinitionType.Structure == TypeStructure.Time) ? 8 : 0;
            }

            if (intSize > 0 // && intSize < 255
                ) return intSize;

            if (fldDefinitionType.Structure == TypeStructure.Integer ||
                fldDefinitionType.Structure == TypeStructure.Real &&
                fldDefinitionType.Decimals != null)
            {
                string[] strAttribute = { };
                if (fld.Definition.SpecialAnswers != null)
                {
                    strAttribute = fld.Definition.SpecialAnswers.ToArray();
                };
                if (fldDefinitionType.MaxValue != null)
                {
                    var intCode = Math.Pow(10, fldDefinitionType.MaxValue.ToString().Length) - 2; // 8, 98, 998 etc.
                    if (strAttribute.Contains("Refusal"))
                    {
                        if ((int)fldDefinitionType.MaxValue >= intCode)
                        {
                            intSize += 1;
                        }
                    }
                    else if (strAttribute.Contains("DontKnow"))
                    {
                        if ((int)fldDefinitionType.MaxValue > intCode)
                        {
                            intSize += 1;
                        }
                    }
                }
                else if (strAttribute.Contains("Refusal") || strAttribute.Contains("DontKnow"))
                {
                    intSize += 1;
                }
                return intSize;
            }

            int[] arrField_Decimals = { };
            //  Get number of decimals
            if (fld.Definition.Type.Decimals.HasValue)
            {
                return fld.Definition.Type.Decimals.Value;
            }

            //  Determine size of a set  
            if (fldDefinitionType.Structure == TypeStructure.Set)
            {
                if (fldDefinitionType.Cardinality.HasValue)
                {
                    return fldDefinitionType.Cardinality.Value;
                }
                else
                {
                    return fldDefinitionType.MemberType.Categories.Count;
                }
            }
            else
            {
                Console.WriteLine("No length found for : " + fld.FullName);
                return MaxFieldLen;
            }
        }

        // Return fields properties according to field Type
        public static (int FldLen, string FldTypeTag, string LeftRightJustify) GetFieldProps(IType fldDefinitionType)
        {


            switch (fldDefinitionType.Structure)
            {
                case (TypeStructure.String):
                    return (fldDefinitionType.MaxLength ?? DefaultFieldLengthIfNoneFound, FieldListTypePadding + "(A)", "L");
                case (TypeStructure.Date):
                    return (DateFieldLength, "", "L");
                case (TypeStructure.Time):
                    return (DateFieldLength, FieldListTypePadding + "(TIME)", "L");
                case (TypeStructure.Real):
                case (TypeStructure.Integer):
                case (TypeStructure.Enumeration):
                    return (fldDefinitionType.MaxValue.ToString().Length, "", "R");
                // Enum  -- todo - chekc if need to loop Enum like Sets?   return ((int)fldDefinitionType.MaxValue, "", "R");
                case (TypeStructure.Set):
                    return ((int)fldDefinitionType.MemberType.MaxValue, "", "L");
                case (TypeStructure.Array):
                    return (fldDefinitionType.MaxValue.ToString().Length, "", "L");
                case (TypeStructure.Classification):
                    return (fldDefinitionType.ClassificationKeyLength.ToString().Length, "", "L");
                default:
                    return (DefaultFieldLengthIfNoneFound, "", "L");
            }
        }

        // Add a new field to hashset (won't allow duplicates).  For duplicates field names, append a counter
        public static HashSpsUniqFldList AddUniqFld(SpsFieldProperties spsFieldProps, ref HashSpsUniqFldList hashSet)
        {
            int counter = 1;
            string fieldName = spsFieldProps.spsFieldName;

            // Add counter for Set field
            if (spsFieldProps.TypeStructure == TypeStructure.Set)
            {
                // If long names - add _1 else just 1 for short names
                if (OutputFullFieldName) spsFieldProps.spsFieldName = spsFieldProps.spsFieldName + "_" + counter;
                if (!OutputFullFieldName) spsFieldProps.spsFieldName = spsFieldProps.spsFieldName + counter;
            }

            // Add to hashSet or increment if already exists
            while (!hashSet.UniqFldList.Add(spsFieldProps))
            {
                counter += 1;
                string currentFieldName = fieldName;
                int counterLen = counter.ToString().Length;

                // Append counter if shorter than Max fld len 
                if (currentFieldName.Length + counterLen < SpsFieldMaxLen || OutputFullFieldName)
                {
                    currentFieldName = currentFieldName + "_" + counter;
                }
                else
                {
                    // otherwise trim fieldname to fit counter length
                    string subFldName = currentFieldName.Substring(0, SpsFieldMaxLen - counterLen);

                    currentFieldName = subFldName.Length + counterLen > SpsFieldMaxLen ?
                        currentFieldName.Substring(0, SpsFieldMaxLen - counterLen) :
                        subFldName + counter;
                }
                spsFieldProps.spsFieldName = currentFieldName;
            }
            return hashSet;
        }

        // Set spacings for Field list e.g. TIMESTAR    10 - 17 (TIME)
        // The spacing have no meaning
        private static string FldListLineFormater(string currFldName, int spsFldStart, int spsFldEnd, string tag)
        {
            string spacer = "        ";
            string sep1 = " - ";
            return spacer + (currFldName).PadRight(SpsFieldMaxLen + 50) + spsFldStart.ToString().PadLeft(8) + sep1 + spsFldEnd.ToString().PadLeft(6) + tag + Environment.NewLine;
        }
    }
}

