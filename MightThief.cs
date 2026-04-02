using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Honkai_Star_Rail;

public sealed class Might_Thief : CardModel
{


    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[] { new PowerVar<StrengthPower>(5m) };

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[] { HoverTipFactory.FromPower<StrengthPower>() };

    public Might_Thief()
        : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.AnyAlly)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        decimal stealAmount = base.DynamicVars.Strength.BaseValue;

        // 从目标队友处移除力量 )
        await PowerCmd.Apply<StrengthPower>(cardPlay.Target, -stealAmount, base.Owner.Creature, this);
        // 给自己添加等量的力量
        await PowerCmd.Apply<StrengthPower>(base.Owner.Creature, stealAmount, base.Owner.Creature, this);
    }

    protected override void OnUpgrade()
    {
        //升级数值 (+3)
        base.DynamicVars.Strength.UpgradeValueBy(3m);
    }
}