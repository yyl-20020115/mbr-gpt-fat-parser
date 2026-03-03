using System;

namespace FileManager
{
    internal static class Program
    {
        static void Main()
        {
            PhysicalDrive pd = new PhysicalDrive(@"\\.\PhysicalDrive0");

            for (int i = 0; i < pd.Length; i++)
            {
                Console.WriteLine(pd[i].ToString());
            }




            Console.ReadKey();
        }
    }
}