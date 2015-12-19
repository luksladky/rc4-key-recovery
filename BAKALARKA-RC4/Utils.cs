using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAKALARKA_RC4
{
    class Utils
    {
        public static int ord(char ch)
        {
            return (int)(Encoding.GetEncoding(1252).GetBytes(ch + "")[0]);
        }

        public static char chr(int i)
        {
            byte[] bytes = new byte[1];
            bytes[0] = (byte)i;
            return Encoding.GetEncoding(1252).GetString(bytes)[0];
        }







        public static int getByteLenghth(long x)
        {
            int lenght = 0;
            while (x > 0)
            {
                lenght++;
                x = x / 256;
            }
            return lenght;
        }

    }
}
