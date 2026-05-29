using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fartooth.Powers;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;

namespace Fartooth.Cards
{
    /// <summary>
    /// 攻击：近身射击
    /// </summary>
    public sealed class SeekingSupplies : CardModel
    {
        protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<Distance>()];
        protected override List<DynamicVar> CanonicalVars => [
            new CardsVar(2),
    ];

        public SeekingSupplies()
        : base(0, CardType.Skill, CardRarity.Common, TargetType.Self) { }

        protected override bool IsPlayable
        {
            get
            {
                // 距离判定,找到目标对象Distance
                var power = Owner.Creature.Powers
           .FirstOrDefault(p => p is Distance);

                if ((power == null) || (Owner.Creature.Powers.OfType<DistanceConsumePower>().FirstOrDefault() != null)) return false;

                return power.Amount >= 1;
            }
        }


        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {

            await CardPileCmd.Draw(choiceContext, base.DynamicVars.Cards.BaseValue, base.Owner);
            await PowerCmd.Apply<Distance>(Owner.Creature, -1m, Owner.Creature, null);
        }

        protected override void OnUpgrade()
        {
            base.DynamicVars.Cards.UpgradeValueBy(1m); // 升级后加 3点伤害
        }

    }

}
