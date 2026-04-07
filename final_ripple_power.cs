using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Honkai_Star_Rail;

public sealed class Final_Ripple_Power : PowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new[] { HoverTipFactory.ForEnergy(this) };

    // 抽牌增益（仅buff持有者生效）
    public override decimal ModifyHandDraw(Player player, decimal count)
    {
        if (player != base.Owner.Player) return count;
        return count + base.Amount switch { 1 => 3m, 2 => 4m, _ => 0m };
    }

    // 回合开始获得能量（仅buff持有者生效）
    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != base.Owner.Player || !base.Owner.IsAlive) return;

        int energy = base.Amount switch { 1 => 3, 2 => 4, _ => 0 };
        await PlayerCmd.GainEnergy(energy, base.Owner.Player);
        Flash();
    }
}