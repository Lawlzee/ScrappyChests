using RoR2;

namespace ScrappyChests;

public static class WaveHooks
{
    public static void Init()
    {
        On.RoR2.InfiniteTowerWaveController.DropRewards += InfiniteTowerWaveController_DropRewards;
        On.RoR2.ArenaMonsterItemDropTable.GenerateUniqueDropsPreReplacement += ArenaMonsterItemDropTable_GenerateUniqueDropsPreReplacement;
        On.RoR2.ArenaMissionController.EndRound += ArenaMissionController_EndRound;
    }

    private static void InfiniteTowerWaveController_DropRewards(On.RoR2.InfiniteTowerWaveController.orig_DropRewards orig, InfiniteTowerWaveController self)
    {
        if (Configuration.Instance.ModEnabled.Value && Configuration.Instance.ReplaceSimulacrumOrbDropTable.Value)
        {
            using var _ = self.rewardDropTable.ReplaceDropTable(nameof(InfiniteTowerWaveController_DropRewards));
            orig(self);
            return;
        }

        orig(self);
    }

    private static PickupIndex[] ArenaMonsterItemDropTable_GenerateUniqueDropsPreReplacement(On.RoR2.ArenaMonsterItemDropTable.orig_GenerateUniqueDropsPreReplacement orig, ArenaMonsterItemDropTable self, int maxDrops, Xoroshiro128Plus rng)
    {
        if (Configuration.Instance.ModEnabled.Value && Configuration.Instance.ReplaceSimulacrumOrbDropTable.Value)
        {
            using var _ = self.ReplaceDropTable(nameof(ArenaMonsterItemDropTable_GenerateUniqueDropsPreReplacement));
            return orig(self, maxDrops, rng);
        }

        return orig(self, maxDrops, rng);
    }

    private static void ArenaMissionController_EndRound(On.RoR2.ArenaMissionController.orig_EndRound orig, ArenaMissionController self)
    {
        if (Configuration.Instance.ModEnabled.Value && Configuration.Instance.ReplaceVoidFieldsOrbDropTable.Value)
        {
            using var disposables = new CompositeDisposable();

            foreach (var rewardOrder in self.playerRewardOrder)
            {
                disposables.Add(rewardOrder.ReplaceDropTable(nameof(ArenaMissionController_EndRound)));
            }

            orig(self);
            return;
        }

        orig(self);
    }
}
