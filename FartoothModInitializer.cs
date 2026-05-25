using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

namespace FartoothMod
{
	[ModInitializer(nameof(Initialize))]
	public static class FartoothModInitializer
	{
		public static void Initialize()
		{
			try
			{
				ModHelper.AddModelToPool(typeof(FartoothRelicPool), typeof(FartoothRelic));
				ModHelper.AddModelToPool(typeof(FartoothCardPool), typeof(PreciseShot));
				var harmony = new Harmony("FeatherLight.FartoothMod");
				harmony.PatchAll();
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
	
	
	public class FartoothRelic : RelicModel
	{
		public override RelicRarity Rarity => RelicRarity.Starter;
		// 稀有度

		protected override IEnumerable<DynamicVar> CanonicalVars => [
			new EnergyVar(2) // 关联 能量 的动态变量
			];
		// 动态变量

		public override async Task AfterSideTurnStart(CombatSide side, CombatState combatState)
		{
			// 判断事件调用时是否为遗物持有者一方，且回合数是否为 1
			if (side == Owner.Creature.Side && combatState.RoundNumber == 1)
			{
				Flash(); // 触发遗物图标闪烁
				await PlayerCmd.GainEnergy(DynamicVars.Energy.BaseValue, Owner);
				// 给予玩家能量
			}
		}
	}

	

	[HarmonyPatch(typeof(Ironclad), nameof(Ironclad.StartingRelics), MethodType.Getter)]
	public static class IroncladStartingRelicsPatch
	{
		static void Postfix(ref IReadOnlyList<RelicModel> __result)
		{
			var customRelic = ModelDb.Relic<FartoothRelic>();
			// 从注册的数据库中获取我们自定义的遗物实例对象

			if (__result.Any(r => r.Id == customRelic.Id))
				return;
			// 遍历原本的初始遗物列表，如果已经存在这个遗物，就直接返回

			var list = __result.ToList();
			list.Add(customRelic);
			__result = list;
			// 向 __result 追加 customRelic自定义遗物对象
		}
	}

	


}
