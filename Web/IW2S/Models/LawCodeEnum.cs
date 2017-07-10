using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IW2S.Models
{
    public enum LawCodeEnum
    {
        /// <summary>
        /// 擅自使用知名商品特有的名称、包装、装潢行为
        /// </summary>
        UseFamous =1,
        /// <summary>
        /// 商业贿赂行为
        /// </summary>
        BusinessBribery=2,
        /// <summary>
        /// 虚假宣传行为
        /// </summary>
        FakeClaim=3,
        /// <summary>
        /// 侵犯商业秘密行为
        /// </summary>
        BusinessSecret=4,
        /// <summary>
        /// 不正当有奖销售行为
        /// </summary>
        PrizeSale=5,
        /// <summary>
        /// 公用企业或独占经营者强制交易行为
        /// </summary>
        ForcedTrade=6,
        /// <summary>
        /// 滥用行政权力限制竞争行为
        /// </summary>
        LegalRestrict=7,
        /// <summary>
        /// 串通招投标行为
        /// </summary>
        FakeTender=8
    }


}