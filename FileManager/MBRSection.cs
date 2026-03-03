using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileManager;

class MBRSection : ISection
{
    private Guid PartitionTypeGuid;
    private Guid UniquePartitionGUID;//10h
    private ulong StartingLBA;//20h
    private ulong EndingLBA;//28h
    private long Attributes;//30
    private string PartitionName;//38

    //-------------------------------Property--------------------------------------       
    public bool isBootable
    {
        get { return bootable == 0x80 ? true : false; }
    }

    public byte Type
    {
        get { return type; }
        set { type = value; }
    }
    // ----------------------------------------------------------------------------- ~PROPERTY

    // --------------------------------MBR Fields---------------------------------------
    private byte bootable;
    private byte type;
    private uint first_sector;
    private uint number_of_sectors;
    //------------------------------------------------------------------------------ ~FIELDS   

    public MBRSection() 
    {
        PartitionTypeGuid = Guid.Empty;
        UniquePartitionGUID = Guid.Empty;//10h
        StartingLBA = 0U;//20h
        EndingLBA = 0U;//28h
        Attributes = 0L;//30
        PartitionName = null;//38

        bootable = 0;
        type = 0;
        first_sector = 0;
        number_of_sectors = 0;
    }

    public MBRSection(byte[] data)
    {
        PartitionTypeGuid = Guid.Empty;
        UniquePartitionGUID = Guid.Empty;//10h
        StartingLBA = 0U;//20h
        EndingLBA = 0U;//28h
        Attributes = 0L;//30
        PartitionName = null;//38

        this.bootable = (byte)BitConverter.ToChar(data, 0x0);
        this.type = (byte)BitConverter.ToChar(data,  0x4);
        this.first_sector = BitConverter.ToUInt32(data, 0x8);
        this.number_of_sectors = BitConverter.ToUInt32(data,  0xC);
    }

    public MBRSection( byte[] data , int offset )
    {
        this.bootable = (byte)BitConverter.ToChar(data, offset + 0x0 );
        this.type = (byte)BitConverter.ToChar(data, offset + 0x4);
        this.first_sector = BitConverter.ToUInt32( data, offset + 0x8);
        this.number_of_sectors = BitConverter.ToUInt32(data, offset + 0xC );

        PartitionTypeGuid = Guid.Empty;
        UniquePartitionGUID = Guid.Empty;//10h
        StartingLBA = 0U;//20h
        EndingLBA = 0U;//28h
        Attributes = 0L;//30
        PartitionName = null;//38
    }

    public byte getFileSystemType()
    {
        return type;
    }
    
    public void setFirstSector( uint first_sector ){
        this.first_sector = first_sector;
    }

    public ulong getFirstSector()
    {
        return (ulong)first_sector;
    }

    public void setNumberOfSectors(uint number_of_sectors)
    {
        this.number_of_sectors = number_of_sectors;
    }
    public ulong getSize()
    {
        return number_of_sectors;
    }

    public long getAttributes()
    {
        return Attributes;
    }

    public string getPartitionName()
    {
        return PartitionName;
    }

    public Guid getPartitionTypeGuid()
    {
        return PartitionTypeGuid;
    }

    public Guid getUniquePartitionGuid()
    {
        return UniquePartitionGUID;
    }
    
    public override string ToString()
    {
        return "Bootable: " + bootable + " Code: 0x" + type.ToString("X") + " first_sector: " + first_sector.ToString("X") 
            + " number_of sectors: " + number_of_sectors;
    }
    

}
