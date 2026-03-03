using System;

namespace FileManager;

internal static class Program
{
    static void Main(string[] args)
    {
        if(args.Length==0)        {
            Console.WriteLine("Usage: FileManager.exe <PhysicalDriveName>");
            Console.WriteLine("Example: FileManager.exe \\\\.\\PhysicalDrive0");
            return;
        }
        var pd = new PhysicalDrive(args[0]);

        for (int i = 0; i < pd.Length; i++)
        {
            Console.WriteLine(pd[i].ToString());
        }




        Console.ReadKey();
    }
}