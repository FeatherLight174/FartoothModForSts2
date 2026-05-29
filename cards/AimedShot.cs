using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fartooth.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Fartooth.Cards
{
    /// <summary>
    /// 攻击：定点射击
    /// </summary>
    public sealed class AimedShot : CardModel
    {
        protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<Mark>()];
        protected override HashSet<CardTag> CanonicalTags => new HashSet<CardTag> { CardTag.OstyAttack };
        protected override List<DynamicVar> CanonicalVars => [
           new DamageVar(6m, ValueProp.Move) // 伤害值

   ];
        public AimedShot()
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
            var mark = cardPlay.Target.Powers.FirstOrDefault(p => p is Mark);
            if (mark != null)
            {
                if (mark.Amount > 0)
                {
                    await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue)
                     .FromCard(this) // 攻击来源
                     .Targeting(cardPlay.Target) // 攻击目标
                    .WithHitFx("vfx/vfx_attack_slash") // 攻击特效
                    .Execute(choiceContext); // 执行攻击效果
                }
            }
    }
        protected override void OnUpgrade()
        {
            base.DynamicVars.Damage.UpgradeValueBy(2m); // 升级后加 1 点伤害
        }
        protected override bool ShouldGlowGoldInternal =>base.CombatState?.Enemies?.Any(e =>!e.IsDead &&e.Powers.Any(p => p is Mark mark && mark.Amount > 0)) == true;


    }
}
