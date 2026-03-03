using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
                if ( v.Name[0] == volumeName[0] )
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