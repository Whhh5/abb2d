using System;
using OfficeOpenXml;
using System.Drawing;
using OfficeOpenXml.Style;
using System;
using System.Reflection;
using Newtonsoft.Json;
using System.Diagnostics;

namespace ExcelTools
{
    public enum EnExcelSourceType
    {
        c = 1 << 0, // client
        s = 1 << 1, // server
        All = c + s,
    }
    public class ExcelTools
    {
        public List<string> GetExportCfgList(string dir)
        {
            var catalogPath = Path.Combine(dir, $"CfgCatalog.xlsx");
            var catalogExcel = ExcelUtil.GetExccel(catalogPath);

            var workbook = catalogExcel.Workbook;
            var workSheets = workbook.Worksheets;

            var exportList = new List<string>();
            for (int i = 0; i < workSheets.Count; i++)
            {
                var workSheet = workSheets[i];
                var rowCount = workSheet.Rows.Count();
                var colCount = workSheet.Columns.Count();

                for (int rowI = 1; rowI <= rowCount; rowI++)
                {
                    for (int colI = 1; colI <= colCount; colI++) 
                    {
                        var valueStr = workSheet.GetValue(rowI, colI);
                        var path = Path.Combine(dir, $"{valueStr}.xlsx");
                        Console.WriteLine(path);
                        exportList.Add(path);
                    }
                }
            }
            return exportList;
        }

        public List<ExportExcelInfo> GetExportExcelInfoList(List<string> cfgList, EnExcelSourceType sourceType)
        {
            var exportExcelInfoList = new List<ExportExcelInfo>();
            for (int i = 0; i < cfgList.Count; i++)
            {
                var cfgPackage = ExcelUtil.GetExccel(cfgList[i]);
                var workbook = cfgPackage.Workbook;
                var worksheets = workbook.Worksheets;
                for (int j = 0; j < worksheets.Count(); j++)
                {
                    var sheet = worksheets[i];
                    var excelInfo = GetExcelInfo(sheet, sourceType);

                    var exportExcelInfo = GetExportExcelInfo(sheet, excelInfo);
                    exportExcelInfoList.Add(exportExcelInfo);
                }
            }
            return exportExcelInfoList;
        }
        public Dictionary<string, List<object>> GetExportCfgListData(List<ExportExcelInfo> exportExcelInfoList)
        {
            var cfgDic = new Dictionary<string, List<object>>();
            for (int i = 0; i < exportExcelInfoList.Count; i++)
            {
                var exportInfo = exportExcelInfoList[i];
                var className = exportInfo.worksheet.Name;
                var dynamicType = ExcelUtil.CreateDynamicType(className, exportInfo.fieldList, exportInfo.fieldTypeList);
                var fieldList = dynamicType.GetFields(BindingFlags.Public | BindingFlags.Instance);

                var cfgInsList = new List<object>();
                for (int j = exportInfo.excelInfo.dataStartRow; j <= exportInfo.worksheet.Rows.Count(); j++)
                {
                    var insType = Activator.CreateInstance(dynamicType);
                    for (int k = 0; k < fieldList.Length; k++)
                    {
                        var fieldInfo = fieldList[k];
                        if (!exportInfo.field2ColList.TryGetValue(fieldInfo.Name, out var col))
                        {
                            Console.WriteLine($"读取字段列数失败，type:{insType}, name:{fieldInfo.Name}");
                            continue;
                        }
                        var valueStr = exportInfo.worksheet.GetValue<string>(j, col);
                        var value = Convert.ChangeType(valueStr, fieldInfo.FieldType);
                        if (value == null)
                        {
                            Console.WriteLine($"类型转化失败，type:{insType}, value:{valueStr}， row:{j}, col:{col}, ");
                            continue;
                        }
                        fieldInfo.SetValue(insType, value);
                    }
                    cfgInsList.Add(insType);
                }
                cfgDic.Add(className, cfgInsList);
            }
            return cfgDic;
        }
        public void ExportExcel2JsonFile(Dictionary<string, List<object>> cfgDic, string targetRoot)
        {

            foreach (var item in cfgDic)
            {
                var jsonStr = JsonConvert.SerializeObject(item.Value);
                var path = Path.Combine(targetRoot, $"{item.Key}.json");
                File.WriteAllText(path, jsonStr, System.Text.Encoding.UTF8);
                Console.WriteLine(path);
            }
        }

        private ExportExcelInfo GetExportExcelInfo(ExcelWorksheet sheet, ExcelInfo excelInfo)
        {
            var exportExcelInfo = new ExportExcelInfo();
            exportExcelInfo.excelInfo = excelInfo;
            exportExcelInfo.worksheet = sheet;
            for (int col = excelInfo.dataStartCol; col < sheet.Columns.Count(); col++)
            {
                var valueStr = sheet.GetValue<string>(excelInfo.descRow, col);
                exportExcelInfo.descList.Add(valueStr);
            }
            for (int col = excelInfo.dataStartCol; col < sheet.Columns.Count(); col++)
            {
                var valueStr = sheet.GetValue<string>(excelInfo.fieldRow, col);
                exportExcelInfo.fieldList.Add(valueStr);
                exportExcelInfo.field2ColList.Add(valueStr, col);
            }
            for (int col = excelInfo.dataStartCol; col < sheet.Columns.Count(); col++)
            {
                var valueStr = sheet.GetValue<string>(excelInfo.fieldTypeRow, col);
                exportExcelInfo.fieldTypeList.Add(valueStr);
                exportExcelInfo.fieldType2ColList.Add(valueStr, col);
            }
            return exportExcelInfo;
        }

        private ExcelInfo GetExcelInfo(ExcelWorksheet worksheet, EnExcelSourceType sourceType)
        {
            var result = new ExcelInfo();

            var checkCol = 1;
            var startCol = 2;
            var rowCount = MathF.Min(10, worksheet.Rows.Count());
            var colCount = worksheet.Columns.Count();
            var startRow = -1;
            for (int i = 1; i <= rowCount; i++)
            {
                var valueStr = worksheet.GetValue<string>(i, checkCol);
                switch (valueStr)
                {
                    case "${name}":
                        result.fieldRow = i;
                        break;
                    case "${desc}":
                        result.descRow = i;
                        break;
                    case "${type}":
                        result.fieldTypeRow = i;
                        break;
                    case "${source}":
                        {
                            for (int j = startCol; j <= colCount; j++)
                            {
                                var valueSource = worksheet.GetValue<string>(i, j);
                                foreach (var item in valueSource)
                                {
                                    if (!Enum.TryParse<EnExcelSourceType>(item.ToString(), true, out var type))
                                        continue;
                                    if ((sourceType & type) != type)
                                        continue;
                                    if (result.validCol.Contains(j))
                                        continue;
                                    result.validCol.Add(j);
                                }
                            }
                        }
                        break;
                    case "${key}":
                        {
                            var keyInfo = new List<int>();
                            result.keysCol.Add(keyInfo);
                            for (int j = startCol; j <= colCount; j++)
                            {
                                var valueSource = worksheet.GetValue<string>(i, j);
                                if (string.IsNullOrEmpty(valueSource))
                                    continue;
                                keyInfo.Add(j);
                            }
                        }
                        break;
                    default:
                        continue;
                }
                startRow = i;
            }
            result.dataStartCol = startCol;
            result.dataStartRow = startRow + 1;
            return result;
        }
        private void CreateCfgClass(List<string> cfgList, string targetRoot)
        {

        }
        //private void 
    }

    public class ExportExcelInfo
    {
        public ExcelInfo excelInfo;
        public List<string> fieldList = new();
        public Dictionary<string, int> field2ColList = new();
        public List<string> fieldTypeList = new();
        public Dictionary<string, int> fieldType2ColList = new();
        public List<string> descList = new();
        public ExcelWorksheet worksheet;
    }
    public class ExcelInfo
    {
        public int dataStartRow;
        public int dataStartCol;
        public int fieldRow;
        public int fieldTypeRow;
        public int descRow;
        public HashSet<int> validCol = new();
        public List<List<int>> keysCol = new();
    }
}

