using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileManager;

class BootFat32 : IBootable
{

    private string oem;//original equipment manufacturer  
    private int bytesPerSector;
    private byte sectorPerClaster;
    private byte mediaDescriptor;
    private int sizeOfReserve;
    private byte numOfFatCopy;      
    private int sizeOfFatCopy;//36-37
    private int beginOfRoot;
    private byte backupBootSector;
    private long serialNumber;
    private string volumeNameOfPartition;
    private string name;


    public BootFat32(byte[] bootSector)
    {
        // --------------- oem offset ------------------
        int offset = 0x3;
        char[] oemBuff = new char[8];
        for (int i = 0; i < oemBuff.LongLength; i++)
        {
            oemBuff[i] = (char)bootSector[offset + i];
        }
        oem = new String(oemBuff);
        // ----------------------------------------------

        bytesPerSector = BitConverter.ToInt16(bootSector, 11);
        sectorPerClaster = bootSector[13];
        sizeOfReserve = BitConverter.ToInt16(bootSector, 14);
        numOfFatCopy = bootSector[16];

        mediaDescriptor = bootSector[0x15];
        sizeOfFatCopy = BitConverter.ToInt16( bootSector, 36 );
        backupBootSector = bootSector[50];
        beginOfRoot = BitConverter.ToInt16(bootSector, 44);
        serialNumber = BitConverter.ToInt32(bootSector, 67);

        // ------------ name of partition ---------------
        offset = 71;
        char[] partitionBuff = new char[11];
        for (int i = 0; i < partitionBuff.LongLength; i++)
        {
            partitionBuff[i] = (char)bootSector[offset + i];
        }
        volumeNameOfPartition = new String( partitionBuff );
        // ----------------------------------------------

        // --------------- indetifier offset ------------------
        offset = 82;
        char[] nameBuff = new char[8];
        for (int i = 0; i < nameBuff.LongLength; i++)
        {
            nameBuff[i] = (char)bootSector[offset + i];
        }
        name = new String(nameBuff);
        // ----------------------------------------------


    }// ~public BootFat32(byte[] bootSector)


    public string OEM 
    {
        get { return oem; }
        set { oem = value; } 
    }

    public string Name 
    {
        get { return name; }
        set { name = value; } 
    }

    public int BytesPerSector 
    { 
        get { return bytesPerSector;} 
        set { bytesPerSector = value;} 
    }

    public byte SectorPerClaster 
    { 
        get { return sectorPerClaster;} 
        set { sectorPerClaster = value;} 
    }

    public long SerialNumber 
    { 
        get { return serialNumber;} 
        set { serialNumber = value;} 
    }

    public byte MediaDescriptor 
    { 
        get { return mediaDescriptor;} 
        set { mediaDescriptor = value;} 
    }

    //public byte NumberOfCopiesOfFAT 
    //{ 
    //    get; 
    //    set; 
    //} 

    public int SizeOfReserve
    {
        get { return sizeOfReserve; } 
        set { sizeOfReserve = value; } 
    }
    public byte NumOfFatCopy
    {
        get { return numOfFatCopy; }
        set { numOfFatCopy = value; } 
    }
    public int SizeOfFatCopy 
    {
        get { return sizeOfFatCopy;} 
        set { sizeOfFatCopy = value;} 
    }
    public int BeginOfRoot
    {
        get { return beginOfRoot; } 
        set { beginOfRoot = value;} 
    }
    public byte BackupBootSector
    {
        get { return backupBootSector;} 
        set { backupBootSector = value;} 
    }

    public string VolumeNameOfPartition
    {
        get { return volumeNameOfPartition; }
        set { volumeNameOfPartition = value; } 
    }
    


}