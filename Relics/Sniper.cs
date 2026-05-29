using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fartooth.Powers;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.Entities.Relics;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Events;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Rooms;
namespace Fartooth.Relics;
//初始遗物：狙击镜，每回合开始时距离+1
public sealed class Sniper : RelicModel
{
    public override RelicRarity Rarity => RelicRarity.Starter;
    // 稀有度


    public override async Task BeforeSideTurnStart(PlayerChoiceContext choiceContext, CombatSide side, CombatState combatState)
    {
        // 判断事件调用时是否为遗物持有者一方
        if (side == Owner.Creature.Side && combatState.RoundNumber <= 1)
        {
            await Cmd.CustomScaledWait(0.2f, 0.4f);
            Flash(); // 触发遗物图标闪烁
           
            //await PowerCmd.Apply<Distance>(Owner.Creature, 2m, Owner.Creature, null);
            await PowerCmd.Apply<Mark>(combatState.Enemies, 1m, Owner.Creature, null);
            await PowerCmd.Apply<PerfectTurnPower>(Owner.Creature, 1m, Owner.Creature, null);

        }
    }
    public bool tookDamageLastTurn { get; private set; }
    public bool DidTakeDamageLastTurn => tookDamageLastTurn;

    //是否受伤
    private bool tookDamageThisTurn = false;

    // 回合开始
    public override Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        tookDamageLastTurn = tookDamageThisTurn;
        tookDamageThisTurn = false;
        if(tookDamageLastTurn == false)
        {
            PowerCmd.Apply<PerfectTurnPower>(player.Creature, 1m, base.Owner.Creature, null);
        }
        return Task.CompletedTask;
    }

    public override Task AfterCurrentHpChanged(Creature creature, decimal delta)
    {
        //玩家受伤将本回合受伤为true
        if (creature.Side == CombatSide.Player)
        {
            if (delta < 0)
            {
                tookDamageThisTurn = true;
            }
        }
        return Task.CompletedTask;
    }


}


