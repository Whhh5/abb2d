using UnityEngine;
using OfficeOpenXml;
using System.IO;

public class ExcelUtil
{
    public static ExcelPackage ReadExcel(string path)
    {
        var fileInfo = new FileInfo(path);
        var excelPackage = new ExcelPackage(fileInfo);
        return excelPackage;
    }
    public static void SaveExcel()
    {

    }
}
