using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.Models.Powers;
using MegaCrit.Sts2.Core.ValueProps;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

// 你的自定义命名空间
namespace Honkai_Star_Rail;

/// <summary>
/// 花火(HuaHuo)专属 痛击(Bash)
/// 1费 攻击牌 | 造成3点伤害，施加1层易伤
/// 升级后：造成5点伤害，施加2层易伤
/// </summary>
public sealed class Bash_Hua_Huo : CardModel
{
    // 修正：替换编译器内部类型，标准数组
    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[] { HoverTipFactory.FromPower<VulnerablePower>() };

    // 修正：替换编译器内部类型 + 修改数值：3伤害 / 1层易伤
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new DamageVar(3m, ValueProp.Move),
        new PowerVar<VulnerablePower>(1m)
    };

    // 核心修改：费用从 2费 → 1费
    public Bash_Hua_Huo()
        : base(1, CardType.Attack, CardRarity.Basic, TargetType.AnyEnemy)
    {
    }

    // 打出逻辑（保留原版打击特效，无修改）
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, nameof(cardPlay.Target));
        await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_blunt", null, "blunt_attack.mp3")
            .Execute(choiceContext);
        await PowerCmd.Apply<VulnerablePower>(cardPlay.Target, base.DynamicVars.Vulnerable.BaseValue, base.Owner.Creature, this);
    }

    // 升级修改：伤害+2（3→5），易伤+1（1→2）
    protected override void OnUpgrade()
    {
        base.DynamicVars.Damage.UpgradeValueBy(2m);
        base.DynamicVars.Vulnerable.UpgradeValueBy(1m);
    }
}