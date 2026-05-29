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
    public sealed class Focusing : CardModel
    {


        

        protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<Distance>(), base.EnergyHoverTip];
        protected override bool HasEnergyCostX => true;
        protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        
        new PowerVar<Distance>(2m)
    };
        public Focusing()
        : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.Self)
        {

        }


        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {

            int num = ResolveEnergyXValue();
            if (base.IsUpgraded)
            {
                num++;
            }
            if (num > 0) {
                for (int i = 0; i < num; i++) {
                    if (Owner.Creature.Powers.OfType<DistanceAvailablePower>().FirstOrDefault() == null)
                    {
                        await PowerCmd.Apply<Distance>(Owner.Creature, base.DynamicVars[nameof(Distance)].BaseValue, Owner.Creature, null);
                    }
                }
                
            }
            
        }

    }
}
