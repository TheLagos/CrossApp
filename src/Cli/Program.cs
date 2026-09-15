using System.Runtime.InteropServices;

Console.WriteLine("CrossApp - Cross-Platform Programming Lab");
Console.WriteLine("Student: Bohuslav, Group: FEI-36");
Console.WriteLine(new string('-', 52));
Console.WriteLine($"OS (OSDescription)   : {RuntimeInformation.OSDescription}");
Console.WriteLine($"OS (Environment)     : {Environment.OSVersion}");
Console.WriteLine($"Process Architecture : {RuntimeInformation.ProcessArchitecture}");
Console.WriteLine($".NET Version (CLR)   : {Environment.Version}");
Console.WriteLine($"Runtime              : {RuntimeInformation.FrameworkDescription}");
Console.WriteLine($"Application Directory: {AppContext.BaseDirectory}");
Console.WriteLine($"Current Directory    : {Environment.CurrentDirectory}");
Console.WriteLine(new string('-', 52));
Console.WriteLine("Domain: Order ( Customer, Product, Order)");