using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace BAKALARKA_RC4
{
    public interface IAttack
    {
        bool GuessKey();
    }

    class Attacks : BaseClass
    {
        protected RC4 cipher;
        protected int[] InvS;

        protected int l;

        public Attacks(RC4 cipher)
        {
            this.cipher = cipher;
            l = cipher.K.Length;
            S = cipher.savedS;
        }

        protected void ConstructInvS()
        {
            InvS = new int[N];
            for (int k = 0; k < N; k++)
            {
                InvS[S[k]] = k;
            }
        }
    }
}
