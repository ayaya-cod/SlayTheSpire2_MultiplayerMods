using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Combat;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Honkai_Star_Rail;

public sealed class Make_Fun_Of_You : CardModel
{
    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;

    protected override IEnumerable<DynamicVar> CanonicalVars => new[] { new EnergyVar(3) };
    
    protected override IEnumerable<IHoverTip> ExtraHoverTips => new[] { base.EnergyHoverTip };
    //确保适用对象的费用大于偷取费用
    protected override bool IsPlayable => base.CurrentTarget.Player.PlayerCombatState.Energy >= base.DynamicVars.Energy.IntValue;
    public Make_Fun_Of_You()
        : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.AnyAlly)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {

        ArgumentNullException.ThrowIfNull(cardPlay.Target);

        // 队友失去 3 能量
        await PlayerCmd.LoseEnergy(base.DynamicVars.Energy.IntValue, cardPlay.Target.Player);

        // 你获得 6 能量
        await PlayerCmd.GainEnergy(base.DynamicVars.Energy.IntValue * 2, base.Owner);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Energy.UpgradeValueBy(1m);
    }
}