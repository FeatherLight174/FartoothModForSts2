using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace Fartooth.Relics;

public sealed class Sniper : RelicModel
{
    public override RelicRarity Rarity => RelicRarity.Starter;
    // 稀有度

    protected override IEnumerable<DynamicVar> CanonicalVars => [
        new EnergyVar(2) // 关联 能量 的动态变量
        ];
    // 动态变量

    public override async Task AfterSideTurnStart(CombatSide side, CombatState combatState)
    {
        // 判断事件调用时是否为遗物持有者一方，且回合数是否为 1
        if (side == Owner.Creature.Side && combatState.RoundNumber == 1)
        {
            Flash(); // 触发遗物图标闪烁
            await PlayerCmd.GainEnergy(DynamicVars.Energy.BaseValue, Owner);
            // 给予玩家能量
        }
    }
}

    
