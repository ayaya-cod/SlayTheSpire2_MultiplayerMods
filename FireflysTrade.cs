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

// 你的专属命名空间
namespace Honkai_Star_Rail;

public sealed class FireflysTrade : CardModel
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => new CardKeyword[] { CardKeyword.Retain };
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new PowerVar<StrengthPower>(1m),
        new PowerVar<DexterityPower>(1m),
        new PowerVar<PlatingPower>(4m),
        new EnergyVar(3)
    };

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[]
    {
        HoverTipFactory.FromPower<StrengthPower>(),
        HoverTipFactory.FromPower<DexterityPower>(),
        HoverTipFactory.FromPower<PlatingPower>(),
        base.EnergyHoverTip
    };

    public FireflysTrade()
        : base(1, CardType.Skill, CardRarity.Rare, TargetType.AnyAlly)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, nameof(cardPlay.Target));

        Creature target = cardPlay.Target;
        if (!target.IsAlive || target.Side != base.Owner.Creature.Side)
            return;

        await CreatureCmd.Kill(target);

        await CreatureCmd.TriggerAnim(base.Owner.Creature, "Cast", base.Owner.Character.CastAnimDelay);

        if (NCombatRoom.Instance != null)
        {
            NFireBurningVfx vfx = NFireBurningVfx.Create(target, 1.5f, false);
            NCombatRoom.Instance.CombatVfxContainer.AddChildSafely(vfx);
        }

        await PowerCmd.Apply<StrengthPower>(base.Owner.Creature, base.DynamicVars.Strength.BaseValue, base.Owner.Creature, this);
        await PowerCmd.Apply<DexterityPower>(base.Owner.Creature, base.DynamicVars.Dexterity.BaseValue, base.Owner.Creature, this);
        await PowerCmd.Apply<PlatingPower>(base.Owner.Creature, base.DynamicVars["PlatingPower"].BaseValue, base.Owner.Creature, this);

        int powerLevel = IsUpgraded ? 2 : 1;
        await PowerCmd.Apply<FireflysTradePower>(base.Owner.Creature, powerLevel, base.Owner.Creature, this);
    }

    public override async Task OnEnqueuePlayVfx(Creature? target)
    {
        if (NCombatRoom.Instance == null || target == null)
            return;

        NFireBurningVfx child = NFireBurningVfx.Create(target, 1.5f, false);
        NCombatRoom.Instance.CombatVfxContainer.AddChildSafely(child);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Strength.UpgradeValueBy(1m);  // 1 → 2
        base.DynamicVars.Dexterity.UpgradeValueBy(1m); // 1 → 2
        // PlatingPower 保持4层不升级
        base.DynamicVars.Energy.UpgradeValueBy(1m);    // 3 → 4
        // 能量消耗保持1点不减少
    }
}