using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CastleClub.BusinessLogic.Utils
{
    public class Excel
    {
        public static string GenerateExcelFile(List<List<string>> body)
        {
            FileInfo template = new FileInfo(CastleClub.BusinessLogic.Data.GlobalParameters.ExcelTemplatePath);
            DateTime now = DateTime.Now;
            string name = now.Year + "_" + now.Month + "_" + now.Day + "_" + now.Hour + "_" + now.Minute + "_" + now.Second + ".xlsx";
            string returnPath = CastleClub.BusinessLogic.Data.GlobalParameters.ExcelOutPath + "\\" + name;
            FileInfo newFile = new FileInfo(returnPath);

            using (ExcelPackage excelPackage = new ExcelPackage(newFile, template))
            {
                ExcelWorkbook myWorkbook = excelPackage.Workbook;
                ExcelWorksheet myWorksheet = myWorkbook.Worksheets["Hoja1"];

                foreach (List<string> row in body)
                {
                    foreach (string cell in row)
                    {
                        myWorksheet.Cell(body.IndexOf(row) + 2, row.IndexOf(cell) + 1).Value = cell.Replace("'", "`");
                    }
                }

                excelPackage.Save();

                return name;
            }
        }

        public static string GetExcelColumn(int column)
        {
            string excelColumn = string.Empty;
            column = column - 1;

            do
            {
                int indexColumn = column % 26;
                column = column / 26;
                switch (indexColumn)
                {
                    case 0:
                        excelColumn = "A" + excelColumn;
                        break;
                    case 1:
                        excelColumn = "B" + excelColumn;
                        break;
                    case 2:
                        excelColumn = "C" + excelColumn;
                        break;
                    case 3:
                        excelColumn = "D" + excelColumn;
                        break;
                    case 4:
                        excelColumn = "E" + excelColumn;
                        break;
                    case 5:
                        excelColumn = "F" + excelColumn;
                        break;
                    case 6:
                        excelColumn = "G" + excelColumn;
                        break;
                    case 7:
                        excelColumn = "H" + excelColumn;
                        break;
                    case 8:
                        excelColumn = "I" + excelColumn;
                        break;
                    case 9:
                        excelColumn = "J" + excelColumn;
                        break;
                    case 10:
                        excelColumn = "K" + excelColumn;
                        break;
                    case 11:
                        excelColumn = "L" + excelColumn;
                        break;
                    case 12:
                        excelColumn = "M" + excelColumn;
                        break;
                    case 13:
                        excelColumn = "N" + excelColumn;
                        break;
                    case 14:
                        excelColumn = "O" + excelColumn;
                        break;
                    case 15:
                        excelColumn = "P" + excelColumn;
                        break;
                    case 16:
                        excelColumn = "Q" + excelColumn;
                        break;
                    case 17:
                        excelColumn = "R" + excelColumn;
                        break;
                    case 18:
                        excelColumn = "S" + excelColumn;
                        break;
                    case 19:
                        excelColumn = "T" + excelColumn;
                        break;
                    case 20:
                        excelColumn = "U" + excelColumn;
                        break;
                    case 21:
                        excelColumn = "V" + excelColumn;
                        break;
                    case 22:
                        excelColumn = "W" + excelColumn;
                        break;
                    case 23:
                        excelColumn = "X" + excelColumn;
                        break;
                    case 24:
                        excelColumn = "Y" + excelColumn;
                        break;
                    case 25:
                        excelColumn = "Z" + excelColumn;
                        break;
                }
            }
            while (column > 0);

            return excelColumn;
        }
    }
}
