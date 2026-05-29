using Fartooth.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fartooth.Cards
{
    /// <summary>
    /// 攻击：翻滚攻击
    /// </summary>
    public sealed class RollingShot : CardModel
    {
        protected override List<DynamicVar> CanonicalVars => [
           new DamageVar(4m, ValueProp.Move) ,// 伤害值
            new PowerVar<Distance>(1m)

   ];
        public RollingShot()
        : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy)
        {
        }

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            //await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue)
            await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue)
             .FromCard(this) // 攻击来源
             .Targeting(cardPlay.Target) // 攻击目标
        .WithHitFx("vfx/vfx_attack_slash") // 攻击特效
        .Execute(choiceContext); // 执行攻击效果
            await PowerCmd.Apply<Distance>(Owner.Creature, base.DynamicVars[nameof(Distance)].BaseValue, Owner.Creature, null);//增加距离
        }

        protected override void OnUpgrade()
        {
            base.DynamicVars[nameof(Distance)].UpgradeValueBy(1m);
        }
    }
}
