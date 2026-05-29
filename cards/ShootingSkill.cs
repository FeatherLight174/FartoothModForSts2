using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fartooth.Powers;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MegaCrit.Sts2.Core.ValueProps;

namespace Fartooth.Cards
{
    /// <summary>
    /// 攻击：翻滚攻击
    /// </summary>
    public sealed class ShootingSkill : CardModel
    {
        protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<Distance>()];
        public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];
        protected override List<DynamicVar> CanonicalVars => [
           // 伤害值
            new PowerVar<Distance>(1m),
            new RepeatVar(0)
   ];
        public ShootingSkill()
        : base(1, CardType.Skill, CardRarity.Rare, TargetType.Self)
        {
        }
        public override async Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side, CombatState combatState)
        {
            if (side == Owner.Creature.Side && combatState.RoundNumber <= 1)
            {
                await Cmd.CustomScaledWait(0.2f, 0.4f);
                int num = Owner.PlayerCombatState.AllCards.Count((CardModel c) => c.Tags.Contains(CardTag.OstyAttack));
                for (int i = 0; i < num; i++)
                {

                    base.DynamicVars.Repeat.UpgradeValueBy(1);
                }
            }
            
            
        }
        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            await CreatureCmd.TriggerAnim(base.Owner.Creature, "Cast", base.Owner.Character.CastAnimDelay);
            if (Owner.Creature.Powers.OfType<DistanceAvailablePower>().FirstOrDefault() == null)
            {
                int num = Owner.PlayerCombatState.AllCards.Count((CardModel c) => c.Tags.Contains(CardTag.OstyAttack));
                for (int i = 0; i < num; i++)
                {

                    await PowerCmd.Apply<Distance>(Owner.Creature, base.DynamicVars[nameof(Distance)].BaseValue, Owner.Creature, null);
                }
            }
                
        }
        


        protected override void OnUpgrade()
        {
            base.EnergyCost.UpgradeBy(-1);
        }
    }
}
