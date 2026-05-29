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
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;


namespace Fartooth.Cards
{
    /// <summary>
    /// 战斗经验
    /// </summary>
    public sealed class CombatExperience : CardModel
    {
        protected override List<DynamicVar> CanonicalVars => [
        new PowerVar<Distance>(1m)
    ];

        // 构造
        public CombatExperience()
        : base(2, CardType.Power, CardRarity.Uncommon, TargetType.Self) { }

        protected override IEnumerable<IHoverTip> ExtraHoverTips => [HoverTipFactory.FromPower<Distance>()];


        protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
        {

            await CreatureCmd.TriggerAnim(base.Owner.Creature, "Cast", base.Owner.Character.CastAnimDelay);
            await CardPileCmd.Draw(choiceContext, 2, base.Owner);
            await PowerCmd.Apply<CombatExperiencePower>(base.Owner.Creature, base.DynamicVars["Distance"].BaseValue, base.Owner.Creature, this);
        }

        protected override void OnUpgrade()
        {
            base.EnergyCost.UpgradeBy(-1); // 升级后加 3点伤害
        }
    }

}
