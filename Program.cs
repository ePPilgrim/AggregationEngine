// See https://aka.ms/new-console-template for more information

using AggregationEngine;
using System.Reflection;
using static System.Net.Mime.MediaTypeNames;
using System.Reflection.Emit;

var obj = new MetaData()
{
    Name = "Object",
    PortfolioName = "Portfolio",
    SecurityIK = 1981,
    Currency = 3.14,
    HoldingIK = 42
};

Type myClassType = typeof(IMetaData);
PropertyInfo[] properties = myClassType.GetProperties();

foreach (PropertyInfo property in properties)
{
    if (property.GetCustomAttributes(typeof(AggregationLevelAttribute), true).Any())
    {
        var attributeObj = property.GetCustomAttribute<AggregationLevelAttribute>();
        //Console.WriteLine($"Property name: {property.Name}");
        Console.WriteLine($"Aggregation level: {attributeObj.AggregationLevel}");

        Type[] aargs = { typeof(IMetaData)};
        DynamicMethod GetProportyValue = new DynamicMethod($"Get{property.Name}",property.PropertyType,aargs,typeof(Program).Module);
        MethodInfo getPropertyMethod = property.GetGetMethod();

        ILGenerator il = GetProportyValue.GetILGenerator();
        il.Emit(OpCodes.Ldarg_0);
        il.EmitCall(OpCodes.Callvirt, getPropertyMethod, null);
        il.Emit(OpCodes.Ret);


        Type delegateType = typeof(Func<,>).MakeGenericType(typeof(IMetaData), property.PropertyType);
        Delegate Rep = GetProportyValue.CreateDelegate(delegateType);

        Getter getter = (IMetaData x) =>
        {
            var result = Rep.DynamicInvoke(obj);
            if(result == null)
            {
                return "Somthin wrond!!!!!!!!!!!!!!!!!!!";
            }
            return result.ToString();
        };

        Console.WriteLine($"Property name: {property.Name}, Property value: {getter(obj)}");

        
        //Delegate getStringDelegate = methodBuilder.CreateDelegate(delegateType);


    }
}




Console.WriteLine("Hello, World!");


delegate string? Getter(IMetaData metaData);