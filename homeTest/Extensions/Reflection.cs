using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;

namespace HpaUtility.Extensions
{
    internal class Reflection
    {
        public static void FillObjectWithProperty(ref object objectTo, string propertyName, object propertyValue)
        {
            var tOb2 = objectTo.GetType();
            if (propertyValue == DBNull.Value) 
                return;

            var tObj2PropInfo = tOb2.GetProperty(propertyName);
            if (tObj2PropInfo == null || tObj2PropInfo.SetMethod == null)
                return;

            if(tObj2PropInfo.PropertyType.BaseType != null && tObj2PropInfo.PropertyType.BaseType == (typeof(Enum)))
                tObj2PropInfo.SetValue(objectTo, Enum.Parse(tObj2PropInfo.PropertyType, propertyValue.ToString()));
            else if (tObj2PropInfo.PropertyType.Name.Equals(typeof (List<>).Name))
            {
                //Bypass, since not support list right now.
            }
            else
                tObj2PropInfo.SetValue(objectTo, propertyValue);
        }

        public static T AssembeObj<T>(DbDataReader dr, Type instance)
        {
            return AssembeObj<T>(dr, Activator.CreateInstance(instance));
        }

        public static T AssembeObj<T>(DbDataReader dr, object instance = null)
        {
            //Create an instance of the object needed.
            //The instance is created by obtaining the object type T of the object
            //list, which is the object that calls the extension method
            //Type T is inferred and is instantiated
            if (instance == null)
                instance = Activator.CreateInstance(typeof(T));

            //Loop all the fields of each row of dataReader, and through the object
            //reflector (first step method) fill the object instance with the datareader values
            var schemaTable = dr.GetSchemaTable();
            if (schemaTable != null)
            {
                foreach (DataRow row in schemaTable.Rows)
                {
                    FillObjectWithProperty(ref instance,
                        row.ItemArray[0].ToString(), dr[row.ItemArray[0].ToString()]);
                }
            }

            return (T) Convert.ChangeType(instance, typeof (T));
        }


        public static T ConvertType<T>(object instance)
        {
            return (T)Convert.ChangeType(instance, typeof(T));
        }
        

        public static object AssembeObj<T, T2>(T t, object instance = null, IEnumerable<PropertyInfo> ignoreProperties = null)
        {
            //Step 1. Create an instance of the object need
            if (instance == null)
                instance = Activator.CreateInstance(typeof (T2));

            //Step 2. Check T2 object has members or not. if not have, return empty T2 object
            var t2MemeberInfos = typeof (T2).GetMembers();
            if (t2MemeberInfos.IsNullOrEmpty())
                return (T2) Convert.ChangeType(instance, typeof (T2));

            //Step 3. AssembeObj
            var tObjType = t.GetType();
            foreach (var t2MemberInfo in t2MemeberInfos)
            {
                var property = tObjType.GetProperty(t2MemberInfo.Name);
                if(property == null)
                    continue;

                if(ignoreProperties != null && ignoreProperties.Any(r => r.PropertyType == property.PropertyType && r.Name == property.Name))
                    continue;
                
                //not support collection for now.
                if(property.PropertyType.IsGenericType && property.PropertyType.GetGenericTypeDefinition() == typeof(ICollection<>))
                    continue;

                try
                {
                    FillObjectWithProperty(ref instance, t2MemberInfo.Name, property.GetValue(t));
                }
                catch { }
            }
            return instance;
        }

        public static IEnumerable<Type> GetClassTypeByNamespace(string asmFileName, string ns)
        {
            var asmPath = AppDomain.CurrentDomain.BaseDirectory; //Your assemblies's root folder
            var asmFullPath = System.IO.Path.Combine(asmPath, asmFileName);
            var asm = System.Reflection.Assembly.LoadFrom(asmFullPath);

            return asm.GetTypes()
                .Where(t => t.IsClass && !string.IsNullOrEmpty(t.Namespace) && t.Namespace.Contains(ns));
        } 
    }


}
