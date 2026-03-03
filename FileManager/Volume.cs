using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Microsoft.Win32.SafeHandles;


namespace FileManager;

class Volume
{
    string name;
    ulong beginAdders;
    ulong size;
    byte fileSystemType;
    ulong serialNumber;
    Guid uniquePartitionGuid;
    Guid partitionTypeGuid;

    IBootable BPB;
    //VolumeBootSector BPB;
    //ISystemic FileSystem;

    public override string ToString() => "\nName: " + name + "\nPartitonTypeGUID: " + partitionTypeGuid.ToString() + "\nBegin address: " + beginAdders + "\nSize: " + size + "\nFile System: " + fileSystemType +
            "\nSerial Number: " + serialNumber + "\nUniquePartitionGuid: " + uniquePartitionGuid.ToString()
             ;


    public Volume( string physicalDriveName, ISection section )
    {
        //-----------inits from section-------------
        beginAdders = section.getFirstSector();
        size = section.getSize();
        uniquePartitionGuid = section.getUniquePartitionGuid();
        partitionTypeGuid = section.getPartitionTypeGuid();
        fileSystemType = section.getFileSystemType();
        uniquePartitionGuid = section.getUniquePartitionGuid();
        partitionTypeGuid = section.getPartitionTypeGuid();
        //------------------------------------------

        
        //-------------------------------------BOOT SECTOR---------------------------------------------
        const uint SectorSize = 512;
        byte[] buff = new byte[512];
        uint nBytesRead;
        
        SafeFileHandle shFile = WinApi.CreateFile(physicalDriveName, FileAccess.Read, FileShare.Read, (IntPtr)0, FileMode.Open, 0, (IntPtr)0);
        if (shFile.IsInvalid)
            throw new IOException();
        
        UInt64 addr = beginAdders * SectorSize;
        int hPart = (int)(addr >> 32);
        uint lPart = (uint)addr;

        WinApi.SetFilePointer(shFile, lPart, out hPart, (uint)SeekOrigin.Begin);
        if (shFile.IsInvalid)
            throw new IOException();

        //read boot
        WinApi.ReadFile(shFile, buff, SectorSize, out nBytesRead, (IntPtr)0);
        if (shFile.IsInvalid)
            throw new IOException();

        // def common fields
        Boot boot = new Boot(buff);
        // ------------------------------------------------------------------------------------------------- BOOT SECTOR

        // ------------------- init from boot ----------------------------------
        serialNumber = (ulong)boot.SerialNumber;

        // ----------------------------------------------------------------------

        
        // -------- get name by means of guid or serial number ------------------
        if (section.getUniquePartitionGuid() == Guid.Empty)
        {
            name = getDiskLetter(serialNumber);
        }
        else
        {
            name = getDiskLetter( uniquePartitionGuid );
            serialNumber = 0U;
        }
        //-------------------------------------------------------------- ~ getName

        
        //------------------------------------------------------------------------------------- FILL BOOT SECTOR

                    
        shFile.Close();
    } // ~Volume( string physicalDriveName, ISection section )

    //private ulong getSerialNumber(byte[] boot, byte type)
    //{
    //    ulong SerialNumber;
    //    //Читаем серийный номер из бут-сектора в зависимости от файловой системы
    //    switch (type)
    //    {
    //        case 0x01:
    //        case 0x04:
    //        case 0x06:
    //        case 0x0E:
    //        case 0x14:
    //        case 0x16:
    //        case 0x1E:
    //            SerialNumber = BitConverter.ToUInt32(boot, 0x27);//39
    //            break;
    //        case 0x0B:
    //        case 0x0C:
    //        case 0x11:
    //        case 0x1B:
    //        case 0x1C:
    //            SerialNumber = BitConverter.ToUInt32(boot, 0x43);//67
    //            break;
    //        case 0x07:
    //            SerialNumber = BitConverter.ToUInt64(boot, 0x48);
    //            break;
    //        default:
    //            SerialNumber = 0;
    //            break;
    //    }
    //    return SerialNumber;
    //}


    private string getDiskLetter(ulong serialNumber)
    {
        //if (serialNumber == 0) return "*:\\";

        //uint BitMaskOfNames;
        DriveType driveType;
        uint SNvolume;
       

        //получаем битовую маску наименования логических дисков
        char[] NamesBuff = new char[128];


        WinApi.GetLogicalDriveStrings(128, NamesBuff);
        char[] DriveName = new char[4];

        for (int BuffOffset = 0; BuffOffset < NamesBuff.Length; BuffOffset += 4)
        {
            for (int NameOffset = 0; NameOffset < 4; NameOffset++)
            {
                DriveName[NameOffset] = NamesBuff[NameOffset + BuffOffset];
            }  
            //--------------        
            driveType = WinApi.GetDriveType( new String(DriveName) );

            if (driveType == DriveType.Fixed || driveType == DriveType.Removable)
            {
                //WinApi.GetVolumeInformation(DriveName, nameBuffer, 150,
                //    out SNvolume, MCLength, FileSF, SysNameBuffer, SysNameBuffer.Length );
                WinApi.GetVolumeInformation(DriveName, null, 0, out SNvolume, 0, 0, null, 0);
                if (SNvolume == (uint)serialNumber)
                {          
                    return new String(DriveName);
                }
            }
            
            //--------------
        }
        return new String(DriveName);
    }

    private string getDiskLetter(Guid uniqueGuid)
    {
        DriveType driveType;
        char[] NamesBuff = new char[128];

        WinApi.GetLogicalDriveStrings(128, NamesBuff);
        char[] DriveName = new char[4];

        for (int BuffOffset = 0; BuffOffset < NamesBuff.Length; BuffOffset += 4)
        {
            //-----cut drive name from string with drive names------------
            for (int NameOffset = 0; NameOffset < 4; NameOffset++)
            {
                DriveName[NameOffset] = NamesBuff[NameOffset + BuffOffset];
            }
            //-------------------------------------------------------------

            if (DriveName[0] == '\0')
            {
                DriveName[0] = '*'; DriveName[1] = ':'; DriveName[2] = '\\'; DriveName[3] = '\0';
                break;
            }  

            //-------------- 
            driveType = WinApi.GetDriveType(new String(DriveName));

            if (driveType == DriveType.Fixed || driveType == DriveType.Removable)
            {    
                string guidOfDrive;
                guidOfDrive = WinApi.GetVolumeName(new String(DriveName));
                guidOfDrive = guidOfDrive.Substring( 11 );//del beg
                guidOfDrive = guidOfDrive.Remove((guidOfDrive.Length - 2));//del end
           
                if ( uniqueGuid.ToString() == guidOfDrive)
                {
                    return new String(DriveName);
                }
            }
            //--------------
        }
        return new String(DriveName);
    }

    public string Name
    {
        get { return name; }
    }

    public ulong BeginAddress
    {
        get { return beginAdders; }
    }

    public ulong Size
    {
        get { return size; }
    }
   
    //string Name;
    //ulong BeginAdders;
    //ulong Size;
    //byte FileSystemType;
    //ulong SerialNumber;
    //Guid UniquePartitionGuid;
    //Guid PartitionTypeGuid;


}
