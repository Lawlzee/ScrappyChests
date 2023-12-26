using RoR2;
using RoR2.Artifacts;
using System;
using System.Collections.Generic;
using System.Text;

namespace ScrappyChests;

public static class ArtifactHooks
{
    public static void Init()
    {
        On.RoR2.DoppelgangerDropTable.GenerateDropPreReplacement += DoppelgangerDropTable_GenerateDropPreReplacement;
        On.RoR2.Artifacts.SacrificeArtifactManager.OnServerCharacterDeath += SacrificeArtifactManager_OnServerCharacterDeath;
    }

    private static PickupIndex DoppelgangerDropTable_GenerateDropPreReplacement(On.RoR2.DoppelgangerDropTable.orig_GenerateDropPreReplacement orig, DoppelgangerDropTable self, Xoroshiro128Plus rng)
    {
        if (Configuration.Instance.ModEnabled.Value && Configuration.Instance.ReplaceDoppelgangerDropTable.Value)
        {
            using var _ = self.ReplaceDropTable(nameof(DoppelgangerDropTable_GenerateDropPreReplacement));
            return orig(self, rng);
        }

        return orig(self, rng);
    }

    private static void SacrificeArtifactManager_OnServerCharacterDeath(On.RoR2.Artifacts.SacrificeArtifactManager.orig_OnServerCharacterDeath orig, DamageReport damageReport)
    {
        if (Configuration.Instance.ModEnabled.Value && Configuration.Instance.ReplaceSacrificeArtifactDropTable.Value)
        {
            using var _ = SacrificeArtifactManager.dropTable.ReplaceDropTable(nameof(SacrificeArtifactManager_OnServerCharacterDeath));
            orig(damageReport);
            return;
        }

        orig(damageReport);
    }
}
