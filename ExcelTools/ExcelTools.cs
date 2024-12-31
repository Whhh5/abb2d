using System;
using OfficeOpenXml;
using System.Drawing;
using OfficeOpenXml.Style;
using System;
using System.Reflection;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Text;

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
                var worksheet = workSheets[i];
                var rowCount = worksheet.Dimension.End.Row;
                var colCount = worksheet.Dimension.End.Column;

                for (int rowI = 1; rowI <= rowCount; rowI++)
                {
                    for (int colI = 1; colI <= colCount; colI++) 
                    {
                        var valueStr = worksheet.GetValue(rowI, colI);
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
        public Dictionary<string, List<ICfg>> GetExportCfgListData(List<ExportExcelInfo> exportExcelInfoList)
        {
            var cfgDic = new Dictionary<string, List<ICfg>>();
            for (int i = 0; i < exportExcelInfoList.Count; i++)
            {
                var exportInfo = exportExcelInfoList[i];
                var className = exportInfo.worksheet.Name;
                var dynamicType = ExcelUtil.CreateDynamicType(className, exportInfo.fieldList, exportInfo.fieldTypeList);
                var fieldList = dynamicType.GetFields(BindingFlags.Public | BindingFlags.Instance);

                var cfgInsList = new List<ICfg>();
                var rowCount = exportInfo.worksheet.Dimension.End.Row;
                for (int j = exportInfo.excelInfo.dataStartRow; j <= rowCount; j++)
                {
                    var insType = Activator.CreateInstance(dynamicType);
                    for (int k = 0; k < fieldList.Length; k++)
                    {
                        var fieldInfo = fieldList[k];
                        if (!exportInfo.field2ColList.TryGetValue(fieldInfo.Name, out var col))
                        {
                            Console.Error.WriteLine($"读取字段列数失败，type:{insType}, name:{fieldInfo.Name}");
                            continue;
                        }
                        var valueStr = exportInfo.worksheet.GetValue<string>(j, col);
                        var value = Convert.ChangeType(valueStr, fieldInfo.FieldType);
                        if (value == null)
                        {
                            Console.Error.WriteLine($"类型转化失败，type:{insType}, value:{valueStr}， row:{j}, col:{col}, ");
                            continue;
                        }
                        fieldInfo.SetValue(insType, value);
                    }
                    cfgInsList.Add(insType as ICfg);
                }
                cfgDic.Add(className, cfgInsList);
            }
            return cfgDic;
        }
        public void ExportExcel2JsonFile(Dictionary<string, List<ICfg>> cfgDic, string targetRoot)
        {
            foreach (var item in cfgDic)
            {
                var jsonStr = JsonConvert.SerializeObject(item.Value);
                var path = Path.Combine(targetRoot, $"{item.Key}.json");
                File.WriteAllText(path, jsonStr, System.Text.Encoding.UTF8);
                Console.WriteLine(path);
            }
        }

        public void ExportExcel2CSFile(List<ExportExcelInfo> excelInfoList, string targetRoot)
        {
            foreach (var excelInfo in excelInfoList)
            {
                var name = excelInfo.worksheet.Name;
                var path = Path.Combine(targetRoot, $"{name}.cs");
                var csContentStr = ExcelUtil.CreateCshapFileContent(name, excelInfo.strDesc, excelInfo.fieldList, excelInfo.fieldTypeList, excelInfo.descList);

                File.WriteAllText(path, csContentStr, System.Text.Encoding.UTF8);
                Console.WriteLine(path);
            }
        }

        private ExportExcelInfo GetExportExcelInfo(ExcelWorksheet sheet, ExcelInfo excelInfo)
        {
            var exportExcelInfo = new ExportExcelInfo();
            exportExcelInfo.excelInfo = excelInfo;
            exportExcelInfo.strDesc = $"{sheet.Name}";
            exportExcelInfo.worksheet = sheet;
            var colCount = sheet.Dimension.End.Column;
            for (int col = excelInfo.dataStartCol; col <= colCount; col++)
            {
                var valueStr = sheet.GetValue<string>(excelInfo.descRow, col);
                exportExcelInfo.descList.Add(valueStr);
            }
            for (int col = excelInfo.dataStartCol; col <= colCount; col++)
            {
                var valueStr = sheet.GetValue<string>(excelInfo.fieldRow, col);
                exportExcelInfo.fieldList.Add(valueStr);
                exportExcelInfo.field2ColList.Add(valueStr, col);
            }
            for (int col = excelInfo.dataStartCol; col <= colCount; col++)
            {
                var valueStr = sheet.GetValue<string>(excelInfo.fieldTypeRow, col);
                exportExcelInfo.fieldTypeList.Add(valueStr);
            }
            return exportExcelInfo;
        }

        public void CreateCfgUtil(List<ExportExcelInfo> excelInfoList, string targetRoot)
        {
            var strBuilder = new StringBuilder();
            strBuilder.AppendLine($"public static class CfgUtil");
            strBuilder.AppendLine("{");
            for (int i = 0; i < excelInfoList.Count; i++)
            {
                var excelInfo = excelInfoList[i];
                var keysList = excelInfo.excelInfo.keysCol;
                for (int j = 0; j < keysList.Count; j++)
                {
                    var keyColList = keysList[j];
                    strBuilder.AppendLine($"private Dictionary<>");
                    for (int k = 0; k < keyColList.Count; k++)
                    {

                    }
                }
            }
            strBuilder.AppendLine("}");
        }

        private ExcelInfo GetExcelInfo(ExcelWorksheet worksheet, EnExcelSourceType sourceType)
        {
            var result = new ExcelInfo();
            result.excelName = worksheet.Name;

            var checkCol = 1;
            var startCol = 2;
            var rowCount = MathF.Min(10, worksheet.Dimension.End.Row);
            var colCount = worksheet.Dimension.End.Column;
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
    }

    public class ExportExcelInfo
    {
        public ExcelInfo excelInfo;
        public string strDesc;
        public List<string> fieldList = new();
        public Dictionary<string, int> field2ColList = new();
        public List<string> fieldTypeList = new();
        public List<string> descList = new();
        public ExcelWorksheet worksheet;
    }
    public class ExcelInfo
    {
        public string excelName;
        public int dataStartRow;
        public int dataStartCol;
        public int fieldRow;
        public int fieldTypeRow;
        public int descRow;
        public HashSet<int> validCol = new();
        public List<List<int>> keysCol = new();
    }
}

