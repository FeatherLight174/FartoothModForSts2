using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers; 
using MegaCrit.Sts2.Core.ValueProps;

namespace Fartooth.Cards;
public sealed class DefendFartooth : CardModel
{
    public override bool GainsBlock => true;
    protected override HashSet<CardTag> CanonicalTags => new HashSet<CardTag> { CardTag.Defend };
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new BlockVar(5m, ValueProp.Move)
    };
    // 动态变量

    public DefendFartooth()
		: base(0, CardType.Skill, CardRarity.Basic, TargetType.Self) { }
	// 卡牌的构造函数，指定卡牌的相关属性

	protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
	{
        await CreatureCmd.GainBlock(base.Owner.Creature, base.DynamicVars.Block, cardPlay);
    }

	protected override void OnUpgrade()
	{
		base.DynamicVars.Block.UpgradeValueBy(3m); // 升级后加 2 点伤害
	}
}
