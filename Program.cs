// See https://aka.ms/new-console-template for more information

using AggregationEngine;
using System.Reflection;
using static System.Net.Mime.MediaTypeNames;
using System.Reflection.Emit;
using System.Collections.ObjectModel;


int[] ar = new int[3];
int i = 0;
ar[i++] = 1;

var t1 = new Stack<int>(new int[] {1,2,3,4});
t1.Push(333);
var tt1 = t1.ToArray();

var t2 = new Stack<int>(t1.Reverse());


var t = new Stack<int>();
t.Push(1);
t.Push(2);
t.Push(3);
t.Push(4);

var tt = t.ToArray();

object ttt = (object)tt;
var type = ttt.GetType();

foreach(var el in t)
{
    Console.WriteLine(el);  
}

var v1=AggregationLevel.Holding.ToString();

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

Type[] interfaces = myClassType.GetInterfaces();

var dels = new List<object>();
foreach (PropertyInfo property in properties)
{
    if (property.GetCustomAttributes(typeof(AggregationLevelAttribute), true).Any())
    {
        var attributeObj = property.GetCustomAttribute<AggregationLevelAttribute>();
        //Console.WriteLine($"Property name: {property.Name}");
        //Console.WriteLine($"Aggregation level: {attributeObj.AggregationLevel}");

        Type[] aargs = { typeof(IMetaData)};
        DynamicMethod GetProportyValue = new DynamicMethod($"Get{property.Name}",property.PropertyType,aargs,typeof(Program).Module);
        MethodInfo getPropertyMethod = property.GetGetMethod();

        ILGenerator il = GetProportyValue.GetILGenerator();
        il.Emit(OpCodes.Ldarg_0);
        il.EmitCall(OpCodes.Callvirt, getPropertyMethod, null);
        il.Emit(OpCodes.Ret);


        Type delegateType = typeof(Func<,>).MakeGenericType(typeof(IMetaData), property.PropertyType);
        Delegate Rep = GetProportyValue.CreateDelegate(delegateType);

        DelegateGetter<IMetaData>.Getter getter = (IMetaData x) =>
        {
            var result = Rep.DynamicInvoke(obj);
            return KeyValuePair.Create(property.Name, (result == null) ? null : result.ToString());
        };

        dels.Add(getter);

        //Console.WriteLine($"Property name: {property.Name}, Property value: {getter(obj)}");

        
        //Delegate getStringDelegate = methodBuilder.CreateDelegate(delegateType);
    }
}

foreach(var del in dels)
{
    var getter = del as DelegateGetter<IMetaData>.Getter;
    if(getter == null)
    {
        Console.WriteLine("Could not resolve delegate!!!");
        break;
    }
    var res = getter(obj);
    Console.WriteLine($"Property name: {res.Key}, Property value: {res.Value}");
}




Console.WriteLine("Hello, World!");

public class DelegateGetter<T>
{
    public delegate KeyValuePair<string, string?> Getter(T metaData);
}

