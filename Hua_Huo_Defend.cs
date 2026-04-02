using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.ValueProps;

namespace MegaCrit.Sts2.Core.Models.Cards;

/// <summary>
/// 花火（Hanabi）的基础防御卡牌，逻辑与铁甲战士的DefendIronclad完全一致
/// </summary>
public sealed class Defend_Hua_Huo : CardModel
{
    // 标识卡牌提供格挡值
    public override bool GainsBlock => true;

    // 卡牌的标准标签（防御标签）
    protected override HashSet<CardTag> CanonicalTags => new HashSet<CardTag> { CardTag.Defend };

    // 卡牌的基础格挡值（5点，升级后+3点）
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[] { new BlockVar(6m, ValueProp.Move) };

    // 构造函数：1费、技能牌、基础稀有度、目标为自己
    public Defend_Hua_Huo()
        : base(1, CardType.Skill, CardRarity.Basic, TargetType.Self)
    {
    }

    // 卡牌打出时的逻辑：为卡牌拥有者的生物增加格挡值
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        await CreatureCmd.GainBlock(base.Owner.Creature, base.DynamicVars.Block, cardPlay);
    }

    // 卡牌升级时的逻辑：格挡值增加3点（升级后总格挡值8点）
    protected override void OnUpgrade()
    {
        base.DynamicVars.Block.UpgradeValueBy(3m);
    }
}