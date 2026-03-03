using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileManager
{
    class Boot : IBootable
    {
        private string oem;//original equipment manufacturer
        private string name;
        private int bytesPerSector;
        private byte sectorPerClaster;
        private long serialNumber;
        private byte mediaDescriptor;


        public Boot(byte[] bootSector)
        {
            // ----------------------- define type by name ------------------

            // --------------- oem offset ------------------
            int offset = 0x3;
            char[] oemBuff = new char[8];
            for(int i = 0; i < oemBuff.LongLength; i++)
            {
                oemBuff[i] = (char)bootSector[offset + i];
            }
            oem = new String(oemBuff);
            // ----------------------------------------------

            // ------------ get name for fat32 --------------
            offset = 82;
            char[] nameBuff = new char[8];
            for(int i = 0; i < nameBuff.LongLength; i++)
            {
                nameBuff[i] = (char)bootSector[offset + i];
            }
            name = new String(nameBuff);
            // ----------------------------------------------

            if ( oem == "NTFS    " )
            {
                name = oem;
                oem = "";

                // -------------- NTFS offset ------------------
                bytesPerSector = BitConverter.ToInt16(bootSector, 0xB);
                sectorPerClaster = bootSector[0xD];
                mediaDescriptor = bootSector[0x15];
                serialNumber = BitConverter.ToInt64( bootSector, 0x48);
                // ---------------------------------------------
            }
            else if (name == "FAT32   ")
            {
                // ------------- for FAT32 offset ---------------
                bytesPerSector = BitConverter.ToInt16(bootSector, 11);
                sectorPerClaster = bootSector[13];
                mediaDescriptor = bootSector[21];
                serialNumber = BitConverter.ToInt32(bootSector, 67);
                // ----------------------------------------------
            }
            else
            {
                // ------------ get name for fat32 --------------
                offset = 54;
                nameBuff = new char[8];
                for (int i = 0; i < nameBuff.LongLength; i++)
                {
                    nameBuff[i] = (char)bootSector[offset + i];
                }
                name = new String(nameBuff);
                // ----------------------------------------------
                if (name == "FAT16   ")
                {
                    // ------------ for FAT16 offset --------------
                    bytesPerSector = BitConverter.ToInt16(bootSector, 11);
                    sectorPerClaster = bootSector[13];
                    mediaDescriptor = bootSector[21];
                    serialNumber = BitConverter.ToInt32(bootSector, 39);
                    // --------------------------------------------

                }
                else
                {
                    // ----------- unknown type sector ------------
                    name = "unknown ";
                    oem = "unknown ";
                    bytesPerSector = 0;
                    sectorPerClaster = 0;
                    mediaDescriptor = 0;
                    serialNumber = 0;
                    // --------------------------------------------
                }
            }

        }// ~Boot(byte[] bootSector)


        public string OEM { get; set; }

        public string Name { get { return name; } set { name = value; } }//

        public int BytesPerSector { get; set; }//

        public byte SectorPerClaster { get; set; }//

        public long SerialNumber { get { return serialNumber; } set { serialNumber = value; } }//

        public byte MediaDescriptor { get; set; }//

        public byte NumberOfCopiesOfFAT { get; set; }
        public int SizeOfReserve { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public byte NumOfFatCopy { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int SizeOfFatCopy { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public int BeginOfRoot { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public byte BackupBootSector { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string VolumeNameOfPartition { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    }
}
