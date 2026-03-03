using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileManager;

class FileDescriptor
{
	    private string name;
	    private Byte attribute;
	    private uint address;
	    private uint size;

    public string Name
    {
        set { name = value; }
        get { return name; }
    }

    public Byte Attribute
    {
        set { attribute = value; }
        get { return attribute; }
    }

    public uint Address
    {
        set { Address = value; }
        get { return address; }
    }

    public uint Size
    {
        set { size = value; }
        get { return size; }
    }

}
