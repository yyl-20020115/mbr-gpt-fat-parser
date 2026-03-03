using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32.SafeHandles;
using System.IO;


namespace FileManager;

//Описание файловой системы
class Fat32
{
    private UInt64 ADDRESS;//отностельный адресс начала раздела

    private UInt64 ADDRESS_ROOT;//адрес начала рут каталога
    private int ROOT_SIZE;//размер рута
    private FileDescriptor[] ROOT_FILES;//массив с элементами в рут каталоге

    private BootFat32 BPB;//Bios parametr block aka boot sector
    private int[] FAT_TABLE;//массив таблицы фат, i-й элемент соответсвует i-тому кластеру

    private List<FileDescriptor> LIST_OF_ROOT_FILES;
    private List<FileDescriptor> ListLFN;


    private void updateFatTable(SafeFileHandle shFile)
    {
        UInt64 begFAT;
        //вделяем буфер кратный размеру сектора
        int BuffSize = BPB.SizeOfFatCopy * BPB.BytesPerSector;
        Byte[] buff = new Byte[BuffSize];
        UInt32 dwBytesRead;
        int nFatElement;

        //-----------------------------------------------------------------------------------

        //address of Fat table
        begFAT = ADDRESS + (ulong)BPB.SizeOfReserve * (ulong)BPB.BytesPerSector;

        int hPart = (int)(begFAT >> 32);
        uint lPart = (uint)begFAT;
        //-----------------------------------------------------------------------------------

        //--смещаемся и читаем фат таблицу в память---
        WinApi.SetFilePointer(shFile, lPart, out hPart, (uint)SeekOrigin.Begin);
        if (shFile.IsInvalid)
        {
            throw new InvalidDataException();
        }

        try
        {
            WinApi.ReadFile(shFile, buff, (uint)BuffSize, out dwBytesRead, (IntPtr)0);
        }
        catch (FileLoadException ex)
        {
            ex.ToString();//!!!!!!!!!!!!!!!!! shit
        }


        //-----------------------------------------------------------------------------------

        //заполняем таблицу фат

        nFatElement = (BPB.SizeOfFatCopy * BPB.BytesPerSector) / 4;
        FAT_TABLE = new int[nFatElement];
        for (int i = 0, j = 0; i < nFatElement; i++, j += 4)
        {
            FAT_TABLE[i] = BitConverter.ToInt32(buff, j);
        }

        //------------------------------------------------------------------------------------
    }


    public Fat32(string physicalDriveName, ulong relativeAddress, BootFat32 BPB)
    {

        //---смещаемся на логический диск который выбрали---
        //-----------чтение boot-сектора--------------------
        //HANDLE hFile = CreateFile(hardDrive, GENERIC_READ, FILE_SHARE_READ, NULL, OPEN_EXISTING, FILE_ATTRIBUTE_READONLY, NULL);

        //addr.QuadPart = ((unsigned __int64)relativeAddress) * 512;
        //SetFilePointer(hFile, addr.LowPart, &addr.HighPart, FILE_BEGIN);
        //checkError();

        //BYTE *bootSector = readSector(hFile);
        //checkError();
        ////----------проверка сигнатуры NTFS-------------------------------------
        //char *oem = (char*)&bootSector[0x3];
        //if (oem[0] == 'N' && oem[1] == 'T' && oem[2] == 'F' && oem[3] == 'S')
        //{
        //    cout << "Current logical disk have NTFS!!!\n";
        //    system("pause");
        //    return;
        //}

        ////Заполняем поля структуры
        //BPB = fillBootSector(bootSector);
        ////----------------------------------------------------------------------

        ////определение типа фат
        //type = getTypeOfFAT(BPB);
        ////--------------------------------------------------------------------------

        SafeFileHandle shFile = WinApi.CreateFile(physicalDriveName, FileAccess.Read, FileShare.Read, (IntPtr)0, FileMode.Open, 0, (IntPtr)0);
        if (shFile.IsInvalid)
            throw new IOException();

        //загружаем фат таблицу в память
        updateFatTable(shFile);
        //--------------------------------------------------------------------------

        ////-------------Рут каталог------------------------------------------------
        //ADDRESS_ROOT = relativeAddress + BPB->SizeOfReserve + (BPB.NumOfFATcpy * BPB.sizeOfFATcpy) + ((BPB.begRoot - 2) * BPB.SecPerCluster)) *BPB.BytsPerSector;
        ////-------------------------------------------------------------

        //updateRootSize(shFile);
        //updateRootElem(hFile);
        //CloseHandle(hFile);
    }
}
