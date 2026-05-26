using System.Collections.Generic;
using System.Linq;
using Fartooth.Relics;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;

namespace Fartooth.RelicPools;

public sealed class FartoothRelicPool : RelicPoolModel
{
    public override string EnergyColorName => "fartooth";

    public override Color LabOutlineColor => StsColors.gold;

    protected override IEnumerable<RelicModel> GenerateAllRelics()
    {
        return new RelicModel[]
        {
            ModelDb.Relic<Sniper>(),
        };
    }

    
}