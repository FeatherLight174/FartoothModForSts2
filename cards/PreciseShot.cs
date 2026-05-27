using System;
using System.Collections.Generic;
using System.Linq;
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
public sealed class PreciseShot : CardModel
{

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[3]
    {
       new CalculationBaseVar(6m),          // 基础伤害 6
        new ExtraDamageVar(1m),              // 每层Distance的倍率：1 👈 独立EXTRA
        new CalculatedDamageVar(ValueProp.Move).WithMultiplier((CardModel _, Creature? target) => _.Owner?.Creature.GetPowerAmount<Distance>() ?? 0 )
            
    };
    //动态变量


    public PreciseShot()
		: base(1, CardType.Attack, CardRarity.Basic, TargetType.AnyEnemy) { }
	// 卡牌的构造函数，指定卡牌的相关属性

	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
        int distanceAmount = Owner.Creature.GetPowerAmount<Distance>();
        //await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue)
        await DamageCmd.Attack(base.DynamicVars.CalculatedDamage)
         .FromCard(this) // 攻击来源
		 .Targeting(cardPlay.Target) // 攻击目标
	.WithHitFx("vfx/vfx_attack_slash") // 攻击特效
	.Execute(choiceContext); // 执行攻击效果
	}

	protected override void OnUpgrade()
	{
		base.DynamicVars.ExtraDamage.UpgradeValueBy(1m); // 升级后加 1 点伤害
	}
}
