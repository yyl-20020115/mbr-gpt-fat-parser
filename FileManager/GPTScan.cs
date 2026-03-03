using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32.SafeHandles;
using System.IO;


namespace FileManager;

class GPTScan : IScan
{
    private MBR Mbr;
    private GPTHeader Header;
    private List<ISection> Sections = new List<ISection>();
    
    public int Length
    {
        get { return Sections.Count; }
    }


    //IBootable Indexer return address of begining for every logical volume
    public ISection this[int index]
    {
        get 
        { 
            return Sections[index]; 
        }
    }

    public GPTScan()
    {
        //...
    }

    public GPTScan( string physicalDriveName )
    {
        const uint SectorSize = 512;          
        byte[] buff = new byte[512];
        uint nBytesRead;

        SafeFileHandle shFile = WinApi.CreateFile( physicalDriveName, FileAccess.Read, FileShare.Read, (IntPtr)0, FileMode.Open, 0, (IntPtr)0);
       
        //LBA0
        WinApi.ReadFile(shFile, buff, 512, out nBytesRead, (IntPtr)0);
        
        //LBA1 ---------------------------HEADER----------------------------------------------
        Mbr = new MBR(buff);

        UInt64 addr = 512;
        int hPart = (int)(addr >> 32);
        uint lPart = (uint)addr;
        WinApi.SetFilePointer(shFile, lPart, out hPart, (uint)SeekOrigin.Begin);
        if (shFile.IsInvalid)
        {
            throw new InvalidDataException();
        }

        try
        {
            WinApi.ReadFile(shFile, buff, SectorSize, out nBytesRead, (IntPtr)0);
        }
        catch (FileLoadException ex)
        {
            ex.ToString();//!!!!!!!!!!!!!!!!! shit
        }

        Header = new GPTHeader(buff);
        //---------------------------------------------------------------------------- ~Header


        //LBA2...X
        for( uint i = 0; i < Header.getNumberOfPartitionEntries() / ( SectorSize / Header.getSizeOfPartitionEntry()); i++)
        {
            addr += 512;
            hPart = (int)(addr >> 32);
            lPart = (uint)addr;
            WinApi.SetFilePointer(shFile, lPart, out hPart, (uint)SeekOrigin.Begin);
                
            WinApi.ReadFile(shFile, buff, 512, out nBytesRead, (IntPtr)0);
           
            int offset = 0;//офсет внутри сектора
            for (int j = 0; j < 512 / Header.getSizeOfPartitionEntry(); j++)
            {
                //---Анализируем гуид на предмет неиспользуемой записи 
                byte[] guidBuff = new byte [16];
                for (int guidIndex = 0; guidIndex < 16; guidIndex++)
                {
                    guidBuff[guidIndex] = buff[guidIndex + offset]; 
                }
                Guid guid = new Guid(guidBuff);
                //----------------------------------------------------------
                if( !guid.Equals(Guid.Empty) )
                {
                    Sections.Add(new GPTSection(buff, offset));
                }
                offset += (int)Header.getSizeOfPartitionEntry();
            }
         }
        shFile.Close();
    }//GPTScan( string PhysicalDriveName )

    public bool Scan(string physicalDriveName)
    {
        const uint SectorSize = 512;
        byte[] buff = new byte[512];
        uint nBytesRead;

        SafeFileHandle shFile = WinApi.CreateFile(physicalDriveName, FileAccess.Read, FileShare.Read, (IntPtr)0, FileMode.Open, 0, (IntPtr)0);

        //LBA0
        WinApi.ReadFile(shFile, buff, 512, out nBytesRead, (IntPtr)0);

        //LBA1
        Mbr = new MBR(buff);
        if (!Mbr.isProtective())
        {
            Header = new GPTHeader();
            return false;
        }

        UInt64 addr = 512;
        int hPart = (int)(addr >> 32);
        uint lPart = (uint)addr;
        WinApi.SetFilePointer(shFile, lPart, out hPart, (uint)SeekOrigin.Begin);

        try
        {
            WinApi.ReadFile(shFile, buff, SectorSize, out nBytesRead, (IntPtr)0);
        }
        catch (FileLoadException ex)
        {
            ex.ToString();//!!!!!!!!!!!!!!!!! shit
        }

        Header = new GPTHeader(buff);

        //LBA2...X
        for (uint i = 0; i < Header.getNumberOfPartitionEntries() / (SectorSize / Header.getSizeOfPartitionEntry()); i++)
        {
            addr += 512;
            hPart = (int)(addr >> 32);
            lPart = (uint)addr;
            WinApi.SetFilePointer(shFile, lPart, out hPart, (uint)SeekOrigin.Begin);

            WinApi.ReadFile(shFile, buff, 512, out nBytesRead, (IntPtr)0);

            int offset = 0;//офсет внутри сектора
            for (int j = 0; j < 512 / Header.getSizeOfPartitionEntry(); j++)
            {
                //---Анализируем гуид на предмет неиспользуемой записи 
                byte[] guidBuff = new byte[16];
                for (int guidIndex = 0; guidIndex < 16; guidIndex++)
                {
                    guidBuff[guidIndex] = buff[guidIndex + offset];
                }
                Guid guid = new Guid(guidBuff);
                //----------------------------------------------------------
                if (!guid.Equals(Guid.Empty))
                {
                    Sections.Add(new GPTSection(buff, offset));
                }
                offset += (int)Header.getSizeOfPartitionEntry();
            }
        }
        shFile.Close();
        return true;
    }//Scan( string PhysicalDriveName )


    public GPTHeader getHeader()
    {
        return Header;
    }

    public List<ISection> getGPTSectionList()
    {
        return Sections;
    }

    public override string ToString()
    {
        string buffString = "   Protective MBR\n" + Mbr.ToString() + "\n    Header\n" + Header.ToString();
        for (int i = 0; i < Sections.Count; i++ )
        {
            buffString += "\n   Section " + i + "\n" + Sections[i].ToString();
        }

        return buffString;
    }
        

}

