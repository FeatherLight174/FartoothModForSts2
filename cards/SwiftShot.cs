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
using MegaCrit.Sts2.Core.ValueProps;

namespace Fartooth.Cards
{
    /// <summary>
    /// 攻击：迅捷射击 距离每增加1，对全体敌人造成2/3伤害
    /// </summary>
    /// 
    public sealed class SwiftShot : CardModel
    {
        protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<Distance>()];
        protected override HashSet<CardTag> CanonicalTags => new HashSet<CardTag> { CardTag.OstyAttack };
        protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[3]
    {
       new CalculationBaseVar(0m),          // 基础伤害 6
        new ExtraDamageVar(3m),              // 每层Distance的倍率：1 👈 独立EXTRA
        new CalculatedDamageVar(ValueProp.Move).WithMultiplier((CardModel card,Creature? target) =>
            {
                int distance = card.Owner.Creature.GetPowerAmount<Distance>();
                int tempDistance = card.Owner.Creature.GetPowerAmount<TemporaryDistance>();
                return distance + tempDistance;
            })

    };

        //构造函数
        public SwiftShot()
        : base(1, CardType.Attack, CardRarity.Common, TargetType.AllEnemies) { }

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            //await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue)
            await DamageCmd.Attack(base.DynamicVars.CalculatedDamage)
             .FromCard(this) // 攻击来源
              .TargetingAllOpponents(base.CombatState) // 攻击目标
        .WithHitFx("vfx/vfx_attack_slash") // 攻击特效
        .Execute(choiceContext); // 执行攻击效果
        }

        //升级
        protected override void OnUpgrade()
        {
            base.DynamicVars.ExtraDamage.UpgradeValueBy(1m); // 升级后加 3点伤害
        }

    }
}