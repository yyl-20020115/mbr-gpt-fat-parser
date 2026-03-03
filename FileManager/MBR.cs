using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileManager;

class MBR
{
    private byte[] bootstrap;
    private MBRSection[] PT;
    private byte[] signature;

    public int Length
    {
        get { return PT.Length; }
    }

    public MBRSection this[int index]
    {
        get { return PT[index]; }
        set { PT[index] = value; }
    }

    public MBR()
    {
       
        bootstrap = new byte [446];
        PT = new MBRSection[4];
        signature = new byte[2];
    }

    public MBR( byte[] data )
    {
        bootstrap = new byte[446];
        PT = new MBRSection[4];
        signature = new byte[2];

        //------------fill bootstrap---------------
        for (int i = 0; i < bootstrap.Length; i++)
        {
            bootstrap[i] = data[i];
        }
        //------------------------------------------

        //---------fill Partition table-------------
        int offset = 0x1BE;
        for (int i = 0; i < 4; i++)
        {
            PT[i] = new MBRSection(data, offset);
            offset += 0x10;
        }

        signature[0] = data[510];
        signature[1] = data[511];
        //------------------------------------------
    }// ~MBR(byte[] data)

    public void set( byte[] data )
    {
        //------------fill bootstrap---------------
        for (int i = 0; i < bootstrap.Length; i++)
        {
            bootstrap[i] = data[i];
        }
        //------------------------------------------

        //---------fill Partition table-------------
        int offset = 0x1BE;
        for (int i = 0; i < 4; i++)
        {
            PT[i] = new MBRSection(data, offset);
            offset += 0x10;
        }
        //------------------------------------------
    }// ~set()

    public bool isProtective()
    {
        return  PT[0].Type == 0xEE ? true : false;
    }

    public override string ToString()
    {
        string PTInfo;
        PTInfo = "1: " + PT[0].ToString() + "\n";
        PTInfo += "2: " + PT[1].ToString() + "\n";
        PTInfo += "3: " + PT[2].ToString() + "\n";
        PTInfo += "4: " + PT[3].ToString() + "\n";
        PTInfo += "   Signture: " + signature[0].ToString("X") + signature[1].ToString("X");
        return PTInfo;
    }


}//~class
