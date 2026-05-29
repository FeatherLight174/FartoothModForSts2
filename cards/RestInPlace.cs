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
using MegaCrit.Sts2.Core.Models.Cards;

namespace Fartooth.Cards
{
    public sealed class RestInPlace : CardModel
    {

        protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new EnergyVar(2),
    };
        public RestInPlace()
        : base(1, CardType.Attack, CardRarity.Uncommon, TargetType.Self)
        {
        }
        protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[1]
    {
        base.EnergyHoverTip,
    };
        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            //await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue)
            await PowerCmd.Apply<DistanceAvailablePower>(Owner.Creature, 1m, Owner.Creature, null);
            await PowerCmd.Apply<DistanceConsumePower>(Owner.Creature, 1m, Owner.Creature, null);
            await PlayerCmd.GainEnergy(2m, base.Owner);
        }

        protected override void OnUpgrade()
        {
            base.EnergyCost.UpgradeBy(-1);
        }
    }
}
