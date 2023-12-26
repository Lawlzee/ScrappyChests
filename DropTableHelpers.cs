using RoR2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScrappyChests;

public static class DropTableHelpers
{
    public static IDisposable ReplaceDropTable(this PickupDropTable dropTable, string caller)
    {
        if (dropTable is BasicPickupDropTable basicDropTable)
        {
            IDisposable disposable = CreateSelectorCopy(basicDropTable.selector, x => basicDropTable.selector = x);
            ReplaceDropTableSelector(basicDropTable.selector);
            return disposable;
        }

        if (dropTable is ExplicitPickupDropTable explicitDropTable)
        {
            IDisposable disposable = CreateSelectorCopy(explicitDropTable.weightedSelection, x => explicitDropTable.weightedSelection = x);
            ReplaceDropTableSelector(explicitDropTable.weightedSelection);
            return disposable;
        }

        if (dropTable is FreeChestDropTable freeChestDropTable)
        {
            IDisposable disposable = CreateSelectorCopy(freeChestDropTable.selector, x => freeChestDropTable.selector = x);
            ReplaceDropTableSelector(freeChestDropTable.selector);
            return disposable;
        }

        if (dropTable is DoppelgangerDropTable doppelgangerDropTable)
        {
            IDisposable disposable = CreateSelectorCopy(doppelgangerDropTable.selector, x => doppelgangerDropTable.selector = x);
            ReplaceDropTableSelector(doppelgangerDropTable.selector);
            return disposable;
        }

        if (dropTable is ArenaMonsterItemDropTable arenaMonsterItemDropTable)
        {
            IDisposable disposable = CreateSelectorCopy(arenaMonsterItemDropTable.selector, x => arenaMonsterItemDropTable.selector = x);
            ReplaceDropTableSelector(arenaMonsterItemDropTable.selector);
            return disposable;
        }

        Log.Warning($"Failed to override {caller} dropTable");
        Log.Warning($"{caller} dropTable is of type {dropTable?.GetType().FullName ?? "null"}");

        return Disposable.Empty;

        void ReplaceDropTableSelector(WeightedSelection<PickupIndex> selector)
        {
            for (int i = 0; i < selector.Count; i++)
            {
                ref var choice = ref selector.choices[i];

                bool replaceItem = choice.value.pickupDef.itemTier switch
                {
                    ItemTier.Tier1 => Configuration.Instance.ReplaceWhiteItems.Value,
                    ItemTier.Tier2 => Configuration.Instance.ReplaceGreenItems.Value,
                    ItemTier.Tier3 => Configuration.Instance.ReplaceRedItems.Value,
                    ItemTier.Boss => Configuration.Instance.ReplaceYellowItems.Value,
                    ItemTier.Lunar => Configuration.Instance.ReplaceBlueItems.Value,
                    ItemTier.VoidTier1 => Configuration.Instance.ReplaceVoidTier1Items.Value,
                    ItemTier.VoidTier2 => Configuration.Instance.ReplaceVoidTier2Items.Value,
                    ItemTier.VoidTier3 => Configuration.Instance.ReplaceVoidTier3Items.Value,
                    _ => false
                };

                if (!replaceItem)
                {
                    continue;
                }

                string scrapPickupName = choice.value.pickupDef.itemTier switch
                {
                    ItemTier.Tier1 => "ItemIndex.ScrapWhite",
                    ItemTier.Tier2 => "ItemIndex.ScrapGreen",
                    ItemTier.Tier3 => "ItemIndex.ScrapRed",
                    ItemTier.Boss => "ItemIndex.ScrapYellow",
                    ItemTier.Lunar => "ItemIndex.LunarTrinket",
                    ItemTier.VoidTier1 => "ItemIndex.ScrapWhite",
                    ItemTier.VoidTier2 => "ItemIndex.ScrapGreen",
                    ItemTier.VoidTier3 => "ItemIndex.ScrapRed",
                    _ => throw new Exception("Unreachable")
                };

                if (scrapPickupName != null)
                {
                    choice.value = PickupCatalog.FindPickupIndex(scrapPickupName);
                }
            }

            UpdateSpeedItemsSpawnRate(selector);

            Log.Debug($"{caller} {dropTable.GetType().Name} ({dropTable.name}) replaced");
        }
    }

    public static void UpdateSpeedItemsSpawnRate(WeightedSelection<PickupIndex> selector)
    {
        if (Configuration.Instance.SpeedItemSpawnMultiplier.Value == 1)
        {
            return;
        }

        var speedItems = new (ItemIndex Item, float Weigth)[]
        {
            (RoR2Content.Items.Hoof.itemIndex, 1),//Paul's Goat Hoof
            (RoR2Content.Items.SprintBonus.itemIndex, 1),//Energy Drink
            (DLC1Content.Items.AttackSpeedAndMoveSpeed.itemIndex, 1),//Mocha
            (RoR2Content.Items.SprintOutOfCombat.itemIndex, 1),//Red Whip
            (RoR2Content.Items.JumpBoost.itemIndex, 1),//Wax Quail
            (DLC1Content.Items.MoveSpeedOnKill.itemIndex, 1),//Hunter's Harpoon
            (RoR2Content.Items.WardOnLevel.itemIndex, 0.33f),//Warbanner
            (RoR2Content.Items.WarCryOnMultiKill.itemIndex, 0.33f),//Berzerker's Pauldron
            (RoR2Content.Items.LunarBadLuck.itemIndex, 0.33f),//Purity
            (RoR2Content.Items.AlienHead.itemIndex, 0.33f),//Alien Head
            (RoR2Content.Items.UtilitySkillMagazine.itemIndex, 0.33f),//Hardlight Afterburner
            (DLC1Content.Items.HalfAttackSpeedHalfCooldowns.itemIndex, 0.33f),//Light Flux Pauldron
            (RoR2Content.Items.Phasing.itemIndex, 0.25f),//Old War Stealthkit
            (RoR2Content.Items.Bandolier.itemIndex, 0.25f),//Bandolier
        };

        var speedEquipements = new (EquipmentIndex Equipement, float Weigth)[]
        {
            (RoR2Content.Equipment.TeamWarCry.equipmentIndex, 0.5f),//Gorag's Opus
            (RoR2Content.Equipment.Gateway.equipmentIndex, 0.5f),//Eccentric Vase
            (RoR2Content.Equipment.Jetpack.equipmentIndex, 0.5f),//Milky Chrysalis
            (RoR2Content.Equipment.FireBallDash.equipmentIndex, 0.5f),//Volcanic Egg
            (RoR2Content.Equipment.Tonic.equipmentIndex, 0.5f)//Spinel Tonic
        };

        for (int i = 0; i < selector.choices.Length; i++)
        {
            ref var choice = ref selector.choices[i];
            var pickupDef = choice.value.pickupDef;

            var itemWeigth = speedItems
                .Where(x => x.Item == pickupDef.itemIndex)
                .Select(x => x.Weigth)
                .Concat(speedEquipements
                    .Where(x => x.Equipement == pickupDef.equipmentIndex)
                    .Select(x => x.Weigth))
                .FirstOrDefault();

            if (itemWeigth > 0)
            {
                var oldWeight = choice.weight;
                selector.ModifyChoiceWeight(i, choice.weight + choice.weight * (Configuration.Instance.SpeedItemSpawnMultiplier.Value - 1) * itemWeigth);
                Log.Debug($"{choice.value.pickupDef.nameToken} weight changed from {oldWeight} to {choice.weight}");
            }
        }
    }

    public static IDisposable CreateSelectorCopy(WeightedSelection<PickupIndex> selector, Action<WeightedSelection<PickupIndex>> setSelector)
    {
        WeightedSelection<PickupIndex> oldSelector = selector;

        setSelector(new WeightedSelection<PickupIndex>
        {
            Capacity = selector.Capacity,
            choices = selector.choices.ToArray(),
            totalWeight = selector.totalWeight,
            Count = selector.Count
        });

        return new Disposable(() =>
        {
            setSelector(oldSelector);
        });
    }
}
