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


        }
    }
}