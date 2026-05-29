using System.Collections.Generic;
using System.Threading.Tasks;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.Combat;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using MegaCrit.Sts2.Core.ValueProps;



namespace Fartooth.Powers;
public sealed class CombatExperiencePower : PowerModel
{
    // 效果类型
    public override PowerType Type => PowerType.Buff;

    // 效果堆叠类型
    public override PowerStackType StackType => PowerStackType.Counter;

    // 叠加的行为
    public override bool IsInstanced => false;

    // 允许层数为负数
    public override bool AllowNegative => false;
    // 👇 正确重写父类的 Amount（包含 get + set）
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<Distance>()];
    public override async Task AfterSideTurnStart(CombatSide side, CombatState combatState)
    {
        if (side == base.Owner.Side)
        {
            Flash();
            await PowerCmd.Apply<Distance>(base.Owner, base.Amount, base.Owner, null);
        }
    }

}


