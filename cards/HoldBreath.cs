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
    /// 技能：屏息
    /// </summary>
    public sealed class HoldBreath: CardModel
    {
        private const string _strengthLossKey = "StrengthLoss";

        protected override IEnumerable<DynamicVar> CanonicalVars
    => [new DynamicVar("StrengthLoss", 2m)];
        public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

        protected override IEnumerable<IHoverTip> ExtraHoverTips
            => [HoverTipFactory.FromPower<StrengthPower>()];
        
        public HoldBreath()
        : base(0, CardType.Skill, CardRarity.Common, TargetType.AllEnemies)
        {
            
        }


        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            int distance = Owner.Creature.GetPowerAmount<Distance>();
            int tempDistance = Owner.Creature.GetPowerAmount<TemporaryDistance>();
            int distanceTotal = distance + tempDistance;
            await CreatureCmd.TriggerAnim(base.Owner.Creature, "Cast", base.Owner.Character.CastAnimDelay);
            for(int i = 0; i< distanceTotal; i++)
            {
                await PowerCmd.Apply<HoldBreathPower>(CombatState.Enemies, base.DynamicVars["StrengthLoss"].BaseValue, base.Owner.Creature, this);
            }
        }
        protected override void OnUpgrade()
        {
            base.DynamicVars["StrengthLoss"].UpgradeValueBy(1m); // 升级后加 一层力量
        }
    }
}
