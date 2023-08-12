// See https://aka.ms/new-console-template for more information

using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.Json;


var dic = new Dictionary<int, int>() { { 1, 11 }, { 2, 22 } };
dic.Add(1, 12);

int[] numbers = { 1, 2, 3, 4, 5 };

var transformedResults = await numbers
    .Select(async num => await TransformAsync(num))
    .ToListAsync();


Console.WriteLine("Hello, World!");

static async Task<int> TransformAsync(int number)
{
    // Simulate an asynchronous operation
    await Task.Delay(100);
    return number * 2;
}

public class MyClass
{
    public MyClass()
    {
        Id = 0;
        Name = "";
        Values = new double[] { };
    }
    public int? Id { get; set;}
    public string? Name { get; set;}
    public double[]? Values { get; set;}

    public int NumberOfBytes()
    {
        int sz = sizeof(int) + sizeof(double) * Values.Length + sizeof(char) * Name.Length;
        int n = 0;
        for (; sz != 0; n++, sz >>= 1);
        return 1 << n;
    }
}