using RoR2;
using System;
using UnityEngine;

namespace ScrappyChests;

public static class CostHooks
{
    public static void Init()
    {
        On.RoR2.PurchaseInteraction.GetContextString += PurchaseInteraction_GetContextString;
        On.RoR2.PurchaseInteraction.CanBeAffordedByInteractor += PurchaseInteraction_CanBeAffordedByInteractor;
        On.RoR2.PurchaseInteraction.OnInteractionBegin += PurchaseInteraction_OnInteractionBegin; ;
        On.RoR2.PurchaseInteraction.UpdateHologramContent += PurchaseInteraction_UpdateHologramContent;
        On.RoR2.UI.PingIndicator.RebuildPing += PingIndicator_RebuildPing;
    }

    private static string PurchaseInteraction_GetContextString(On.RoR2.PurchaseInteraction.orig_GetContextString orig, PurchaseInteraction self, Interactor activator)
    {
        return ReplacePurchaseInteraction(self, () => orig(self, activator));
    }
    private static bool PurchaseInteraction_CanBeAffordedByInteractor(On.RoR2.PurchaseInteraction.orig_CanBeAffordedByInteractor orig, PurchaseInteraction self, Interactor activator)
    {
        return ReplacePurchaseInteraction(self, () => orig(self, activator));
    }

    private static void PurchaseInteraction_OnInteractionBegin(On.RoR2.PurchaseInteraction.orig_OnInteractionBegin orig, PurchaseInteraction self, Interactor activator)
    {
        ReplacePurchaseInteraction(self, () => { orig(self, activator); return true; });
    }

    private static void PurchaseInteraction_UpdateHologramContent(On.RoR2.PurchaseInteraction.orig_UpdateHologramContent orig, PurchaseInteraction self, GameObject hologramContentObject)
    {
        ReplacePurchaseInteraction(self, () => { orig(self, hologramContentObject); return true; });
    }

    private static void PingIndicator_RebuildPing(On.RoR2.UI.PingIndicator.orig_RebuildPing orig, RoR2.UI.PingIndicator self)
    {
        if (self.pingTarget)
        {
            PurchaseInteraction purchaseInteraction = self.pingTarget.GetComponent<PurchaseInteraction>();
            if (purchaseInteraction)
            {
                ReplacePurchaseInteraction(purchaseInteraction, () => { orig(self); return true; });
                return;
            }
        }

        orig(self);
    }

    private static T ReplacePurchaseInteraction<T>(PurchaseInteraction self, Func<T> orig)
    {
        //LUNAR_CHEST_NAME: Lunar Pod
        //LUNAR_REROLL_NAME: Slab
        //LUNAR_TERMINAL_NAME: Lunar Bud
        //LOCKEDMAGE_NAME: Free the survivor (artificer unlock)
        //FROG_NAME: Frog
        //SHRINE_RESTACK_NAME: Shrine of Order
        //BAZAAR_BLUEPRINT_NAME: Junk
        if (Configuration.Instance.ModEnabled.Value
            && ((Configuration.Instance.ReplaceNewtAltarsCost.Value && self.displayNameToken == "NEWT_STATUE_NAME")
                || (Configuration.Instance.ReplaceLunarSeerCost.Value && self.displayNameToken == "BAZAAR_SEER_NAME")
                || (Configuration.Instance.ReplaceLunarPodCost.Value && self.displayNameToken == "LUNAR_CHEST_NAME")
                || (Configuration.Instance.ReplaceSlabCost.Value && self.displayNameToken == "LUNAR_REROLL_NAME")
                || (Configuration.Instance.ReplaceLunarBudCost.Value && self.displayNameToken == "LUNAR_TERMINAL_NAME")
                || (Configuration.Instance.ReplaceMageCost.Value && self.displayNameToken == "LOCKEDMAGE_NAME")
                || (Configuration.Instance.ReplaceFrogCost.Value && self.displayNameToken == "FROG_NAME")
                || (Configuration.Instance.ReplaceShrineOfOrderCost.Value && self.displayNameToken == "SHRINE_RESTACK_NAME")))
        {
            var oldCostType = self.costType;
            using var disposable = new Disposable(() => self.costType = oldCostType);
            self.costType = CostTypeIndex.WhiteItem;
            return orig();
        }

        return orig();
    }
}
