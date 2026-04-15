using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Players;
using MegaCrit.Sts2.Core.Entities.Powers;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Models;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Honkai_Star_Rail;

public sealed class FireflysTradePower : PowerModel
{
    public override PowerType Type => PowerType.Buff;
    public override PowerStackType StackType => PowerStackType.Counter;

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[] { HoverTipFactory.ForEnergy(this) };

    // 抽牌效果
    public override decimal ModifyHandDraw(Player player, decimal count)
    {
        if (player != base.Owner.Player) return count;
        return count + base.Amount switch { 1 => 2m, 2 => 3m, _ => 0m };
    }

    // 回合开始触发
    public override async Task AfterPlayerTurnStart(PlayerChoiceContext choiceContext, Player player)
    {
        if (player != base.Owner.Player || !base.Owner.IsAlive) return;

        // 获得能量：等级1获得1点，等级2获得2点
        await PlayerCmd.GainEnergy(base.Amount, base.Owner.Player);

        Flash();
    }
}