using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAKALARKA_RC4
{
    class Key
    {
        private int[] keyArray;
        private int keyLength;

        public int Length
        {
            get
            {
                return keyLength;
            }

        }

        public int this[int i]
        {
            get { return keyArray[i % keyLength]; }
        }

        public Key(Random rnd, byte length)
        {
            keyLength = length;
            keyArray = new int[length];
            for (int i = 0; i < length; i++)
            {
                keyArray[i] = rnd.Next(256);
            }
        }


        public Key(string key)
        {
            this.keyLength = key.Length;
            this.keyArray = new int[keyLength];
            for (int k = 0; k < keyLength; k++)
            {
                keyArray[k] = Utils.ord(key[k]);
            }
        }

        public Key(long key)
        {
            keyLength = Utils.getByteLenghth(key);
            long mask = 0xFF;
            long tmp;
            int shift;

            for (int k = 0; k < keyLength; k++)
            {
                shift = 8*(k);
                tmp = key & (mask << shift);
                keyArray[k] = (int) (tmp >> shift);

            }
        }

        public Key(int[] key)
        {
            keyLength = key.Length;
            keyArray = new int[keyLength];
            for (int k = 0; k < keyLength; k++)
            {

                keyArray[k] = key[k];

            }

        }

        public override string ToString()
        {

            string s = "";
            for (int i = 0; i < keyLength; i++)
            {
                s = s + keyArray[i].ToString("X2") + ",";
            }

            return s;
        }

        public bool Equals(Key key)
        {
            bool equals = true;
            if (key == null)
                return false;
            if (key.Length != this.Length)
                return false;
            for (int i = 0; i < this.Length; i++)
            {
                if (key[i] != this[i])
                    equals = false;
            }

            return equals;
        }
    }
}
