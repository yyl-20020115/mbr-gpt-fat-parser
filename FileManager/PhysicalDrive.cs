using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Win32.SafeHandles;
using System.IO;

namespace FileManager;

class PhysicalDrive
{
    string name;
    IScan Scanner;
    List<Volume> volumes = [];
    //ISystemic
    //тип сенгментації

    //-------------------------------------------------------Constructor(string)------------------
    public PhysicalDrive(string driveName)
    {

        const uint SectorSize = 512;
        byte[] buff = new byte[512];
        uint nBytesRead;

        name = driveName;
        SafeFileHandle shFile = WinApi.CreateFile(driveName, FileAccess.Read, FileShare.Read, (IntPtr)0, FileMode.Open, 0, (IntPtr)0);
        if (shFile.IsInvalid)
            throw new DriveNotFoundException("Drive" + driveName + "is not avalible");

        WinApi.ReadFile(shFile, buff, SectorSize, out nBytesRead, (IntPtr)0);
        if (shFile.IsInvalid)
            throw new InvalidDataException("Reading data form " + driveName + " is not avalible");

        List<(uint, uint, string)> partitions = [];
        for (uint i = 1; ; i++)
        {
            WinApi.SetFilePointer(shFile, i * SectorSize, out var high, 0);

            WinApi.ReadFile(shFile, buff, SectorSize, out nBytesRead, (IntPtr)0);
            if (shFile.IsInvalid)
                throw new InvalidDataException("Reading data form " + driveName + " is not avalible");
            var signature0 = BitConverter.ToUInt32(buff, 0);
            if (signature0 == 0) break;
            var signature1 = BitConverter.ToUInt32(buff, 4);
            var offset = BitConverter.ToUInt32(buff, 8) * SectorSize;
            var length = BitConverter.ToUInt32(buff, 12) * SectorSize;
            var name = Encoding.ASCII.GetString(buff, 16, (int)SectorSize - 16).TrimEnd('\0');
            partitions.Add((offset, length, name));
            Console.WriteLine($"{name}: offset={offset:X8}, length={length:X8}");
        }
        foreach (var partition in partitions)
        {
            using var stream = new FileStream(partition.Item3 + ".bin", FileMode.Create, FileAccess.Write);
            WinApi.SetFilePointer(shFile, partition.Item1, out var high, 0);
            for (uint i = 0; i < partition.Item2; i += SectorSize)
            {
                WinApi.ReadFile(shFile, buff, SectorSize, out nBytesRead, (IntPtr)0);
                stream.Write(buff, 0, (int)SectorSize);
            }
        }




        if (false)
        {

            //---------------------------------------------------
            MBR protectiveMbr = new MBR(buff);

            if (protectiveMbr.isProtective())
            {
                Scanner = new GPTScan(name);
            }
            else
            {
                Scanner = new MBRScan(name);
            }
            //---------------------------------------------------

            //---------------------------------------------------
            for (int i = 0; i < Scanner.Length; i++)
            {
                volumes.Add(new Volume(driveName, Scanner[i]));
            }
            //---------------------------------------------------
        }
        shFile.Close();
    }
    //------------------------------------------------------------------------- ~PhysicalDrive(string driveName)

    public Volume this[int index]
    {
        get
        {
            try
            {
                return volumes[index];
            }
            catch (ArgumentOutOfRangeException)
            {
                throw;//throw up
            }
        }
    }

    public Volume this[string volumeName]
    {
        get
        {

            foreach (Volume v in volumes)
            {
                //compare only letter
                if (v.Name[0] == volumeName[0])
                    return v;
            }
            throw new VolumeNotFoundException("Volume " + volumeName + " not found");
        }
    }

    public int Length
    {
        get { return volumes.Count; }
    }


    //public bool isOneOf("C:\");




}