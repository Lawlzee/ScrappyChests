using EntityStates.ScavBackpack;
using RoR2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace ScrappyChests;

public static class MobHooks
{
    public static void Init()
    {
        On.RoR2.EquipmentSlot.FireBossHunter += EquipmentSlot_FireBossHunter;

        On.RoR2.BossGroup.DropRewards += BossGroup_DropRewards;
        On.EntityStates.ScavBackpack.Opening.FixedUpdate += Opening_FixedUpdate;
        On.RoR2.MasterDropDroplet.DropItems += MasterDropDroplet_DropItems;
        On.RoR2.ChestBehavior.RollItem += ChestBehavior_RollItem;

        On.RoR2.PlayerCharacterMasterController.Init += PlayerCharacterMasterController_Init;
    }

    private static void BossGroup_DropRewards(On.RoR2.BossGroup.orig_DropRewards orig, BossGroup self)
    {
        if (Configuration.Instance.ModEnabled.Value)
        {
            if (self.bossDropChance == 0 && self.dropTable.name == "dtTier3Item" && self.bossDropTables.Count == 1 && self.bossDropTables[0].name == "dtBossRoboBallBoss")
            {
                if (Configuration.Instance.ReplaceAWUDropTable.Value)
                {
                    Replace();
                    return;
                }
            }
            else if (Configuration.Instance.ReplaceBossDropTable.Value)
            {
                Replace();
                return;
            }
        }

        orig(self);

        void Replace()
        {
            using var disposables = new CompositeDisposable();
            if (self.dropTable)
            {
                disposables.Add(self.dropTable.ReplaceDropTable(nameof(BossGroup_DropRewards)));
            }
            else
            {
                Log.Info($"dropTable is {self.dropTable?.GetType().Name ?? "null"}");
            }

            foreach (PickupDropTable dropTable in self.bossDropTables.Distinct())
            {
                disposables.Add(dropTable.ReplaceDropTable(nameof(BossGroup_DropRewards)));
            }

            orig(self);
        }
    }

    private static void MasterDropDroplet_DropItems(On.RoR2.MasterDropDroplet.orig_DropItems orig, MasterDropDroplet self)
    {
        if (Configuration.Instance.ModEnabled.Value && Configuration.Instance.ReplaceElderLemurianDropTable.Value)
        {
            using CompositeDisposable disposable = new CompositeDisposable();
            foreach (var dropTable in self.dropTables)
            {
                disposable.Add(dropTable.ReplaceDropTable(nameof(MasterDropDroplet_DropItems)));
            }
            orig(self);
            return;
        }

        orig(self);
    }

    private static void Opening_FixedUpdate(On.EntityStates.ScavBackpack.Opening.orig_FixedUpdate orig, Opening self)
    {
        if (Configuration.Instance.ModEnabled.Value && Configuration.Instance.ReplaceScavengerDropTable.Value)
        {
            var chestBehavior = self.GetComponent<ChestBehavior>();
            var dropTable = (BasicPickupDropTable)chestBehavior.dropTable;

            using var _ = DropTableHelpers.CreateSelectorCopy(dropTable.selector, x => dropTable.selector = x);

            dropTable.selector.Clear();

            List<PickupIndex> lunarCoin = new List<PickupIndex>
            {
                PickupCatalog.FindPickupIndex(RoR2Content.MiscPickups.LunarCoin.miscPickupIndex)
            };

            dropTable.Add(Run.instance.availableTier1DropList, chestBehavior.tier1Chance / Run.instance.availableTier1DropList.Count);
            dropTable.Add(Run.instance.availableTier2DropList, chestBehavior.tier2Chance / Run.instance.availableTier2DropList.Count);
            dropTable.Add(Run.instance.availableTier3DropList, chestBehavior.tier3Chance / Run.instance.availableTier3DropList.Count);
            dropTable.Add(Run.instance.availableLunarCombinedDropList, chestBehavior.lunarChance / Run.instance.availableLunarCombinedDropList.Count);
            dropTable.Add(lunarCoin, chestBehavior.lunarCoinChance);

            using var __ = dropTable.ReplaceDropTable(nameof(Opening_FixedUpdate));
            orig(self);
            return;
        }

        orig(self);
    }

    private static void ChestBehavior_RollItem(On.RoR2.ChestBehavior.orig_RollItem orig, ChestBehavior self)
    {
        if (Configuration.Instance.ModEnabled.Value && Configuration.Instance.ReplaceScavengerDropTable.Value)
        {
            self.Roll();
            return;
        }
        orig(self);
    }

    private static bool EquipmentSlot_FireBossHunter(On.RoR2.EquipmentSlot.orig_FireBossHunter orig, EquipmentSlot self)
    {
        if (Configuration.Instance.ModEnabled.Value && Configuration.Instance.ReplaceBossHunterDropTable.Value)
        {
            self.UpdateTargets(DLC1Content.Equipment.BossHunter.equipmentIndex, true);
            HurtBox hurtBox = self.currentTarget.hurtBox;
            DeathRewards deathRewards = hurtBox?.healthComponent?.body?.gameObject?.GetComponent<DeathRewards>();
            if (!(bool)hurtBox || !(bool)deathRewards)
                return false;

            using var _ = deathRewards.bossDropTable.ReplaceDropTable(nameof(EquipmentSlot_FireBossHunter));
            return orig(self);
        }

        return orig(self);
    }

    private static void PlayerCharacterMasterController_Init(On.RoR2.PlayerCharacterMasterController.orig_Init orig)
    {
        //Copy of the code
        GlobalEventManager.onCharacterDeathGlobal += damageReport =>
        {
            CharacterMaster characterMaster = damageReport.attackerMaster;
            if (!characterMaster)
            {
                return;
            }

            if (characterMaster.minionOwnership.ownerMaster)
            {
                characterMaster = characterMaster.minionOwnership.ownerMaster;
            }

            PlayerCharacterMasterController component = characterMaster.GetComponent<PlayerCharacterMasterController>();
            if (!component || !Util.CheckRoll(1f * component.lunarCoinChanceMultiplier))
            {
                return;
            }

            PickupIndex pickupIndex = Configuration.Instance.ModEnabled.Value && Configuration.Instance.ReplaceLunarCoinDrops.Value
                ? PickupCatalog.FindPickupIndex(RoR2Content.Items.ScrapWhite.itemIndex)
                : PickupCatalog.FindPickupIndex(RoR2Content.MiscPickups.LunarCoin.miscPickupIndex);

            PickupDropletController.CreatePickupDroplet(pickupIndex, damageReport.victim.transform.position, Vector3.up * 10f);
            component.lunarCoinChanceMultiplier *= 0.5f;
        };
    }
}
