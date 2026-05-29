using System.Collections.Generic;
using System.Threading.Tasks;
using Fartooth.Cards;
using Godot;
using MegaCrit.Sts2.Core.Combat;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

namespace Fartooth.Powers;

public sealed class Mark : PowerModel
{
    public override PowerType Type => PowerType.Debuff;
    public override PowerStackType StackType => PowerStackType.Counter;

    public override bool IsInstanced => false;

    // 允许层数为负数
    private const string _damageIncrease = "DamageIncrease";


    public override decimal ModifyDamageAdditive(
    Creature? target,
    decimal amount,
    ValueProp props,
    Creature? dealer,
    CardModel? cardSource)
    {
        if (target != base.Owner)
        {
            return 0m;
        }
        if (!props.IsPoweredAttack())
        {
            return 0m;
        }
        if (cardSource is AllianceSupport)
        {
            return 0m;
        }
        decimal extra = Amount * 2m;
        
        
        decimal vulnMult = 1m;
        var vuln = target.GetPower<VulnerablePower>();
        if (vuln != null)
        {
            vulnMult = vuln.DynamicVars["DamageIncrease"].BaseValue;
        }
        return extra / vulnMult;
    }


}