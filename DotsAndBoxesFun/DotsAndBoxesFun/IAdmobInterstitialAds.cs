using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DotsAndBoxesFun
{
    public interface IAdmobInterstitialAds
    {
        Task Display(string adId);
    }
}
