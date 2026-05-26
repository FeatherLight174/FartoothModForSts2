using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using HarmonyLib;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Unlocks;

namespace Fartooth.PotionPools;

public sealed class FartoothPotionPool : PotionPoolModel
{
	public override string EnergyColorName => "fartooth";

	public override Color LabOutlineColor => StsColors.orange;

	protected override IEnumerable<PotionModel> GenerateAllPotions()
	{
		return Array.Empty<PotionModel>();
	}

	public override IEnumerable<PotionModel> GetUnlockedPotions(UnlockState unlockState)
	{
		return Array.Empty<PotionModel>();
	}

    
}
