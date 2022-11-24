// Author: Omid Kashefi, Mehrdad Senobari 
// Created on: 2010-March-08
// Last Modified: Omid Kashefi, Mehrdad Senobari at 2010-March-08
//

using System.Diagnostics;

// http://stackoverflow.com/questions/93744/most-common-c-bitwise-operations

namespace SCICT.Utility.Extensions
{
    public static class StringExtensions
    {
        public static T ToEnum<T>(this string value)
        {
            #region Check T
            if (!typeof(T).IsEnum)
            {
                throw new NotSupportedException("T must be an Enum");
            }
            #endregion

            string[] values = value.Split(new char[] {',', ' '}, StringSplitOptions.RemoveEmptyEntries);
            Debug.Assert(value.Length >= 1);

            T enumVal;
            try
            {
                enumVal = (T) Enum.Parse(typeof (T), values[0], true);

                string enumStr;
                for (int i = 1; i < values.Length; i++)
                {
                    enumStr = values[i];
                    enumVal = (T) (enumVal as Enum).Set<T>((T) Enum.Parse(typeof (T), enumStr));
                }
            }
            catch (Exception)
            {
                enumVal = (T) (object) 0;
            }


            return enumVal;
            //return (T)Enum.Parse(typeof(T), value);
        }

        public static bool StartsWith(this string value, string[] array)
        {
            foreach (string arrStr in array)
            {
                if (value.StartsWith(arrStr))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool EndsWith(this string value, string[] array)
        {
            foreach (string arrStr in array)
            {
                if (value.EndsWith(arrStr))
                {
                    return true;
                }
            }

            return false;
        }

        public static bool IsIn(this string value, string[] array)
        {
            foreach (string arrStr in array)
            {
                if (value == arrStr)
                {
                    return true;
                }
            }

            return false;
        }

        public static bool FindsIn(this string value, string[] array)
        {
            foreach (string arrStr in array)
            {
                if (arrStr.Contains(value))
                {
                    return true;
                }
            }

            return false;
        }

        public static string[] ToStringArray(this string value)
        {
            char[] charArray = value.ToCharArray();

            List<string> strList = new List<string>();

            foreach(char c in charArray)
            {
                strList.Add(c.ToString());
            }

            return strList.ToArray();
        }
    }
}
