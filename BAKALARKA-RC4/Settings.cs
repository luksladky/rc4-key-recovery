using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BAKALARKA_RC4
{
    class Settings
    {
        public const int keyLenght = 8;
        public const int rounds = 100;

        public const int updateGroupLength = 4;

        public const double prob_th = 0.01;
        public const double prob_diff_boost = 1.5;
        public static void setLogging()
        {
            //RC4.logKeyVerification = true;
            //RC4.logAfterKSAPerm = true;
            BuildKeyTable.logBuildKeyTableMinFreq = 3;
            BuildKeyTable.logFrequencyList = false;
            BuildKeyTableImproved.logBuildKeyTableMinFreq = 2;
        }
    }
}
