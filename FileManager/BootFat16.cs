using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileManager
{
    class BootFat16 : IBootable
    {
        private string oem;//original equipment manufacturer
        private int bytesPerSector;
        private byte sectorPerClaster;
        private byte mediaDescriptor;
        private int sizeOfReserve;
        private byte numOfFatCopy;
        private int sizeOfFatCopy;//36-37
        private long serialNumber;
        private string volumeNameOfPartition;
        private string name;
        //uniq
        private int numOfHiddenSectors;
        private int maxFilesInRoot;
        private int numSectorsInPartition;


        public BootFat16( byte[] bootSector )
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

            maxFilesInRoot = BitConverter.ToInt16(bootSector, 17);
            mediaDescriptor = bootSector[0x15];
            sizeOfFatCopy = BitConverter.ToInt16(bootSector, 22);
            numOfHiddenSectors = BitConverter.ToInt32( bootSector, 28 );
            numSectorsInPartition = BitConverter.ToInt16(bootSector, 32);           
            serialNumber = BitConverter.ToInt32(bootSector, 39);

            // ------------ name of partition ---------------
            offset = 43;
            char[] partitionBuff = new char[11];
            for (int i = 0; i < partitionBuff.LongLength; i++)
            {
                partitionBuff[i] = (char)bootSector[offset + i];
            }
            volumeNameOfPartition = new String(partitionBuff);
            // ----------------------------------------------

            // ------------- indetifier offset -------------
            offset = 54;
            char[] nameBuff = new char[8];
            for (int i = 0; i < nameBuff.LongLength; i++)
            {
                nameBuff[i] = (char)bootSector[offset + i];
            }
            name = new String(nameBuff);
            // ----------------------------------------------

        }//~ BootFat16

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
            get { return bytesPerSector; }
            set { bytesPerSector = value; }
        }

        public byte SectorPerClaster
        {
            get { return sectorPerClaster; }
            set { sectorPerClaster = value; }
        }

        public long SerialNumber
        {
            get { return serialNumber; }
            set { serialNumber = value; }
        }

        public byte MediaDescriptor
        {
            get { return mediaDescriptor; }
            set { mediaDescriptor = value; }
        }

        //public byte NumberOfCopiesOfFAT
        //{
        //    get;
        //    set;
        //}

        private int SizeOfReserve
        {
            get { return sizeOfReserve; }
            set { sizeOfReserve = value; }
        }
        private byte NumOfFatCopy
        {
            get { return numOfFatCopy; }
            set { numOfFatCopy = value; }
        }
        private int SizeOfFatCopy
        {
            get { return sizeOfFatCopy; }
            set { sizeOfFatCopy = value; }
        }

        private int MaxFilesInRoot
        {
            get { return maxFilesInRoot; }
            set { maxFilesInRoot = value; }
        }
       
        private string VolumeNameOfPartition
        {
            get { return volumeNameOfPartition; }
            set { volumeNameOfPartition = value; }
        }

        public int NumOfHiddenSectors
        {
            get { return numOfHiddenSectors; }
            set { numOfHiddenSectors = value; }
        }

        public int NumSectorsInPartition
        {
            get { return numSectorsInPartition; }
            set { numSectorsInPartition = value; }
        }

        int IBootable.SizeOfReserve { get => SizeOfReserve; set => SizeOfReserve = value; }
        byte IBootable.NumOfFatCopy { get => NumOfFatCopy; set => NumOfFatCopy = value; }
        int IBootable.SizeOfFatCopy { get => SizeOfFatCopy; set => SizeOfFatCopy = value; }
        public int BeginOfRoot { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public byte BackupBootSector { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        string IBootable.VolumeNameOfPartition { get => VolumeNameOfPartition; set => VolumeNameOfPartition = value; }
    }//~class
}
