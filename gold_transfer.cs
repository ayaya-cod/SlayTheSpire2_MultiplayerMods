using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Honkai_Star_Rail;

public sealed class GoldTransfer : CardModel
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => Array.Empty<CardKeyword>();

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new GoldVar(20) // 队友失去的金币（自己获得双倍）
    };

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[]
    {
        // 金币相关的提示可能需要通过其他方式实现
        // 暂时留空，等待确定正确的HoverTip类型
    };

    public GoldTransfer()
        : base(0, CardType.Skill, CardRarity.Uncommon, TargetType.AnyAlly)
    {
    }

    protected override bool IsPlayable
    {
        get
        {
            // 如果没有目标，检查是否有任意队友有足够金币
            if (CurrentTarget == null)
            {
                // 检查是否有任意队友有足够金币
                if (base.Owner?.Creature?.CombatState == null)
                    return false;

                var combatState = base.Owner.Creature.CombatState;
                var playerCreatures = combatState.PlayerCreatures;

                // 寻找有足够金币的队友（不包括自己）
                foreach (var creature in playerCreatures)
                {
                    if (creature == base.Owner.Creature || !creature.IsAlive)
                        continue;

                    var player = creature.Player;
                    if (player != null && player.Gold >= GetRequiredGold())
                    {
                        return true;
                    }
                }

                return false;
            }
            else
            {
                // 检查当前目标是否有足够金币
                if (!CurrentTarget.IsAlive || CurrentTarget.Side != base.Owner?.Creature?.Side)
                    return false;

                var targetPlayer = CurrentTarget.Player;
                if (targetPlayer == null)
                    return false;

                return targetPlayer.Gold >= GetRequiredGold();
            }
        }
    }

    protected override bool ShouldGlowGoldInternal => IsPlayable;

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, nameof(cardPlay.Target));

        var selfCreature = base.Owner?.Creature;
        if (selfCreature == null || !selfCreature.IsAlive)
            return;

        var target = cardPlay.Target;
        if (!target.IsAlive || target.Side != selfCreature.Side)
            return;

        // 播放施法动画
        await CreatureCmd.TriggerAnim(selfCreature, "Cast", base.Owner.Character.CastAnimDelay);

        // 获取金币数值（队友失去，自己获得双倍）
        int loseGold = base.DynamicVars.Gold.IntValue;
        int gainGold = loseGold * 2;

        // 队友失去金币
        if (target.Player != null)
        {
            await PlayerCmd.LoseGold(loseGold, target.Player);
        }

        // 自己获得金币
        if (base.Owner != null)
        {
            await PlayerCmd.LoseGold(-gainGold, base.Owner); // 使用负值表示获得金币
        }

        // 播放VFX特效
        if (NCombatRoom.Instance != null)
        {
            NFireBurningVfx vfx = NFireBurningVfx.Create(target, 1.0f, false);
            NCombatRoom.Instance.CombatVfxContainer.AddChildSafely(vfx);
        }
    }

    public override async Task OnEnqueuePlayVfx(Creature? target)
    {
        var selfCreature = base.Owner?.Creature;
        if (NCombatRoom.Instance == null || selfCreature == null)
            return;

        // 预播放VFX
        NFireBurningVfx child = NFireBurningVfx.Create(selfCreature, 1.0f, false);
        NCombatRoom.Instance.CombatVfxContainer.AddChildSafely(child);
    }

    protected override void OnUpgrade()
    {
        // 升级：队友失去30金币（自己获得60金币）
        base.DynamicVars.Gold.UpgradeValueBy(10); // 20 -> 30
    }

    private decimal GetRequiredGold()
    {
        return base.DynamicVars.Gold.BaseValue;
    }
}