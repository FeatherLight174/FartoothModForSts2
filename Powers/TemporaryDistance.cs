using System.Threading.Tasks;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;

namespace Fartooth.Powers
{
    // 临时距离：本回合获得 X 点距离，回合结束移除
    public sealed class TemporaryDistance : PowerModel
    {
        public override PowerType Type => PowerType.Buff;
        public override PowerStackType StackType => PowerStackType.Counter;

        // 关键：回合结束时，移除这个能力
        public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
        {
            // 只在【自己的回合结束】时消失
            if (side == Owner.Side)
            {
                await PowerCmd.Remove(this);
            }
        }
    }
}