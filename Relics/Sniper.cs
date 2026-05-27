using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fartooth.Powers;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Events;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Rooms;
namespace Fartooth.Relics;

public sealed class Sniper : RelicModel
{
    public override RelicRarity Rarity => RelicRarity.Starter;
    // 稀有度


    public override async Task AfterSideTurnStart(CombatSide side, CombatState combatState)
    {
        // 判断事件调用时是否为遗物持有者一方
        if (side == Owner.Creature.Side)
        {
            await Cmd.CustomScaledWait(0.2f, 0.4f);
            Flash(); // 触发遗物图标闪烁
           
            await PowerCmd.Apply<Distance>(Owner.Creature, 1m, Owner.Creature, null);


        }
    }
    


}


