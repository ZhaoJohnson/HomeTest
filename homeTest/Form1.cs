using homeTest.Service;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace homeTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //var sheetName = "Monthly Property Schedule";
            //int headRowNumber = 3;
            var sheetName = "sheet1";
            int headRowNumber = 0;
            var dialog = new OpenFileDialog();
            dialog.Title = "请选择文件";
            dialog.Filter = "所有文件(*.*)|*.*";
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                string selectfile = dialog.FileName;
                FileInfo finfo = new FileInfo(selectfile);
                var sourceFile = finfo.FullName;
                var excelDT = NPOIHelper.ImportExceltoDt(sourceFile, sheetName, headRowNumber);// NPOIHelper.im(sourceFile, 1);
                var testapp = new ExcelTest();
                testapp.Execute(excelDT);
                //var TapeBiz = new MonthlyTapeService(excelDT);
                //TapeBiz.SaveExcel();

            }
        }
    }
}
