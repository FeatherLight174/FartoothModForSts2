using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fartooth.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;

//战术后撤：获得格挡，本回合内距离+2
namespace Fartooth.Cards
{
    public sealed class StrategicRetreat : CardModel
    {
        public override bool GainsBlock => true;
        protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<Distance>()];
        protected override HashSet<CardTag> CanonicalTags => new HashSet<CardTag> { CardTag.Defend };
        protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
        {
        new BlockVar(4m, ValueProp.Move),
        };
        // 动态变量
        public StrategicRetreat()
                : base(0, CardType.Skill, CardRarity.Common, TargetType.Self) { }
        // 卡牌的构造函数，指定卡牌的相关属性

        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {
            await CreatureCmd.TriggerAnim(base.Owner.Creature, "Cast", base.Owner.Character.CastAnimDelay);
            
            await CreatureCmd.GainBlock(base.Owner.Creature, base.DynamicVars.Block, cardPlay);
            if (Owner.Creature.Powers.OfType<DistanceAvailablePower>().FirstOrDefault() == null)
            {
                await PowerCmd.Apply<TemporaryDistance>(base.Owner.Creature, 2m, base.Owner.Creature, this);
            }
                
        }

        protected override void OnUpgrade()
        {
            base.DynamicVars.Block.UpgradeValueBy(2m); // 升级后加 2 点伤害
        }
    }
}
