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
using MegaCrit.Sts2.Core.Models.Powers;

namespace Fartooth.Cards;

public sealed class BeingPrepared : CardModel
{
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[2]
    {
        new EnergyVar(1),
        new PowerVar<Distance>(2m)
    };

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[1]
    {
        base.EnergyHoverTip,
    };

    public BeingPrepared()
        : base(0, CardType.Skill, CardRarity.Basic, TargetType.Self)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        
        await CreatureCmd.TriggerAnim(base.Owner.Creature, "Cast", base.Owner.Character.CastAnimDelay);
        await PowerCmd.Apply<EnergyNextTurnPower>(base.Owner.Creature, base.DynamicVars.Energy.BaseValue, base.Owner.Creature, this);
        if (Owner.Creature.Powers.OfType<DistanceAvailablePower>().FirstOrDefault()==null) {
            await PowerCmd.Apply<Distance>(Owner.Creature, base.DynamicVars[nameof(Distance)].BaseValue, Owner.Creature, null);
        }
        
        
        
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars[nameof(Distance)].UpgradeValueBy(1m);
        //base.DynamicVars[nameof(di)].UpgradeBy(-1);
    }
}
