using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fartooth.Powers;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Fartooth.Powers
{
    public sealed class DistanceToBlockPower : PowerModel
    {
        protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<Distance>()];
        public override PowerType Type => PowerType.Buff;
        public override PowerStackType StackType => PowerStackType.Single;

        public override bool IsInstanced => true;

        public override async Task BeforeTurnEndEarly(PlayerChoiceContext choiceContext, CombatSide side)
        {
            if (side == CombatSide.Player)
            {
                int num = Owner.GetPowerAmount<Distance>();
                await CreatureCmd.GainBlock(base.Owner, num, ValueProp.Unpowered, null);
            }
            
        }
    }
}
