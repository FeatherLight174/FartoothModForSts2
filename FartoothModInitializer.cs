using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Fartooth.CardPools;
using Fartooth.Cards;
using Fartooth.PotionPools;
using Fartooth.RelicPools;
using HarmonyLib;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Logging;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Characters;
using MegaCrit.Sts2.Core.Models.RelicPools;
using Fartooth;
using Fartooth.Relics;
using Fartooth.cards;




namespace FartoothMod
{
	[ModInitializer(nameof(Initialize))]
	public static class FartoothModInitializer
	{
		public static void Initialize()
		{
			try
			{
				ModHelper.AddModelToPool(typeof(FartoothRelicPool), typeof(Sniper));
				ModHelper.AddModelToPool(typeof(FartoothCardPool), typeof(PreciseShot));
                ModHelper.AddModelToPool(typeof(FartoothCardPool), typeof(StrikeFartooth));
                ModHelper.AddModelToPool(typeof(FartoothCardPool), typeof(DefendFartooth));
                ModHelper.AddModelToPool(typeof(FartoothCardPool), typeof(MegaShot));
                ModHelper.AddModelToPool(typeof(FartoothCardPool), typeof(SurpriseAttackFartooth));
                ModHelper.AddModelToPool(typeof(FartoothCardPool), typeof(CloseRangeShot));
                ModHelper.AddModelToPool(typeof(FartoothCardPool), typeof(BeingPrepared));
                ModHelper.AddModelToPool(typeof(FartoothCardPool), typeof(SwiftShot));
                ModHelper.AddModelToPool(typeof(FartoothCardPool), typeof(QuickDodge));
                ModHelper.AddModelToPool(typeof(FartoothCardPool), typeof(WeaknessShot));
                ModHelper.AddModelToPool(typeof(FartoothCardPool), typeof(StrategicRetreat));
                ModHelper.AddModelToPool(typeof(FartoothCardPool), typeof(AllianceSupport));
                ModHelper.AddModelToPool(typeof(FartoothCardPool), typeof(AimedShot));
                ModHelper.AddModelToPool(typeof(FartoothCardPool), typeof(AimingAtYou));
                ModHelper.AddModelToPool(typeof(FartoothCardPool), typeof(RollingShot));
                ModHelper.AddModelToPool(typeof(FartoothCardPool), typeof(CombatExperience));
                ModHelper.AddModelToPool(typeof(FartoothCardPool), typeof(TrailMark));
                ModHelper.AddModelToPool(typeof(FartoothCardPool), typeof(Concentrate));
                ModHelper.AddModelToPool(typeof(FartoothCardPool), typeof(HoldBreath));
                ModHelper.AddModelToPool(typeof(FartoothCardPool), typeof(ShootingSkill));
                ModHelper.AddModelToPool(typeof(FartoothCardPool), typeof(RestInPlace));
                ModHelper.AddModelToPool(typeof(FartoothCardPool), typeof(DistanceToBlock));
                ModHelper.AddModelToPool(typeof(FartoothCardPool), typeof(EmergencyEvasion));
                ModHelper.AddModelToPool(typeof(FartoothCardPool), typeof(KnightParty));
                ModHelper.AddModelToPool(typeof(FartoothCardPool), typeof(SeekingSupplies));
                ModHelper.AddModelToPool(typeof(FartoothCardPool), typeof(Focusing));
                var harmony = new Harmony("FeatherLight.FartoothMod");
				harmony.PatchAll();
                Godot.Bridge.ScriptManagerBridge.LookupScriptsInAssembly(Assembly.GetExecutingAssembly());
                // 初始化 harmony 库
            }
			catch (Exception e)
			{
				Log.Error("FartoothMod - 加载失败");
				Log.Error(e.Message);
				return;
			}
			Log.Info("FartoothMod - 加载成功!");
		}
	}
	
	

	


    



}
