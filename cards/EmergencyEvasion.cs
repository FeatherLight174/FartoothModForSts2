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

namespace Fartooth.cards
{
    public sealed class EmergencyEvasion : CardModel
    {

        protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new BlockVar(14m, ValueProp.Move),
        new PowerVar<Distance>(2m)
    };
        public EmergencyEvasion()
        : base(2, CardType.Skill, CardRarity.Common, TargetType.Self)
        {
        }

        protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<Distance>()];

        // 构造

        //消耗距离的判断
        protected override bool IsPlayable
        {
            get
            {
                // 距离判定,找到目标对象Distance
                var power = Owner.Creature.Powers
           .FirstOrDefault(p => p is Distance);

                if ((power == null) || (Owner.Creature.Powers.OfType<DistanceConsumePower>().FirstOrDefault() != null)) return false;

                return power.Amount >= 2;
            }
        }


        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {

            await CreatureCmd.GainBlock(base.Owner.Creature, base.DynamicVars.Block, cardPlay);
            await PowerCmd.Apply<Distance>(Owner.Creature, -2m, Owner.Creature, null);
        }

        protected override void OnUpgrade()
        {
            base.DynamicVars.Block.UpgradeValueBy(4m); // 升级后加 3点伤害
        }
    }
}

