using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WpfControlLibrary1
{
    public static class EnumHelper
    {
        // var values = Enum.GetValues(typeof(myenum))
        // var values = Enum.GetNames(typeof(myenum))
        //The first will give you values in form on a array of object, and the second will give you values in form of array of String.

        //Use it in foreach loop as below:

        //foreach(var value in values)
        //{
        //    //Do operations here
        //}

        public static T[] GetValues<T>()
        {
            Type enumType = typeof(T);

            if (!enumType.IsEnum)
            {
                throw new ArgumentException("Type '" + enumType.Name + "' is not an enum");
            }

            List<T> values = new List<T>();

			IEnumerable<System . Reflection . FieldInfo> fields = from field in enumType.GetFields()
                         where field.IsLiteral
                         select field;

            foreach (System.Reflection.FieldInfo field in fields)
            {
                object value = field.GetValue(enumType);
                values.Add((T)value);
            }

            return values.ToArray();
        }

        public static object[] GetValues(Type enumType)
        {
            if (!enumType.IsEnum)
            {
                throw new ArgumentException("Type '" + enumType.Name + "' is not an enum");
            }

            List<object> values = new List<object>();

			IEnumerable<System . Reflection . FieldInfo> fields = from field in enumType.GetFields()
                         where field.IsLiteral
                         select field;

            foreach (System.Reflection.FieldInfo field in fields)
            {
                object value = field.GetValue(enumType);
                values.Add(value);
            }

            return values.ToArray();
        }

        public static List<T> GetEnumValues<T>() where T : new()
        {
            T valueType = new T();
            return typeof(T).GetFields()
                .Select(fieldInfo => (T)fieldInfo.GetValue(valueType))
                .Distinct()
                .ToList();
        }

        public static List<String> GetEnumNames<T>()
        {
            return typeof(T).GetFields()
                .Select(info => info.Name)
                .Distinct()
                .ToList();
        }

        /// <summary>
        /// Gets all items for an enum value.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">The value.</param>
        /// <returns></returns>
        public static IEnumerable<T> GetAllItems<T>(this Enum value)
        {
            return (T[])Enum.GetValues(typeof(T));
        }

        public static Dictionary<int, string> ToList<T>() where T : struct
        {
            return ((IEnumerable<T>)Enum
                .GetValues(typeof(T)))
                .ToDictionary(
                    item => Convert.ToInt32(item),
                    item => item.ToString());
        }

        public static IEnumerable<T> GetValues1<T>()
        {
            return Enum.GetValues(typeof(T)).Cast<T>();
        }

        public static System.Collections.IEnumerable getListOfEnum(Type type)
        {
            System.Reflection.MethodInfo getValuesMethod = typeof(EnumHelper).GetMethod("GetValues").MakeGenericMethod(type);
            return (System.Collections.IEnumerable)getValuesMethod.Invoke(null, null);
        }
    }
}
