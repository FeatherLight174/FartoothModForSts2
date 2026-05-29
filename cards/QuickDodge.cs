using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fartooth.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Modding;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.Cards;
using MegaCrit.Sts2.Core.ValueProps;

namespace Fartooth.Cards;
public sealed class QuickDodge : CardModel
{




    //灵活闪避：每有1距离，获得1格挡

    public override bool GainsBlock => true;
    protected override HashSet<CardTag> CanonicalTags => new HashSet<CardTag> { CardTag.Defend };
    protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<Distance>()];

    public override IEnumerable<CardKeyword> CanonicalKeywords => new CardKeyword[] { CardKeyword.Exhaust };
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new BlockVar(2m, ValueProp.Move)
    };
    // 动态变量
    public QuickDodge()
            : base(0, CardType.Skill, CardRarity.Common, TargetType.Self) { }
        // 卡牌的构造函数，指定卡牌的相关属性

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        //int distanceNum = Owner.Creature.Powers.FirstOrDefault(p => p is Distance).Amount;
        //int temporaryDistanceNum = Owner.Creature.Powers.FirstOrDefault(p => p is TemporaryDistance).Amount;
        //int distanceTotal = distanceNum + temporaryDistanceNum;
        int distance = Owner.Creature.GetPowerAmount<Distance>();
        int tempDistance = Owner.Creature.GetPowerAmount<TemporaryDistance>();
        int distanceTotal = distance + tempDistance;
        for (int i = 0; i < distanceTotal; i++)
        {
            await CreatureCmd.GainBlock(base.Owner.Creature, base.DynamicVars.Block, cardPlay);
        }
        
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Block.UpgradeValueBy(1m); // 升级后加 2 点伤害
    }
}
