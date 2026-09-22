using System;
using Core;

EnvironmentReport report = EnvironmentInfo.Collect();

Console.WriteLine("CrossApp - Env Info");
Console.WriteLine(new string('-', 56));
Console.WriteLine($"OS (OSDescription)   : {report.OsDescription}");
Console.WriteLine($"Runtime         : {report.FrameworkDescription}");
Console.WriteLine($"Architecture : {report.ProcessArchitecture}");
Console.WriteLine($"RID (def) : {report.DetectedRid}");
Console.WriteLine($"RID (friom .NET)  : {report.ReportedRid}");
Console.WriteLine($"Catalog         : {report.BaseDirectory}");
Console.WriteLine($"Target build  : {report.BuildNote}");
Console.WriteLine(new string('-', 56));