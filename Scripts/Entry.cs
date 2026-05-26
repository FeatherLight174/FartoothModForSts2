using System.Collections.Generic;
using System.Linq;
using Fartooth.CardPools;
using Fartooth.PotionPools;
using Fartooth.RelicPools;
using HarmonyLib;
using MegaCrit.Sts2.Core.Models;
using Fartooth.Characters;

[HarmonyPatch(typeof(ModelDb), nameof(ModelDb.AllCardPools), MethodType.Getter)]
public static class ModelDbAllCardPoolsPatch
{
    static void Postfix(ref IEnumerable<CardPoolModel> __result)
    {
        __result = __result
            .Append(ModelDb.CardPool<FartoothCardPool>())
            .Distinct();
    }
}

[HarmonyPatch(typeof(ModelDb), nameof(ModelDb.AllRelicPools), MethodType.Getter)]
public static class ModelDbAllRelicPoolsPatch
{
    static void Postfix(ref IEnumerable<RelicPoolModel> __result)
    {
        __result = __result
            .Append(ModelDb.RelicPool<FartoothRelicPool>())
            .Distinct();
    }
}

[HarmonyPatch(typeof(ModelDb), nameof(ModelDb.AllPotionPools), MethodType.Getter)]
public static class ModelDbAllPotionPoolsPatch
{
    static void Postfix(ref IEnumerable<PotionPoolModel> __result)
    {
        __result = __result
            .Append(ModelDb.PotionPool<FartoothPotionPool>())
            .Distinct();
    }
}

