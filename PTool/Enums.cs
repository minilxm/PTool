using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PTool
{
    public enum ProductID : byte
    {
        GrasebyC6   = 0x04,
        GrasebyF6   = 0x05,
        GrasebyC6T  = 0x09,
        Graseby2000 = 0x0A,
        Graseby2100 = 0x0B,
        WZ50C6      = 0x04^0xFF,
        WZS50F6     = 0x05 ^ 0xFF,
        WZ50C6T     = 0x09 ^ 0xFF,
    }

//    C6 读取当前压力传感器命令
//55 aa 05 04 01 70 00 86

//F6 读取当前压力传感器命令
//55 aa 05 05 01 70 00 85

//C6T 读取当前压力传感器命令
//55 aa 05 09 01 70 00 81

//2000 读取当前压力传感器命令
//55 aa 05 0A 01 70 00 80

//2100 读取当前压力传感器命令
//55 aa 05 0B 01 70 00 7F


    //返回11个字节，取ID为7，8，9三个字节
}
