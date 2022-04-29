using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;

namespace USTGlobal.PIP.EmailScheduler.Helpers
{
    public class IListToDataTableHelper
    {
        public static DataTable ToDataTables<T>(IList<T> data)
        {
            PropertyDescriptorCollection props = TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();
            for (int i = 0; i < props.Count; i++)
            {
                PropertyDescriptor prp = props[i];
                table.Columns.Add(prp.Name, Nullable.GetUnderlyingType(prp.PropertyType) ?? prp.PropertyType);
            }
            object[] values = new object[props.Count];
            foreach (T item in data)
            {
                for (int i = 0; i < values.Length; i++)
                {
                    values[i] = props[i].GetValue(item);
                }
                table.Rows.Add(values);
            }
            return table;
        }

        /// <summary>
        /// Function to convert object to datatable
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data"></param>
        /// <returns></returns>
        public static DataTable ToDataTable<T>(T data)
        {
            PropertyDescriptorCollection props = TypeDescriptor.GetProperties(typeof(T));
            IList<T> listOfData = new List<T>();
            listOfData.Add(data);

            DataTable table = new DataTable();

            for (int i = 0; i < props.Count; i++)
            {
                PropertyDescriptor prp = props[i];
                table.Columns.Add(prp.Name, Nullable.GetUnderlyingType(prp.PropertyType) ?? prp.PropertyType);
            }

            object[] values = new object[props.Count];
            foreach (T item in listOfData)
            {
                for (int i = 0; i < values.Length; i++)
                {
                    values[i] = props[i].GetValue(item);
                }
                table.Rows.Add(values);
            }
            return table;
        }
    }
}
