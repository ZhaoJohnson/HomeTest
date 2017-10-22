using homeTest.Model;
using HpaUtility.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace homeTest.Service
{
    public class ExcelTest
    {
        protected Dictionary<string, string> TableMapping { get; set; }
        protected DataTable Dt { get; set; }
        public void Execute(DataTable dt)
        {
            Dt = dt;
            TableMapping = GetMapping();
            ChangeTableCol();
            var List = ConvertList< TapeTastModel>();
        }
        /// <summary>
        /// Setp 1. Build Mapping
        /// </summary>
        /// <returns></returns>
        protected  Dictionary<string, string> GetMapping()
        {
            TapeTastModel model = new TapeTastModel();
            Dictionary<string, string> result = new Dictionary<string, string>();
            result.Add("Property Id", model.GetPropertyInfo(r => r.PropertyId).Name);
            result.Add("Reporting Date", model.GetPropertyInfo(r => r.ReportingDate).Name);
            result.Add("Allocated Loan Amount", model.GetPropertyInfo(r => r.AllocatedLoanAmount).Name);
            result.Add("NewFile", model.GetPropertyInfo(r => r.TestNew).Name);
            return result;
        }
        /// <summary>
        /// Setp 2.ChangeTable
        /// </summary>
        protected virtual void ChangeTableCol()
        {
            for (int i = 0; i < Dt.Columns.Count; i++)
            {
                var tempColName = Dt.Columns[i].ColumnName;
                //logger.Info(tempColName);
                //WriteLog(tempColName);
                if (TableMapping.ContainsKey(tempColName.ToLower()))
                {
                    var newName = TableMapping[tempColName];
                    if (!string.IsNullOrEmpty(newName))
                    {
                        Dt.Columns[i].ColumnName = newName;
                    }
                    TableMapping.Remove(tempColName);
                }
                else
                {
                    WriteLog($"The Column [{tempColName}] Can't Find In TableMapping");
                }
            }
            foreach (var item in TableMapping)
            {
                WriteLog($"The Column [{item.Key}] Can't Find In Excel");
            }
        }
        /// <summary>
        /// Setp 3. Cov LIst
        /// </summary>
        /// <returns></returns>
        protected virtual List<ExcelEntity> ConvertList<ExcelEntity>()
            where ExcelEntity:class,new()
        {
            return NPOIHelper.ConvertToModel<ExcelEntity>(Dt).ToList();
        }

        protected void WriteLog(string message)
        {
            var fileName = this.GetType().ToString();
           LogUtility.LogLine($"{fileName}{DateTime.Now.ToString("MMddyyyy")}.txt", $"{DateTime.Now.ToString()} : {message}");
        }
    }
}
