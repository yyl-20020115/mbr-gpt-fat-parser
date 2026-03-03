using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileManager;

interface IBootable
{

    string OEM { get; set; }

    string Name { get; set; }

    int BytesPerSector { get; set; }

    byte SectorPerClaster { get; set; }

    long SerialNumber { get; set; }

    byte MediaDescriptor { get; set; }
    
  
    //some for fat32 part-------------------------------------
    int SizeOfReserve
    {
        get ;
        set ;
    }

    byte NumOfFatCopy
    {
        get; 
        set;
    }

    int SizeOfFatCopy
    {
        get;
        set;
    }

    int BeginOfRoot
    {
        get;
        set;
    }

    byte BackupBootSector
    {
        get ;
        set ;
    }

    string VolumeNameOfPartition
    {
        get ;
        set ;
    }
    //-------------------------------------------------

}
