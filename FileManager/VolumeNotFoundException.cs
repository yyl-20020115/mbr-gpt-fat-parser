using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileManager;

class VolumeNotFoundException : ApplicationException
{
    public VolumeNotFoundException() { }

    public VolumeNotFoundException(string massage) : base(massage) { }

    public VolumeNotFoundException(string massage, Exception inner) : base(massage, inner) { }

    
}
