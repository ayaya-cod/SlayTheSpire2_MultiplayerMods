//using MegaCrit.Sts2.Core.Commands;
//using MegaCrit.Sts2.Core.Entities.Cards;
//using MegaCrit.Sts2.Core.Entities.Creatures;
//using MegaCrit.Sts2.Core.GameActions.Multiplayer;
//using MegaCrit.Sts2.Core.Helpers;
//using MegaCrit.Sts2.Core.HoverTips;
//using MegaCrit.Sts2.Core.Localization.DynamicVars;
//using MegaCrit.Sts2.Core.Models;
//using MegaCrit.Sts2.Core.Models.CardPools;
//using MegaCrit.Sts2.Core.Factories;
//using MegaCrit.Sts2.Core.Nodes.Rooms;
//using MegaCrit.Sts2.Core.Nodes.Vfx;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;

//namespace Honkai_Star_Rail;

//public sealed class Revive : CardModel
//{
//    public override IEnumerable<CardKeyword> CanonicalKeywords => new CardKeyword[] { CardKeyword.Exhaust };

//    protected override IEnumerable<DynamicVar> CanonicalVars => new DynamicVar[] { new HealVar(15m), new EnergyVar(3), new CardsVar(3) };

//    protected override IEnumerable<IHoverTip> ExtraHoverTips => System.Array.Empty<IHoverTip>();

//    public Revive()
//        : base(2, CardType.Skill, CardRarity.Rare, TargetType.AnyAlly)
//    {
//    }

//    protected override async Task OnPlay(PlayerChoiceContext choiceContext, CardPlay cardPlay)
//    {
//        ArgumentNullException.ThrowIfNull(cardPlay.Target, nameof(cardPlay.Target));

//        var target = cardPlay.Target;


//        var selfCreature = base.Owner?.Creature;
//        if (selfCreature == null)
//            return;

//        if (NCombatRoom.Instance == null)
//            return;

//        // 下面是原本的复活逻辑
//        await CreatureCmd.TriggerAnim(selfCreature, "Cast", base.Owner.Character.CastAnimDelay);

//        var vfx = NFireBurningVfx.Create(target, 2f, false);
//        NCombatRoom.Instance.CombatVfxContainer.AddChildSafely(vfx);

//        decimal healAmount = Math.Max(1m, (decimal)target.MaxHp * (base.DynamicVars.Heal.BaseValue / 100m));
//        await CreatureCmd.Heal(target, healAmount);

//        // 复活后获得3点能量并重新构建牌组
//        if (target.Player != null)
//        {
//            await PlayerCmd.GainEnergy(base.DynamicVars.Energy.IntValue, target.Player);

//            // 使用Largesse相同的写法处理牌组构建
//            int cardsToAdd = 20; // 固定添加20张牌到抽牌堆
//            int cardsToDraw = base.DynamicVars.Cards.IntValue; // 抽3张牌

//            // 1. 添加20张随机无色牌到抽牌堆（使用与Largesse完全相同的写法）
//            var cardsToAddToDeck = CardFactory.GetDistinctForCombat(
//                target.Player,
//                ModelDb.CardPool<ColorlessCardPool>().GetUnlockedCards(target.Player.UnlockState, target.Player.RunState.CardMultiplayerConstraint),
//                cardsToAdd,
//                base.Owner.RunState.Rng.CombatCardGeneration
//            );

//            if (cardsToAddToDeck.Any())
//            {
//                foreach (var cardModel in cardsToAddToDeck)
//                {
//                    // 添加到抽牌堆 (PileType.Draw)
//                    await CardPileCmd.Add(cardModel, PileType.Draw, CardPilePosition.Top);
//                    await Cmd.Wait(0.05f);
//                }

//                // 2. 从抽牌堆抽三张牌到手牌
//                await CardPileCmd.Draw(choiceContext, cardsToDraw, target.Player);
//            }
//            else
//            {
//                // 如果无法添加牌到抽牌堆，直接添加3张牌到手牌（使用与Largesse完全相同的写法）
//                var cardsToGive = CardFactory.GetDistinctForCombat(
//                    target.Player,
//                    ModelDb.CardPool<ColorlessCardPool>().GetUnlockedCards(target.Player.UnlockState, target.Player.RunState.CardMultiplayerConstraint),
//                    cardsToDraw,
//                    base.Owner.RunState.Rng.CombatCardGeneration
//                );

//                foreach (var cardModel in cardsToGive)
//                {
//                    await CardPileCmd.AddGeneratedCardToCombat(cardModel, PileType.Hand, addedByPlayer: true);
//                    await Cmd.Wait(0.1f);
//                }
//            }
//        }
//    }

//    public override async Task OnEnqueuePlayVfx(Creature? target)
//    {
//        if (NCombatRoom.Instance == null || target == null)
//            return;

//        // 只对死亡目标显示特效
//        if (target.IsAlive)
//            return;

//        var child = NFireBurningVfx.Create(target, 2f, false);
//        NCombatRoom.Instance.CombatVfxContainer.AddChildSafely(child);
//    }

//    protected override void OnUpgrade()
//    {
//        base.DynamicVars.Heal.UpgradeValueBy(10m);
//    }

//    // 重写IsValidTarget以允许选择死亡的队友
//    public override bool IsValidTarget(Creature? target)
//    {
//        if (target == null)
//        {
//            // 对于AnyAlly类型，null不是有效目标
//            return false;
//        }

//        // 检查是否是友方
//        if (target.Side != base.Owner?.Creature?.Side)
//        {
//            return false;
//        }

//        // Revive卡牌只允许选择死亡的队友
//        if (target.IsAlive)
//        {
//            return false;
//        }

//        return true;
//    }
//}