using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using System.Collections.Generic;
using System.Linq;

// 你的自定义命名空间
namespace Honkai_Star_Rail;

[HarmonyPatch(typeof(ModelDb), nameof(ModelDb.AllCardPools), MethodType.Getter)]
public static class ModelDbAllCardPoolsPatch
{
    static void Postfix(ref IEnumerable<CardPoolModel> __result)
    {
        __result = __result
            .Append(ModelDb.CardPool<Hua_Huo_Card_Pool>())
            .Distinct();
    }
}
