using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Fartooth.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers; 
using MegaCrit.Sts2.Core.ValueProps;

namespace Fartooth.Cards;
public sealed class MegaShot : CardModel
{
	protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<Distance>()];
	protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[3]
	{
	   new CalculationBaseVar(0m),          // 基础伤害 6
		new ExtraDamageVar(5m),              // 每层Distance的倍率：1 👈 独立EXTRA
		new CalculatedDamageVar(ValueProp.Move).WithMultiplier((CardModel card,Creature? target) =>
			{
				int distance = card.Owner.Creature.GetPowerAmount<Distance>();
				int tempDistance = card.Owner.Creature.GetPowerAmount<TemporaryDistance>();
				return distance + tempDistance;
			})

	};
	// 动态变量

	public MegaShot()
		: base(3, CardType.Attack, CardRarity.Rare, TargetType.AnyEnemy) { }
	// 卡牌的构造函数，指定卡牌的相关属性

	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
		await DamageCmd.Attack(base.DynamicVars.CalculatedDamage)
		 .FromCard(this) // 攻击来源
		 .Targeting(cardPlay.Target) // 攻击目标
	.WithHitFx("vfx/vfx_attack_slash") // 攻击特效
	.Execute(choiceContext); // 执行攻击效果
	}

	protected override void OnUpgrade()
	{
		base.DynamicVars.ExtraDamage.UpgradeValueBy(2m); // 升级后加 2 点伤害
	}


}
