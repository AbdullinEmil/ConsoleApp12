using System;
using System.IO;
using System.Linq;

[AttributeUsage(AttributeTargets.Property)]
public class ParamsAttribute : Attribute
{
    public string IniFileName { get; }

    public ParamsAttribute(string iniFileName)
    {
        IniFileName = iniFileName;
    }
}
public class MyClass
{
    [ParamsAttribute("config1.ini")]
    public string Property1 { get; set; }

    [ParamsAttribute("config2.ini")]
    public int Property2 { get; set; }

    [ParamsAttribute("config3.ini")]
    public bool Property3 { get; set; }
}
public static class IniReader
{
    public static string ReadValue(string iniFileName, string key)
    {
        using (var reader = new StreamReader(iniFileName))
        {
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                if (line.StartsWith(key + "="))
                {
                    return line.Substring(key.Length + 1).Trim();
                }
            }
        }
        return null;
    }
}
public class Program
{
    public static void Main(string[] args)
    {
        MyClass myClass = new MyClass();
        var properties = typeof(MyClass).GetProperties();
        foreach (var property in properties)
        {
            var paramsAttribute = property.GetCustomAttributes(typeof(ParamsAttribute), false).FirstOrDefault() as ParamsAttribute;
            if (paramsAttribute != null)
            {
                string value = IniReader.ReadValue(paramsAttribute.IniFileName, property.Name);
                property.SetValue(myClass, Convert.ChangeType(value, property.PropertyType));
                Console.WriteLine($"Свойство {property.Name}: {property.GetValue(myClass)}");
            }
            else
            {
                Console.WriteLine($"Атрибут ParamsAttribute не найден для свойства {property.Name}.");
            }
        }
    }
}