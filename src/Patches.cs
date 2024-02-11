using HarmonyLib;

namespace RechargeRedux
{
    [HarmonyPatch]
    internal class Patches
    {
        // Access to the spray can tank is tricky since it is a private variable
        private static AccessTools.FieldRef<SprayPaintItem, float> sprayPaintTank = 
            AccessTools.FieldRefAccess<SprayPaintItem, float>("sprayCanTank");

        private static AccessTools.FieldRef<TetraChemicalItem, float> tetraChemicalFuel = 
            AccessTools.FieldRefAccess<TetraChemicalItem, float>("fuel");

        [HarmonyPatch(typeof(GrabbableObject), "ChargeBatteries")]
        [HarmonyPrefix]
        private static void ChargeBatteries(GrabbableObject __instance)
        {
            if (RechargeRedux.RechargePaint && __instance is SprayPaintItem)
            {
                SprayPaintItem paintInstance = (SprayPaintItem) __instance;
                sprayPaintTank(paintInstance) = __instance.insertedBattery.charge;
            } else if (RechargeRedux.RechargeTZP && __instance is TetraChemicalItem)
            {
                TetraChemicalItem tetraInstance = (TetraChemicalItem)__instance;
                tetraChemicalFuel(tetraInstance) = __instance.insertedBattery.charge;
            }
        }

        [HarmonyPatch(typeof(GrabbableObject), "Start")]
        [HarmonyPostfix]
        private static void Start(GrabbableObject __instance)
        {
            if (RechargeRedux.RechargePaint && __instance is SprayPaintItem)
            {
                __instance.itemProperties.requiresBattery = true;
                __instance.insertedBattery = new Battery(false, sprayPaintTank((SprayPaintItem) __instance));
            } else if (RechargeRedux.RechargeTZP &&__instance is TetraChemicalItem)
            {
                TetraChemicalItem tetraInstance = (TetraChemicalItem)__instance;
                __instance.itemProperties.requiresBattery = true;
                __instance.insertedBattery = new Battery(false, tetraChemicalFuel((TetraChemicalItem)__instance));
            }
        }

        [HarmonyPatch(typeof(SprayPaintItem), "LateUpdate")]
        [HarmonyPostfix]
        private static void SprayPaintLateUpdate(SprayPaintItem __instance)
        {
            if (RechargeRedux.RechargePaint)
            {
                if (!__instance.itemProperties.requiresBattery)
                {
                    Start(__instance);
                }

                __instance.insertedBattery.charge = sprayPaintTank(__instance);
            }
            else if (__instance.itemProperties.requiresBattery)
            {
                __instance.itemProperties.requiresBattery = false;
            }
        }


        [HarmonyPatch(typeof(TetraChemicalItem), "Update")]
        [HarmonyPostfix]
        private static void TetraChemicalUpdate(TetraChemicalItem __instance)
        {
            if (RechargeRedux.RechargeTZP)
            {
                if (!__instance.itemProperties.requiresBattery)
                {
                    Start(__instance);
                }

                __instance.insertedBattery.charge = tetraChemicalFuel(__instance);
            }
            else if (__instance.itemProperties.requiresBattery)
            {
                __instance.itemProperties.requiresBattery = false;
            }
        }
    }
}
