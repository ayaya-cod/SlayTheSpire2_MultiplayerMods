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
using System.Linq;
using System.Threading.Tasks;

namespace Honkai_Star_Rail;

public sealed class Revive : CardModel
{
    public override IEnumerable<CardKeyword> CanonicalKeywords => new CardKeyword[] { CardKeyword.Exhaust };

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[] { new HealVar(15m), new EnergyVar(3), new CardsVar(3) };

    protected override IEnumerable<IHoverTip> ExtraHoverTips => System.Array.Empty<IHoverTip>();

    public Revive()
        : base(2, CardType.Skill, CardRarity.Rare, TargetType.Self) // ⚠️ 注意：这里改成了 TargetType.Self
    {
    }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        var selfCreature = base.Owner?.Creature;
        if (selfCreature == null)
            return;

        // ⚠️ 折中方案：由于官方 IsValidTarget 不允许选死人，我们改为“自动复活生命值最低的死亡友方”
        if (NCombatRoom.Instance == null)
            return;

        // 找到所有死亡的友方单位
        // 使用 CombatState 获取战场上的生物
        var combatState = selfCreature.CombatState;
        if (combatState == null)
            return;

        var deadAllies = combatState.Creatures
            .Where(c => c.Side == selfCreature.Side && !c.IsAlive)
            .OrderBy(c => c.MaxHp) // 优先复活血量上限最低的（通常是脆皮队友）
            .ToList();

        if (deadAllies.Count == 0)
            return; // 没有死人，就不执行任何效果

        var target = deadAllies.First();

        // 下面是原本的复活逻辑
        await CreatureCmd.TriggerAnim(selfCreature, "Cast", base.Owner.Character.CastAnimDelay);

        var vfx = NFireBurningVfx.Create(target, 2f, false);
        NCombatRoom.Instance.CombatVfxContainer.AddChildSafely(vfx);

        decimal healAmount = Math.Max(1m, (decimal)target.MaxHp * (base.DynamicVars.Heal.BaseValue / 100m));
        await CreatureCmd.Heal(target, healAmount);

        // 复活后获得3点能量并抽三张牌
        if (target.Player != null)
        {
            await PlayerCmd.GainEnergy(base.DynamicVars.Energy.IntValue, target.Player);
            await CardPileCmd.Draw(choiceContext, base.DynamicVars.Cards.IntValue, target.Player);
        }
    }

    public override async Task OnEnqueuePlayVfx(Creature? target)
    {
        if (NCombatRoom.Instance == null)
            return;

        var selfCreature = base.Owner?.Creature;
        if (selfCreature == null)
            return;

        // 找到生命值最低的死亡友方（与OnPlay逻辑一致）
        var combatState = selfCreature.CombatState;
        if (combatState == null)
            return;

        var deadAlly = combatState.Creatures
            .Where(c => c.Side == selfCreature.Side && !c.IsAlive)
            .OrderBy(c => c.MaxHp)
            .FirstOrDefault();

        if (deadAlly == null)
            return;

        var child = NFireBurningVfx.Create(deadAlly, 2f, false);
        NCombatRoom.Instance.CombatVfxContainer.AddChildSafely(child);
    }

    protected override void OnUpgrade()
    {
        base.DynamicVars.Heal.UpgradeValueBy(10m);
    }
}