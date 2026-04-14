using Honkai_Star_Rail;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Cards;

namespace Honkai_Star_Rail
{
    [ModInitializer(nameof(Initialize))]
    public static class MyCustomModInitializer
    {
        public static void Initialize()
        {
            Log.Info("MyCustomMod - 加载成功!");
            ModHelper.AddModelToPool(typeof(ColorlessCardPool), typeof(Make_Fun_Of_You));
            ModHelper.AddModelToPool(typeof(ColorlessCardPool), typeof(Outercept));
            ModHelper.AddModelToPool(typeof(ColorlessCardPool), typeof(Might_Thief));
            ModHelper.AddModelToPool(typeof(ColorlessCardPool), typeof(Malicious_Gift));
            ModHelper.AddModelToPool(typeof(ColorlessCardPool), typeof(FireflysTrade));
            ModHelper.AddModelToPool(typeof(ColorlessCardPool), typeof(Final_Ripple));
            ModHelper.AddModelToPool(typeof(ColorlessCardPool),typeof(Revive));
            ModHelper.AddModelToPool(typeof(ColorlessCardPool), typeof(GoldTransfer));


        }
    }
}