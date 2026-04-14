using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Honkai_Star_Rail;

public sealed class Make_Fun_Of_You : CardModel
{
    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[] { new EnergyVar(3) };

    protected override IEnumerable<IHoverTip> ExtraHoverTips => System.Array.Empty<IHoverTip>();

    protected override bool IsPlayable
    {
        get
        {
            if (!base.IsPlayable)
                return false;

            Creature? target = base.CurrentTarget;
            if (target == null)
                return true;

            var targetPlayer = target.Player;
            if (targetPlayer == null || targetPlayer.PlayerCombatState == null)
                return false;

            return targetPlayer.PlayerCombatState.Energy >= base.DynamicVars.Energy.IntValue;
        }
    }

    public Make_Fun_Of_You()
        : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.AnyAlly)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");

        var targetPlayer = cardPlay.Target.Player;
        if (targetPlayer == null || targetPlayer.PlayerCombatState == null)
            return;

        // 队友失去 3 能量（升级后 4 能量）
        await PlayerCmd.LoseEnergy(base.DynamicVars.Energy.IntValue, targetPlayer);

        // 你获得 6 能量（升级后 8 能量）
        await PlayerCmd.GainEnergy(base.DynamicVars.Energy.IntValue * 2, base.Owner);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Energy.UpgradeValueBy(1m);
    }
}