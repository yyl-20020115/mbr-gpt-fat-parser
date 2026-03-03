using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileManager;

class GPTHeader
{
    private string Signature;//0
    private ulong FirstUsableLBA;//28h
    private ulong LastUsableLBA;//30h
    private Guid DiskGUID;//38h
    private ulong PartitionEntryLBA;//48h
    private uint NumberOfPartitionEntries;//50h
    private uint SizeOfPartitionEntry;//54h

    public GPTHeader()
    {
        Signature = null;//0
        FirstUsableLBA = 0;//28h
        LastUsableLBA = 0;//30h
        DiskGUID = Guid.Empty;//38h
        PartitionEntryLBA = 0;//48h
        NumberOfPartitionEntries = 0;//50h
        SizeOfPartitionEntry = 0;//54h
    }

    public GPTHeader( byte[] sector )
    {
         if (sector == null)
            throw new ArgumentNullException();

         char[] SignatureBuff = new char[8];
         for (int i = 0; i < 8; i++)
         {
             SignatureBuff[i] = (char)sector[i];
         }

         Signature = new String(SignatureBuff);  
         FirstUsableLBA = BitConverter.ToUInt64(sector, 0x28);//28h
         LastUsableLBA = BitConverter.ToUInt64(sector, 0x30);//30h

         byte[] DiskGuidBuff = new byte[16];
         for (int i = 0; i < 16; i++)
         {
             DiskGuidBuff[i] = sector[i+0x38];
         }
         DiskGUID = new Guid(DiskGuidBuff);
        
         PartitionEntryLBA = BitConverter.ToUInt64(sector, 0x48);//48h
         NumberOfPartitionEntries = BitConverter.ToUInt32(sector, 0x50);//50h
         SizeOfPartitionEntry = BitConverter.ToUInt32(sector, 0x54);//54h
    }



    public UInt32 getSizeOfPartitionEntry()
    {
        return SizeOfPartitionEntry;
    }

    public UInt32 getNumberOfPartitionEntries()
    {
        return NumberOfPartitionEntries;
    }

    public UInt64 getPartitionEntryLBA()
    {
        return PartitionEntryLBA;
    }

    public string getDiskGUID()
    {
        return DiskGUID.ToString();
    }

    public string getSignature()
    {
        return Signature;
    }

    public UInt64 getFirstUsableLBA()
    {
        return FirstUsableLBA;
    }

    public UInt64 getLastUsableLBA()
    {
        return LastUsableLBA;
    }

   
    public override string ToString()
    {
        return "Signature: " + Signature + " \nFirstUsableLBA: " + FirstUsableLBA + " \nLastUsableLBA: " + LastUsableLBA + " \nDiskGUID: "
            + DiskGUID.ToString() + " \nPartitionEntryLBA: " + PartitionEntryLBA + " \nNumberOfPartitionEntries: " 
            + NumberOfPartitionEntries + " \nSizeOfPartitionEntry: " + SizeOfPartitionEntry;
    }
    

}
