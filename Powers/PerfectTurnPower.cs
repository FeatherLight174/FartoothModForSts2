using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;


namespace Fartooth.Powers
{
    public sealed class PerfectTurnPower : PowerModel
    {
        public override PowerType Type => PowerType.Buff;

        public override bool IsInstanced => false;

        public bool tookDamageLastTurn { get; private set; }
        public bool DidTakeDamageLastTurn => tookDamageLastTurn;

        public override PowerStackType StackType => PowerStackType.Single;
        //是否受伤
        private bool tookDamageThisTurn = false;

        // 回合开始
        public override Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
        {
            tookDamageLastTurn = tookDamageThisTurn;
            if(tookDamageLastTurn == false)
            {
            }
            tookDamageThisTurn = false;


            return Task.CompletedTask;
        }

        public override Task AfterCurrentHpChanged(Creature creature, decimal delta)
        {
            //玩家受伤将本回合受伤为true
            if (creature.Side == CombatSide.Player)
            {
                if (    delta < 0)
                {
                    tookDamageThisTurn = true;
                    //PowerCmd.Apply<PerfectTurnPower>(base.Owner, 0m, base.Owner, null);
                }

            }
            return Task.CompletedTask;
        }
        public override async Task AfterTurnEnd(PlayerChoiceContext choiceContext, CombatSide side)
        {
            if (side == base.Owner.Side)
            {
                await PowerCmd.Remove(this);
            }
        }
    }
}
