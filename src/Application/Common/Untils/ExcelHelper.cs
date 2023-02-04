using Application.Common.Exceptions;
using Microsoft.Win32.SafeHandles;
using Newtonsoft.Json;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System.Data;

namespace Application.Common.Untils
{
    public static class ExcelHelper
    {
        public static T[]? ReadExcelFile<T>(byte[] data)
        {
            var properties = typeof(T).GetProperties().Select(p => p.Name);

            DataTable dtTable = new DataTable();
            List<string> rowList = new List<string>();
            ISheet sheet;
            using (var stream = new MemoryStream(data))
            {
                stream.Position = 0;
                XSSFWorkbook xssWorkbook = new XSSFWorkbook(stream);
                sheet = xssWorkbook.GetSheetAt(0);

                IRow headerRow = sheet.GetRow(0);
                int cellCount = headerRow.LastCellNum;

                // Get header row then add first row cell (header row) to datatable colums
                if (cellCount != properties.Count())
                {
                    throw BussinessException.InvalidExcelFileForm();
                }
                foreach (var headerCell in properties)
                {
                    dtTable.Columns.Add(headerCell);
                }

                // Loop from header row = 1 to last row of sheet
                for (int i = sheet.FirstRowNum + 1; i <= sheet.LastRowNum; i++)
                {
                    IRow row = sheet.GetRow(i);
                    if (row == null) continue;
                    if (row.Cells.All(d => d.CellType == CellType.Blank)) continue;
                    for (int j = row.FirstCellNum; j < cellCount; j++)
                    {
                        if (row.GetCell(j) != null)
                        {
                            if (!string.IsNullOrEmpty(row.GetCell(j).ToString()) && !string.IsNullOrWhiteSpace(row.GetCell(j).ToString()))
                            {
                                rowList.Add(row.GetCell(j).ToString() ?? "");
                            }
                        }
                    }
                    if (rowList.Count > 0)
                        dtTable.Rows.Add(rowList.ToArray());
                    rowList.Clear();
                }
            }
            return JsonConvert.DeserializeObject<T[]>(JsonConvert.SerializeObject(dtTable));
        }

        public static byte[] WriteToExcelFile<T>(List<T> data)
        {
            var table = JsonConvert.DeserializeObject<DataTable>(JsonConvert.SerializeObject(data, Formatting.Indented));
            var columsName = new List<string>();
            if (table == null) return Array.Empty<byte>();

            using (var memoryStream = new MemoryStream())
            {
                IWorkbook workbook = new XSSFWorkbook();
                ISheet excelSheet = workbook.CreateSheet("Sheet1");
                int rowindex = 0;
                int colIndex = 0;
                // Set header row
                IRow headerRow = excelSheet.CreateRow(0);
                foreach (DataColumn tableCol in table.Columns)
                {
                    columsName.Add(tableCol.ColumnName);
                    headerRow.CreateCell(colIndex).SetCellValue(tableCol.ColumnName);
                    colIndex++;
                }

                // Set row data
                foreach (DataRow dsrow in table.Rows)
                {
                    IRow row = excelSheet.CreateRow(++rowindex);
                    for (var index = 0; index < colIndex; index++)
                    {
                        row.CreateCell(index).SetCellValue(dsrow[index].ToString());
                    }
                }

                workbook.Write(memoryStream, true);

                return memoryStream.ToArray();
            }
        }
    }
}