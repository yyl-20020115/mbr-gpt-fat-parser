using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileManager;

interface ISection
{

    ulong getFirstSector();//common

    ulong getSize();//common

    //MBR
    byte getFileSystemType();

    bool isBootable
    {
        get;
    }

    void setFirstSector(uint first_sector);
    
    //-------------------------------MBR
  
    //GPT
    long getAttributes();

    string getPartitionName();//common *in mbr 'type' will be transform in string

    Guid getPartitionTypeGuid();

    Guid getUniquePartitionGuid();

    //-------------------------------GPT
    
}
