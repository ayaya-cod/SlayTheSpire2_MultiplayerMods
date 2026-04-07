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
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Honkai_Star_Rail;

public sealed class Final_Ripple : CardModel
{
    public override List<CardKeyword> CanonicalKeywords => [
    CardKeyword.Retain
];
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new PowerVar<StrengthPower>(2m),
        new PowerVar<DexterityPower>(2m),
        new PowerVar<PlatingPower>(5m),
        new EnergyVar(3)
    };

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[]
    {
        HoverTipFactory.FromPower<StrengthPower>(),
        HoverTipFactory.FromPower<DexterityPower>(),
        HoverTipFactory.FromPower<PlatingPower>(),
        base.EnergyHoverTip
    };

    // 关键1：目标改回【选定单个盟友】
    public Final_Ripple()
        : base(1, CardType.Skill, CardRarity.Rare, TargetType.AnyAlly)
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var selfCreature = base.Owner.Creature;
        // 关键2：获取你【手动选定的单个队友】
        var target = cardPlay.Target;

        // 双重校验：自己存活 + 目标是合法存活盟友
        if (selfCreature == null || !selfCreature.IsAlive)
            return;
        if (target == null || !target.IsAlive || target.Side != selfCreature.Side)
            return;

        // 核心：杀死自己
        await CreatureCmd.Kill(selfCreature);

        // 播放施法动画
        await CreatureCmd.TriggerAnim(selfCreature, "Cast", base.Owner.Character.CastAnimDelay);

        // 自杀VFX特效
        if (NCombatRoom.Instance != null)
        {
            NFireBurningVfx vfx = NFireBurningVfx.Create(selfCreature, 1.5f, false);
            NCombatRoom.Instance.CombatVfxContainer.AddChildSafely(vfx);
        }

        // 关键3：仅对【选定的这一个队友】施加所有增益（无循环，无全体）
        int powerLevel = IsUpgraded ? 2 : 1;
        await PowerCmd.Apply<StrengthPower>(target, base.DynamicVars.Strength.BaseValue, selfCreature, this);
        await PowerCmd.Apply<DexterityPower>(target, base.DynamicVars.Dexterity.BaseValue, selfCreature, this);
        await PowerCmd.Apply<PlatingPower>(target, base.DynamicVars["PlatingPower"].BaseValue, selfCreature, this);
        await PowerCmd.Apply<Final_Ripple_Power>(target, powerLevel, selfCreature, this);
    }

    public override async Task OnEnqueuePlayVfx(Creature? target)
    {
        var selfCreature = base.Owner.Creature;
        if (NCombatRoom.Instance == null || selfCreature == null)
            return;

        NFireBurningVfx child = NFireBurningVfx.Create(selfCreature, 1.5f, false);
        NCombatRoom.Instance.CombatVfxContainer.AddChildSafely(child);
    }

    protected override void OnUpgrade()
    {
        // 升级逻辑完全保留
        base.DynamicVars.Strength.UpgradeValueBy(1m);
        base.DynamicVars.Dexterity.UpgradeValueBy(1m);
        base.DynamicVars["PlatingPower"].UpgradeValueBy(4m);
        base.DynamicVars.Energy.UpgradeValueBy(1m);
        base.EnergyCost.UpgradeBy(-1);
    }
}