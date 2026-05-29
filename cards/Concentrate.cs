using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fartooth.Powers;
using MegaCrit.Sts2.Core.Combat;
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
    /// 技能：凝神
    /// </summary>
    public sealed class Concentrate: CardModel
    {
        private const string _strengthLossKey = "StrengthLoss";

        protected override IEnumerable<DynamicVar> CanonicalVars
    => [new DynamicVar("StrengthLoss", 3m)];



        protected override IEnumerable<IHoverTip> ExtraHoverTips
            => [HoverTipFactory.FromPower<StrengthPower>()];

        public Concentrate()
            : base(1, CardType.Skill, CardRarity.Common, TargetType.Self)
        {
        }

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            await CreatureCmd.TriggerAnim(base.Owner.Creature, "Cast", base.Owner.Character.CastAnimDelay);

            await PowerCmd.Apply<ConcentratePower>(base.Owner.Creature, base.DynamicVars["StrengthLoss"].BaseValue, base.Owner.Creature, this);
            var turnPower = Owner.Creature.Powers.OfType<PerfectTurnPower>().FirstOrDefault();
            if (turnPower != null)
            {
                if (!turnPower.DidTakeDamageLastTurn)
                {
                    await PowerCmd.Apply<ConcentratePower>(base.Owner.Creature, base.DynamicVars["StrengthLoss"].BaseValue, base.Owner.Creature, this);
                }
            }
        }
        protected override void OnUpgrade()
        {
            base.EnergyCost.UpgradeBy(-1); // 升级后加 一层力量
        }
        protected override bool ShouldGlowGoldInternal => (Owner.Creature.Powers.OfType<PerfectTurnPower>().FirstOrDefault() != null) == true;

        
    }
}
