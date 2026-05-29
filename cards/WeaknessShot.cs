using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fartooth.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;


namespace Fartooth.Cards
{
    /// <summary>
    /// 攻击：弱点攻击
    /// </summary>
    public sealed class WeaknessShot :  CardModel
    {
        protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<Distance>()];
        protected override HashSet<CardTag> CanonicalTags => new HashSet<CardTag> { CardTag.OstyAttack };
        protected override List<DynamicVar> CanonicalVars => [
            new CalculationBaseVar(0m),          // 基础伤害 6
            new ExtraDamageVar(2m),              // 每层Distance的倍率：1 👈 独立EXTRA
            new CalculatedDamageVar(ValueProp.Move)
            .WithMultiplier((CardModel card,Creature? target) =>
            {
                int distance = card.Owner.Creature.GetPowerAmount<Distance>();
                int tempDistance = card.Owner.Creature.GetPowerAmount<TemporaryDistance>();
                return distance + tempDistance;
            }),
            //new CalculatedDamageVar(ValueProp.Move).WithMultiplier((CardModel _, Creature? target) => (_.Owner?.Creature.GetPowerAmount<Distance>() ?? 0 + _.Owner?.Creature.GetPowerAmount<TemporaryDistance>() ?? 0)),
            new PowerVar<VulnerablePower>(1m),
            new PowerVar<WeakPower>(1m)
    ];
        public WeaknessShot()
        : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
        {
        }

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {

            await DamageCmd.Attack(base.DynamicVars.CalculatedDamage)
          .FromCard(this) // 攻击来源
          .Targeting(cardPlay.Target) // 攻击目标
          .WithHitFx("vfx/vfx_attack_slash") // 攻击特效
          .Execute(choiceContext); // 执行攻击效果
            await PowerCmd.Apply<WeakPower>(cardPlay.Target, base.DynamicVars.Weak.BaseValue, base.Owner.Creature, this);
            await PowerCmd.Apply<VulnerablePower>(cardPlay.Target, base.DynamicVars.Vulnerable.BaseValue, base.Owner.Creature, this);
        }

        protected override void OnUpgrade()
        {
            base.DynamicVars.Vulnerable.UpgradeValueBy(1m); // 升级后加 一层易伤
            base.DynamicVars.Weak.UpgradeValueBy(1m); // 升级加一层虚弱
        }

    }
}
