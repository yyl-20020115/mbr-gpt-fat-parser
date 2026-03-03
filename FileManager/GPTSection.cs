using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileManager;

class GPTSection : ISection
{
    //gtp data (cammelCase)
    private Guid PartitionTypeGuid;
    private Guid UniquePartitionGUID;//10h
    private ulong StartingLBA;//20h
    private ulong EndingLBA;//28h
    private long Attributes;//30
    private string PartitionName;//38

    //mbr data (under_scope)
    private byte bootable;
    private byte type;
    private uint first_sector;
    private uint number_of_sectors;
    //Reserved not used

    public bool isBootable
    {
        get { return bootable == 0x80 ? true : false; }
    }

    public GPTSection()
    {
        PartitionTypeGuid = new Guid();
        UniquePartitionGUID = new Guid();//10h
        StartingLBA = 0;//20h
        EndingLBA = 0;//28h
        Attributes = 0;//30
        PartitionName = null;//38

        bootable = 0;
        type = 0;
        first_sector = 0;
        number_of_sectors = 0;
    }

    //section - is byte data, offset from begin of sector 
    public GPTSection( byte[] section, int offset )
    {
        bootable = 0;
        type = 0;
        first_sector = 0;
        number_of_sectors = 0;

        byte[] GuidBuff = new byte[16];

        for (int i = 0; i < GuidBuff.Length; i++)
        {
            GuidBuff[i] = section[i+offset];
        }
        PartitionTypeGuid = new Guid(GuidBuff);

        for (int i = 0; i < GuidBuff.Length; i++)
        {
            GuidBuff[i] = section[i + offset + 16];
        }
        UniquePartitionGUID = new Guid(GuidBuff);

        StartingLBA = BitConverter.ToUInt64(section, 0x20 + offset);//20h
        EndingLBA = BitConverter.ToUInt64(section, 0x28 + offset);//28h
        Attributes = BitConverter.ToInt64(section, 0x30 + offset);//30
        
        char[] PartitionNameBuff = new char [72]; 
        for (int i = 0; i < PartitionNameBuff.Length; i++)
        {
            PartitionNameBuff[i] = (char)section[i+offset + 0x38];
        } 
        PartitionName = new String(PartitionNameBuff);//38
    }

    public void setFirstSector(uint first_sector)
    {
        this.first_sector = first_sector;
    }

    public Guid getPartitionTypeGuid()
    {
        return PartitionTypeGuid;
    }

    public Guid getUniquePartitionGuid()
    {
        return UniquePartitionGUID;
    }
    
    public ulong getFirstSector()
    {
        return  StartingLBA;
    }

    public ulong getEndingLBA()
    {
        return EndingLBA;
    }

    public long getAttributes()
    {
        return Attributes;
    }

    public string getPartitionName()
    {
        return PartitionName;
    }

    public ulong getSize()
    {
        return EndingLBA - StartingLBA;
    }

    public byte getFileSystemType()
    {
        return type;
    }

    public override string ToString()
    {
        return "PartitionTypeGuid: " + PartitionTypeGuid.ToString() + "\nUniquePartitionGUID: " + UniquePartitionGUID.ToString() + "\nStartingLBA: " +
            StartingLBA + "\nEndingLBA: " + EndingLBA + "\nAttributes: " + Attributes + "\nPartitionName: " +  PartitionName;
    }
}
