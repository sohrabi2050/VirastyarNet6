// Author: Omid Kashefi, Mehrdad Senobari 
// Created on: 2010-March-08
// Last Modified: Omid Kashefi, Mehrdad Senobari at 2010-March-08
//

using System.Diagnostics;

// http://stackoverflow.com/questions/93744/most-common-c-bitwise-operations

namespace SCICT.Utility.Extensions
{
    /// <summary>
    /// <example>
    /// SomeType value = SomeType.A; 
    /// bool isGrapes = value.Is(SomeType.A); //true 
    /// bool hasGrapes = value.Has(SomeType.A); //true 

    /// value = value.Set(SomeType.B); 
    /// value = value.Set(SomeType.C); 
    /// value = value.Clear(SomeType.A); 

    /// bool hasB = value.Has(SomeType.A); //true 
    /// bool isB = value.Is(SomeType.B); //false 
    /// bool hasA = value.Has(SomeType.A); //false
    ///</example>
    /// </summary>
    public static class EnumExtensions
    {
        public static bool Has<T>(this Enum type, T value)
        {
            #region Check T
            if (!typeof(T).IsEnum)
            {
                throw new NotSupportedException("T must be an Enum");
            }
            #endregion

            try
            {
                return (((int)(object)type & (int)(object)value) == (int)(object)value);
            }
            catch
            {
                return false;
            }
        }

        public static bool Is<T>(this Enum type, T value)
        {
            #region Check T
            if (!typeof(T).IsEnum)
            {
                throw new NotSupportedException("T must be an Enum");
            }
            #endregion

            try
            {
                return (int)(object)type == (int)(object)value;
            }
            catch
            {
                return false;
            }
        }
        
        public static T Set<T>(this Enum type, T value)
        {
            #region Check T
            if (!typeof(T).IsEnum)
            {
                throw new NotSupportedException("T must be an Enum");
            }
            #endregion

            try
            {
                return (T)(object)(((int)(object)type | (int)(object)value));
            }
            catch (Exception ex)
            {
                throw new ArgumentException(
                    string.Format(
                        "Could not append value from enumerated type '{0}'.", typeof(T).Name), ex);
            }
        }

        public static T Clear<T>(this Enum type, T value)
        {
            #region Check T
            if (!typeof(T).IsEnum)
            {
                throw new NotSupportedException("T must be an Enum");
            }
            #endregion

            try
            {
                return (T)(object)(((int)(object)type & ~(int)(object)value));
            }
            catch (Exception ex)
            {
                throw new ArgumentException(
                    string.Format("Could not remove value from enumerated type '{0}'.", typeof(T).Name), ex);
            }
        }
    }
}
