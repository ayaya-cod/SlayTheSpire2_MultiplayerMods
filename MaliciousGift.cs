using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Context;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Factories;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.HoverTips;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models.CardPools;
using MegaCrit.Sts2.Core.Models.Cards;
using MegaCrit.Sts2.Core.Models;

namespace Honkai_Star_Rail;

public sealed class Malicious_Gift : CardModel
{
    private const string EnergyKey = "Energy";
    private const string CardsKey = "Cards";

    public override CardMultiplayerConstraint MultiplayerConstraint => CardMultiplayerConstraint.MultiplayerOnly;

    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[]
    {
        new EnergyVar(EnergyKey, 2),
        new CardsVar(CardsKey, 2)
    };

    protected override IEnumerable<IHoverTip> ExtraHoverTips => new IHoverTip[] { HoverTipFactory.FromCard<Normality>() };

    public Malicious_Gift()
        : base(
            canonicalEnergyCost: 1,
            type: CardType.Skill,
            rarity: CardRarity.Uncommon,
            targetType: TargetType.AnyAlly,
            shouldShowInCardLibrary: true
        )
    { }

    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
    {
        ArgumentNullException.ThrowIfNull(cardPlay.Target, nameof(cardPlay.Target));
        await CreatureCmd.TriggerAnim(base.Owner.Creature, "Cast", base.Owner.Character.CastAnimDelay);

        CardModel normalityCard = CardFactory.GetDistinctForCombat(
            cardPlay.Target.Player,
            ModelDb.CardPool<StatusCardPool>()
                .GetUnlockedCards(cardPlay.Target.Player.UnlockState, cardPlay.Target.Player.RunState.CardMultiplayerConstraint)
                .Where(c => c is Normality),
            1,
            base.Owner.RunState.Rng.CombatCardGeneration
        ).FirstOrDefault();

        if (normalityCard != null)
        {
            await CardPileCmd.AddGeneratedCardToCombat(normalityCard, PileType.Hand, addedByPlayer: true);
            await Cmd.Wait(0.25f);
        }

        await PlayerCmd.GainEnergy(base.DynamicVars[EnergyKey].IntValue, base.Owner);
        await CardPileCmd.Draw(choiceContext, base.DynamicVars[CardsKey].IntValue, base.Owner);
    }

    protected override void OnUpgrade()
    {
        base.EnergyCost.UpgradeBy(-1);
        base.DynamicVars[CardsKey].UpgradeValueBy(1m);
    }
}