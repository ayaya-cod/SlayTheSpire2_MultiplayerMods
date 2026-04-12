using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Helpers;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Nodes.Rooms;
using MegaCrit.Sts2.Core.Nodes.Vfx;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Honkai_Star_Rail;

public sealed class Revive : CardModel
{
    public override List<CardKeyword> CanonicalKeywords => [
    CardKeyword.Exhaust
];
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new HealVar(15m)
    };

    protected override IEnumerable<IHoverTip> ExtraHoverTips => System.Array.Empty<IHoverTip>();
    //三点能量，技能牌
    public Revive()
        : base(2, CardType.Skill, CardRarity.Rare, TargetType.AnyAlly)
    {
    }


    public new bool IsValidTarget(Creature? target)
    {
        if (target == null || base.Owner?.Creature == null)
            return false;

        return target.Side == base.Owner.Creature.Side && !target.IsAlive;
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var selfCreature = base.Owner?.Creature;
        var target = cardPlay.Target;
        if (selfCreature == null || !selfCreature.IsAlive)
            return;
        if (target == null || target.IsAlive || target.Side != selfCreature.Side)
            return;

        await CreatureCmd.TriggerAnim(selfCreature, "Cast", base.Owner.Character.CastAnimDelay);

        if (NCombatRoom.Instance != null)
        {
            var vfx = NFireBurningVfx.Create(target, 2f, false);
            NCombatRoom.Instance.CombatVfxContainer.AddChildSafely(vfx);
        }

        decimal healAmount = Math.Max(1m, (decimal)target.MaxHp * (base.DynamicVars.Heal.BaseValue / 100m));
        await CreatureCmd.Heal(target, healAmount);
    }

    public override async Task OnEnqueuePlayVfx(Creature? target)
    {
        if (NCombatRoom.Instance == null || target == null)
            return;

        var child = NFireBurningVfx.Create(target, 2f, false);
        NCombatRoom.Instance.CombatVfxContainer.AddChildSafely(child);
    }

    protected override void OnUpgrade()
    {
        //升级后多回复5点生命
        base.DynamicVars.Heal.UpgradeValueBy(10m);
;
    }
}