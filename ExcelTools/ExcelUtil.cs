using System;
using OfficeOpenXml;
using System.Reflection;
using System.Reflection.Emit;

namespace ExcelTools
{
	public static class ExcelUtil
	{
        public static string GetExecutingDir()
        {
            var curAssembly = Assembly.GetExecutingAssembly();
            var curPath = Path.GetDirectoryName(curAssembly.Location);
            return curPath;
        }
        public static ExcelPackage GetExccel(string excelPath)
        {
            var excelPackage = new ExcelPackage(excelPath);
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            return excelPackage;
        }
        public static Type CreateDynamicType(string className, List<string> fieldNameList, List<string> fieldTypeList)
        {
            var assemblyName = new AssemblyName("DynamicAssembly");
            AssemblyBuilder assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
            ModuleBuilder moduleBuilder = assemblyBuilder.DefineDynamicModule("MainModule");
            TypeBuilder classBuilder = moduleBuilder.DefineType(className, TypeAttributes.Public);

            // 创建一个无参数的构造函数
            var constructorBuilder = classBuilder.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, Type.EmptyTypes);
            ILGenerator constructorIL = constructorBuilder.GetILGenerator();
            //constructorIL.Emit(OpCodes.Ldstr, "Hello, World!");
            //constructorIL.Emit(OpCodes.Call, typeof(Console).GetMethod("WriteLine", new Type[] { typeof(string) }));
            constructorIL.Emit(OpCodes.Ret);

            for (int i = 0; i < fieldNameList.Count; i++)
            {
                var fileName = fieldNameList[i];
                var filedKeyType = fieldTypeList[i];
                var fieldType = Key2Type(filedKeyType);
                if (string.IsNullOrEmpty(fileName) || fieldType == null)
                {
                    Console.WriteLine($"类型转化失败 class:{className}, name:{fileName}, type:{filedKeyType}");
                    continue;
                }
                var fieldBuilder = classBuilder.DefineField(fileName, fieldType, FieldAttributes.Public);
            }

            // 创建类型
            Type dynamicType = classBuilder.CreateType();
            return dynamicType;
        }
        public static Type Key2Type(string key)
        {
            return key switch
            {
                "int32" => typeof(int),
                "int64" => typeof(long),
                "int32[]" => typeof(int[]),
                "int64[]" => typeof(long[]),
                "uint32" => typeof(uint),
                "uint64" => typeof(ulong),
                "uint32[]" => typeof(uint[]),
                "uint64[]" => typeof(ulong[]),
                "float" => typeof(float),
                "float[]" => typeof(float[]),
                "double" => typeof(double),
                "double[]" => typeof(double[]),
                "string" => typeof(string),
                "string[]" => typeof(string[]),
                _ => null,
            };
        }
    }
}

