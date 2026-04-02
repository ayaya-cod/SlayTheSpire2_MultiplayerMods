using Godot;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Cards;

// 你的自定义命名空间
namespace Honkai_Star_Rail;

public sealed class Hua_Huo_Card_Pool : CardPoolModel
{
    public override string Title => "hua_huo";
    public override string EnergyColorName => "hua_huo";
    public override string CardFrameMaterialPath => "card_frame_hua_huo";
    public override Color DeckEntryCardColor => new Color("#FF6B81");
    public override Color EnergyOutlineColor => new Color("#8B0000");
    public override bool IsColorless => false;

    protected override CardModel[] GenerateAllCards()
    {
        return new CardModel[]
        {
            // 你之前做的花火打击牌
            ModelDb.Card<Strike_Hua_Huo>(),
            ModelDb.Card<Defend_Hua_Huo>(),
            // 这里后面可以加 DefendHuaHuo 防御牌
        };
    }
}
