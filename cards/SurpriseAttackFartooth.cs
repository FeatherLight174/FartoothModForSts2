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

//攻击卡 ： 突袭
namespace FartoothMod.cards
{
    public sealed class SurpriseAttackFartooth : CardModel
    {
        protected override List<DynamicVar> CanonicalVars => [
        new DamageVar(6m, ValueProp.Move) // 伤害值
	];
        public SurpriseAttackFartooth()
        : base(1, CardType.Attack, CardRarity.Common, TargetType.AnyEnemy) { }

        protected override bool IsPlayable
        {
            get
            {
                // 距离判定,找到目标对象Distance
                var power = Owner.Creature.Powers
           .FirstOrDefault(p => p is Distance);
                if (power == null) return false;
                return power.Amount >= 2;
            }
        }


        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {


            await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue)
         .FromCard(this) // 攻击来源
         .Targeting(cardPlay.Target) // 攻击目标
    .WithHitFx("vfx/vfx_attack_slash") // 攻击特效
    .Execute(choiceContext); // 执行攻击效果
        }
    }
}
