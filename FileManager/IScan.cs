using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileManager;

interface IScan
{
    int Length
    {
        get;
    }

    //Indexer return address of begining for every logical volume
    ISection this[int index]
    {
        get;
    }

    bool Scan(string physicalDriveName);

}
