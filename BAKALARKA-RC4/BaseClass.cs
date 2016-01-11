using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAKALARKA_RC4
{
    class BaseClass
    {
        protected int[] S;
        protected int N;


        public BaseClass()
        {
            N = Constants.N;
            S = new int[N];
        }

        protected int mod(int x, int m)
        {
            return (x % m + m) % m;
        }

        protected void Swap(int i, int j)
        {

            int tmp = S[i];
            S[i] = S[j];
            S[j] = tmp;
        }
    }
}
