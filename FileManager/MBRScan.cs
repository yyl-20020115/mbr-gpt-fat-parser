using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32.SafeHandles;
using System.IO;

namespace FileManager;

class MBRScan : IScan
{
    private MBR Mbr;
    private List<MBR> EbrList = new List<MBR>();
    private List<ISection> Sections = new List<ISection>();


    public int Length
    {
        get { return Sections.Count(); }
    }

    //IBootable Indexer return address of begining for every logical volume
    public ISection this[int index]
    {
        get { return Sections[index]; }
    }


    public MBRScan( string physicalDriveName )
    {
        const uint SectorSize = 512;
        byte[] buff = new byte[512];
        uint nBytesRead;
        

        // Create handle to physical drive
        SafeFileHandle shFile = WinApi.CreateFile(physicalDriveName, FileAccess.Read, FileShare.Read, (IntPtr)0, FileMode.Open, 0, (IntPtr)0);
        if (shFile.IsInvalid)
            throw new DriveNotFoundException(physicalDriveName + " not found");


        // - - - - - - - - - - - - main MBR - - - - - - - - - - - - - - - - - - - -
        WinApi.ReadFile(shFile, buff, SectorSize, out nBytesRead, (IntPtr)0);
        Mbr = new MBR(buff);
        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - ~mainMBR
        shFile.Close();
        //add to Sections primary sections
        for (int sectionNumber = 0; sectionNumber < 4; sectionNumber++)
        {
            if (Mbr[ sectionNumber ].Type == 0xF || Mbr[ sectionNumber ].Type == 0x5)
            {

                ulong baseAddr = Mbr[sectionNumber].getFirstSector();
                EbrScan(physicalDriveName, baseAddr, 0);
            }
            else
            {
                Sections.Add( Mbr[sectionNumber] );
            }

        }

    }// ~public MBRScan( string physicalDriveName )-----------------------------------------

    public bool Scan( string physicalDriveName )
    {
        const uint SectorSize = 512;
        byte[] buff = new byte[512];
        uint nBytesRead;
        

        // Create handle to physical drive
        SafeFileHandle shFile = WinApi.CreateFile(physicalDriveName, FileAccess.Read, FileShare.Read, (IntPtr)0, FileMode.Open, 0, (IntPtr)0);

        // - - - - - - - - - - - - main MBR - - - - - - - - - - - - - - - - - - - -
        WinApi.ReadFile(shFile, buff, SectorSize, out nBytesRead, (IntPtr)0);
        Mbr = new MBR(buff);
        if( Mbr.isProtective() )
            return true;
        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - - ~mainMBR
        shFile.Close();
        //add to Sections primary sections
        for (int sectionNumber = 0; sectionNumber < 4; sectionNumber++)
        {
            if (Mbr[ sectionNumber ].Type == 0xF || Mbr[ sectionNumber ].Type == 0x5)
            {

                ulong baseAddr = Mbr[sectionNumber].getFirstSector();
                EbrScan(physicalDriveName, baseAddr, 0);
            }
            else
            {
                Sections.Add( Mbr[sectionNumber] );
            }
            
        }
        return false;
    }// -------------------------------------------------------------------- ~Scan()

    private bool EbrScan( string physicalDriveName, ulong baseAddr, ulong relAddr )
    {
        const uint SectorSize = 512;
        byte[] buff = new byte[512];
        uint nBytesRead;

        SafeFileHandle shFile = WinApi.CreateFile(physicalDriveName, FileAccess.Read, FileShare.ReadWrite, (IntPtr)0, FileMode.Open, 0, (IntPtr)0);
        if (shFile.IsInvalid)
            return true;

        UInt64 addr = (baseAddr + relAddr) * SectorSize;
        int hPart = (int)(addr >> 32);
        uint lPart = (uint)addr;

        WinApi.SetFilePointer( shFile, lPart, out hPart, (uint)SeekOrigin.Begin );
        if (shFile.IsInvalid)
            return true;

        WinApi.ReadFile(shFile, buff, SectorSize, out nBytesRead, (IntPtr)0);
        if (shFile.IsInvalid)
            return true;
        shFile.Close();

        EbrList.Add( new MBR(buff) );//ext 


        Sections.Add(EbrList[EbrList.Count - 1][0]);
        uint fs = (uint)(baseAddr + relAddr + Sections[Sections.Count - 1].getFirstSector());
        Sections[Sections.Count - 1].setFirstSector(fs);
        
        if ( EbrList[EbrList.Count - 1][1].Type != 0x0 )
        {             
            EbrScan(physicalDriveName, baseAddr, EbrList[EbrList.Count - 1][1].getFirstSector());
            return false;
        }
        else
        {
            return false;
        }      
    }// ~EbrScan

    public MBR getPrimaryMbr()
    {
        return Mbr;
    }

    public List<MBR> getEbrList()
    {
        return EbrList;
    } 

    public override string ToString()
    {
        string buffString;
        buffString = Mbr.ToString();
        for (int i = 0; i < EbrList.Count; i++ )
        {
            buffString += "\n" + EbrList[i].ToString();
        }

        return buffString;
    }

}
