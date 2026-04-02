using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;
using MegaCrit.Sts2.Core.ValueProps;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

// 已修改为你自定义的命名空间
namespace Honkai_Star_Rail;

/// <summary>
/// 花火(HuaHuo) 专属基础打击牌
/// 效果：1费 造成6点伤害，升级+3伤害
/// </summary>
public sealed class Strike_Hua_Huo : CardModel
{
    // 卡牌标签：打击（享受所有打击牌加成）
    protected override HashSet<CardTag> CanonicalTags => new HashSet<CardTag> { CardTag.Strike };

    // 基础数值：6点攻击伤害
    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[] { new DamageVar(6m, ValueProp.Move) };

    // 卡牌基础属性：1费 / 攻击牌 / 基础牌 / 目标：任意单个敌人
    public Strike_Hua_Huo()
        : base(1, CardType.Attack, CardRarity.Basic, TargetType.AnyEnemy)
    {
    }

    // 打出卡牌逻辑：造成伤害 + 播放斩击动画
    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, "cardPlay.Target");
        await DamageCmd.Attack(base.DynamicVars.Damage.BaseValue).FromCard(this).Targeting(cardPlay.Target)
            .WithHitFx("vfx/vfx_attack_slash")
            .Execute(choiceContext);
    }

    // 升级效果：伤害+3（6→9）
    protected override void OnUpgrade()
    {
        base.DynamicVars.Damage.UpgradeValueBy(3m);
    }
}