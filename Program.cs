// See https://aka.ms/new-console-template for more information

var outList = new List<int>() { 1,2,3,4 };
var inList = new List<int>() { 11, 22, 33, 44 };

var ll = new List<List<int>>() { outList, inList };

var fff = ll.SelectMany(x => x).ToList();






Console.WriteLine("Hello, World!");





public class Key
{
    public int Field;
    public override bool Equals(object? obj)
    {
        var k = obj as Key;
        return k.Field == Field;
    }

    public override int GetHashCode()
    {
        return Field;
    }
}