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
    /// 同盟支援
    /// </summary>
    public sealed class AllianceSupport : CardModel
    {
        protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<Mark>()];
        protected override List<DynamicVar> CanonicalVars => [
           new DamageVar(5m, ValueProp.Move) // 伤害值

   ];
        public AllianceSupport()
        : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.AnyEnemy)
        {
        }
        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            int markAmount = cardPlay.Target.Powers.FirstOrDefault(p => p is Mark).Amount;
            //await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue)
            await PowerCmd.Apply<Mark>(cardPlay.Target, (decimal)-markAmount, cardPlay.Target, null);
            await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue).WithHitCount(markAmount)
             .FromCard(this) // 攻击来源
             .Targeting(cardPlay.Target) // 攻击目标
        .WithHitFx("vfx/vfx_attack_slash") // 攻击特效
        .Execute(choiceContext); // 执行攻击效果
            
        }
        protected override void OnUpgrade()
        {
            base.DynamicVars.Damage.UpgradeValueBy(1m); // 升级后加 1 点伤害
        }
    }
}
