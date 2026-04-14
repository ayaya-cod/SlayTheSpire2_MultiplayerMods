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

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[] { new EnergyVar(1) };

    protected override IEnumerable<IHoverTip> ExtraHoverTips => System.Array.Empty<IHoverTip>();

    protected override bool IsPlayable
    {
        get
        {
            if (!base.IsPlayable)
                return false;

            // 如果没有目标，检查是否有任意队友有足够能量
            if (CurrentTarget == null)
            {
                // 检查是否有任意队友有足够能量
                if (base.Owner?.Creature?.CombatState == null)
                    return false;

                var combatState = base.Owner.Creature.CombatState;
                var playerCreatures = combatState.PlayerCreatures;

                // 寻找有足够能量的队友（不包括自己）
                foreach (var creature in playerCreatures)
                {
                    if (creature == base.Owner.Creature || !creature.IsAlive)
                        continue;

                    var player = creature.Player;
                    if (player != null && player.PlayerCombatState != null &&
                        player.PlayerCombatState.Energy >= base.DynamicVars.Energy.IntValue)
                    {
                        return true;
                    }
                }

                return false;
            }
            else
            {
                // 检查当前目标是否有足够能量
                if (!CurrentTarget.IsAlive || CurrentTarget.Side != base.Owner?.Creature?.Side)
                    return false;

                var targetPlayer = CurrentTarget.Player;
                if (targetPlayer == null || targetPlayer.PlayerCombatState == null)
                    return false;

                return targetPlayer.PlayerCombatState.Energy >= base.DynamicVars.Energy.IntValue;
            }
        }
    }

    protected override bool ShouldGlowGoldInternal => IsPlayable;

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

        int stealAmount = base.DynamicVars.Energy.IntValue;

        // 再次检查目标是否有足够能量（防御性检查）
        if (targetPlayer.PlayerCombatState.Energy < stealAmount)
            return;

        // 队友失去能量
        await PlayerCmd.LoseEnergy(stealAmount, targetPlayer);

        // 自己获得双倍能量
        await PlayerCmd.GainEnergy(stealAmount * 2, base.Owner);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Energy.UpgradeValueBy(1m);
    }
}