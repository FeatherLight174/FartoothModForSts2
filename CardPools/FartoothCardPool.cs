using Godot;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;

namespace Fartooth.CardPools;
public sealed class FartoothCardPool : CardPoolModel
{
    public override string Title => "fartooth";
    public override string EnergyColorName => "fartooth";
    public override string CardFrameMaterialPath => "card_frame_yellow";
    public override Color DeckEntryCardColor => new Color("FCF242");
    public override Color EnergyOutlineColor => new Color("F0ECB1");
    public override bool IsColorless => false;

    protected override CardModel[] GenerateAllCards()
    {
        return new CardModel[]
        {
            ModelDb.Card<PreciseShot>(),
          
        };
    }
}
