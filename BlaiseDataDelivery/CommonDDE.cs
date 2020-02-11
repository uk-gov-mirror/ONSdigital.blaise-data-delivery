using StatNeth.Blaise.API.DataRecord;
using StatNeth.Blaise.API.Meta;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PgpCore;

namespace DDE
{
    public static class CommonDDE
    {
        public class TypeTest : ITypeJC
        {

            public int? MaxLength { get; set; }

            // IType MemberType { get; }

            public string Name { get; set; }

            public TypeStructure Structure { get; set; }
        }
        public interface ITypeJC
        {
            int? MaxLength { get; set; }
           // IType MemberType { get; }
            string Name { get; set; }
            TypeStructure Structure { get; set; }
        }

        public class SpsFieldProperties
        {
            public string spsFieldName { get; set; }
            public int MaxLen { get; set; }
            public TypeStructure TypeStructure { get; set; }
            public string FullName { get; set; }
            public string FldTypeTag { get; set; }
            public string LeftRightJustify { get; set; }
        }
        public class HashSpsUniqFldList : IEqualityComparer<SpsFieldProperties>
        {
            public HashSpsUniqFldList(HashSet<SpsFieldProperties> uniqFldList)
            {
                UniqFldList = uniqFldList;
            }

            public HashSet<SpsFieldProperties> UniqFldList { get; set; }

            public bool Equals(SpsFieldProperties x, SpsFieldProperties y)
            {
                throw new NotImplementedException();
            }

            public int GetHashCode(SpsFieldProperties obj)
            {
                throw new NotImplementedException();
            }
        }
        public class SpsFldComparer : IEqualityComparer<SpsFieldProperties>
        {
            public bool Equals(SpsFieldProperties x, SpsFieldProperties y)
            {
                return x.spsFieldName.Equals(y.spsFieldName, StringComparison.InvariantCultureIgnoreCase);
            }

            public int GetHashCode(SpsFieldProperties obj)
            {
                return obj.spsFieldName.GetHashCode();
            }
        }

        /// <summary>
        /// Repeats a char for a specified amount of times
        /// </summary>
        /// <param name="blaiseField"></param>
        /// <returns></returns>
        public static string RepeatString(this char c, int n)
        {
            return new String(c, n);
        }

        public static string RightExt(this string value, int length)
        {
            ///This takes a string value and gets the right most characters based on its length
            ///

            //Check if the value is valid
            if (string.IsNullOrEmpty(value))
            {
                //Set valid empty string as string could be null
                value = string.Empty;
            }
            else if (value.Length > length)
            {
                //Make the string no longer than the max length
                value = value.Substring(value.Length - length, length);
            }

            return value;
        }

        public static IEnumerable<IEnumerable<T>> Batch<T>(this IEnumerable<T> source, int size)
        {
            if (size <= 0)
                throw new ArgumentOutOfRangeException("size", "Must be greater than zero.");
            using (var enumerator = source.GetEnumerator())
                while (enumerator.MoveNext())
                {
                    int i = 0;
                    // Batch is a local function closing over `i` and `enumerator` that
                    // executes the inner batch enumeration
                    IEnumerable<T> Batch()
                    {
                        do yield return enumerator.Current;
                        while (++i < size && enumerator.MoveNext());
                    }

                    yield return Batch();
                    while (++i < size && enumerator.MoveNext()) ; // discard skipped items
                }
        }
        public static StringBuilder FormatBlockEnd(StringBuilder str)
        {
            if (str.Length > 3) str =
                    str.Replace("/", "", str.Length - 3, 3)
                        .Remove((str.Length - 2), 2)
                        .Append(Environment.NewLine)
                        .Append(".")
                        .Append(Environment.NewLine);
            return str;
        }
        public static string Join<T>(this IEnumerable<T> s, string j)
        {
            return string.Join(j, s.ToArray());
        }


        public static void Encrypt_DDE_File(string file){
            //using (PGP pgp = new PGP()) {
            //    pgp.EncryptFile(file, file + ".gpg", "C:\\dde\\BlaiseDataDelivery\\key.gpg");
            //}
        }
    }
}
    