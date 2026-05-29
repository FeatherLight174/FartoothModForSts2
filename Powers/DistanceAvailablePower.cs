using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fartooth.Powers;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Fartooth.Powers
{
    public sealed class DistanceAvailablePower : PowerModel
    {
        protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<Distance>()];
        public override PowerType Type => PowerType.Debuff;
        public override PowerStackType StackType => PowerStackType.Single;

        public override bool IsInstanced => false;
        public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
        {
            if (side == base.Owner.Side)
            {
                Flash();
                await PowerCmd.Remove(this);
                
            }
        }
    }
}
