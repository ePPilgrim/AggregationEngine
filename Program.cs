// See https://aka.ms/new-console-template for more information

using AggregationEngine;
using System.Reflection;
using static System.Net.Mime.MediaTypeNames;
using System.Reflection.Emit;
using System.Collections.ObjectModel;



var obj = new MetaData()
{
    Name = "Object",
    PortfolioName = "Portfolio",
    SecurityIK = 1981,
    Currency = 3.14,
    HoldingIK = 42
};

var ddd = new int[] { 1, 1, 1, 2, 3, 4, 5, 6, 7, };
var sd = ddd.ToHashSet();


//var v = typeof(MetaData).GetInterfaces().SelectMany(x => x.GetProperties());

//foreach (var property in v)
//{
//    var tt = property.GetGetMethod();
//    Console.WriteLine($"Property {property.Name} is declared in {tt.Name}");
//}

//Console.WriteLine("-------------------------------------------------------------------------------------------");
var mdi = new List<Type> { typeof(IMetaData3) };
mdi.AddRange(typeof(IMetaData3).GetInterfaces());

var name_of_getters = mdi.SelectMany(x => x.GetProperties())
                            .Where(x => x.GetMethod != null)
                            .Select(x => x.GetMethod.Name).ToHashSet();

if(typeof(MetaData).GetInterfaces().Any(x => x == typeof(IMetaData))){
    var properties = typeof(MetaData).GetProperties()
                                .Where(x => x.GetMethod != null)
                                .Where(x => x.GetCustomAttribute<AggregationLevelAttribute>() != null)
                                .Where(x => name_of_getters.Contains(x.GetMethod.Name));
    foreach (var property in properties)
    {
        var tt = property.GetMethod;
        var getMethodName = (tt != null) ? tt.Name : "None";
        var at = property.GetCustomAttribute<AggregationLevelAttribute>();
        var val = (at != null) ? at.AggregationLevel : AggregationLevel.None;
        Console.WriteLine($"Property {property.Name} is declared in {getMethodName} and has attribute with value = {val}");
    }
}



//Type myClassType = typeof(IMetaData);
////PropertyInfo[] properties = myClassType.GetProperties();

//Type[] interfaces = myClassType.GetInterfaces();

//var dels = new List<object>();
//foreach (PropertyInfo property in properties)
//{
//    if (property.GetCustomAttributes(typeof(AggregationLevelAttribute), true).Any())
//    {
//        var attributeObj = property.GetCustomAttribute<AggregationLevelAttribute>();
//        //Console.WriteLine($"Property name: {property.Name}");
//        //Console.WriteLine($"Aggregation level: {attributeObj.AggregationLevel}");

//        Type[] aargs = { typeof(IMetaData)};
//        DynamicMethod GetProportyValue = new DynamicMethod($"Get{property.Name}",property.PropertyType,aargs,typeof(Program).Module);
//        MethodInfo getPropertyMethod = property.GetGetMethod();

//        ILGenerator il = GetProportyValue.GetILGenerator();
//        il.Emit(OpCodes.Ldarg_0);
//        il.EmitCall(OpCodes.Callvirt, getPropertyMethod, null);
//        il.Emit(OpCodes.Ret);


//        Type delegateType = typeof(Func<,>).MakeGenericType(typeof(IMetaData), property.PropertyType);
//        Delegate Rep = GetProportyValue.CreateDelegate(delegateType);

//        DelegateGetter<IMetaData>.Getter getter = (IMetaData x) =>
//        {
//            var result = Rep.DynamicInvoke(obj);
//            return KeyValuePair.Create(property.Name, (result == null) ? null : result.ToString());
//        };

//        dels.Add(getter);

//        //Console.WriteLine($"Property name: {property.Name}, Property value: {getter(obj)}");

        
//        //Delegate getStringDelegate = methodBuilder.CreateDelegate(delegateType);
//    }
//}

//foreach(var del in dels)
//{
//    var getter = del as DelegateGetter<IMetaData>.Getter;
//    if(getter == null)
//    {
//        Console.WriteLine("Could not resolve delegate!!!");
//        break;
//    }
//    var res = getter(obj);
//    Console.WriteLine($"Property name: {res.Key}, Property value: {res.Value}");
//}




//Console.WriteLine("Hello, World!");

//public class DelegateGetter<T>
//{
//    public delegate KeyValuePair<string, string?> Getter(T metaData);
//}

