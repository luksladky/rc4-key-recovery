using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAKALARKA_RC4
{
    class Program
    {
        static void Main(string[] args)
        {



            while (true)
            {
                //Key key = new Key(new int[] {106,59,220,65,34});
                //Key key = new Key("ahoj");
                Key key = new Key("aaaaaa");
                RC4 cipher = new RC4(key);
                cipher.logAfterKSAPerm = true;
                //cipher.encrypt("Plaintext");
                cipher.BuildKeyTable();
                //cipher.LinearEquationsTest();
            }



           /* cipher.encrypt("Plaintext");
            cipher.getStateAfterKSA();*/


        }
    }
}
