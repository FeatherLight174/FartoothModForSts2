using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fartooth.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
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
    /// </summary>
    public sealed class KnightParty : CardModel
    {


        public override IEnumerable<CardKeyword> CanonicalKeywords => [CardKeyword.Exhaust];

        protected override IEnumerable<IHoverTip> ExtraHoverTips=> [HoverTipFactory.FromPower<Distance>(), base.EnergyHoverTip];

        protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new EnergyVar(1),
        new CardsVar(2),
        new PowerVar<Distance>(1m)
    };
        public KnightParty()
        : base(0, CardType.Skill, CardRarity.Rare, TargetType.Self)
        {

        }


        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {

            if (LocalContext.IsMe(base.Owner))
            {
                VfxCmd.PlayFullScreenInCombat("vfx/vfx_adrenaline");
            }
            await PlayerCmd.GainEnergy(base.DynamicVars.Energy.IntValue, base.Owner);
            await CardPileCmd.Draw(choiceContext, base.DynamicVars.Cards.BaseValue, base.Owner);
            if (Owner.Creature.Powers.OfType<DistanceAvailablePower>().FirstOrDefault() == null)
            {
                await PowerCmd.Apply<Distance>(Owner.Creature, base.DynamicVars[nameof(Distance)].BaseValue, Owner.Creature, null);
            }
        }
        protected override void OnUpgrade()
        {
            base.DynamicVars.Energy.UpgradeValueBy(1m); // 升级后加 一层力量
        }
    }
}
