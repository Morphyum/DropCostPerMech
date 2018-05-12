using BattleTech;
using BattleTech.Framework;
using BattleTech.UI;
using Harmony;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace DropCostPerMech {

    [HarmonyPatch(typeof(AAR_ContractObjectivesWidget), "FillInObjectives")]
    public static class AAR_ContractObjectivesWidget_FillInObjectives {

        static void Postfix(AAR_ContractObjectivesWidget __instance) {
            try {
                MissionObjectiveResult missionObjectiveResult = new MissionObjectiveResult("Operation Costs: " + Mathf.FloorToInt(Fields.cbill) + " ¢", "7facf07a-626d-4a3b-a1ec-b29a35ff1ac0", false, true, ObjectiveStatus.Succeeded, false);
                ReflectionHelper.InvokePrivateMethode(__instance, "AddObjective", new object[] { missionObjectiveResult });
            }
            catch (Exception e) {
                Logger.LogError(e);
            }
        }
    }

    [HarmonyPatch(typeof(Contract), "CompleteContract")]
    public static class Contract_CompleteContract {

        static void Postfix(Contract __instance) {
            try {
                int newMoneyResults = Mathf.FloorToInt(__instance.MoneyResults - Fields.cbill);
                ReflectionHelper.InvokePrivateMethode(__instance, "set_MoneyResults", new object[] { newMoneyResults });
            }
            catch (Exception e) {
                Logger.LogError(e);
            }
        }
    }

    [HarmonyPatch(typeof(LanceHeaderWidget), "RefreshLanceInfo")]
    public static class LanceHeaderWidget_RefreshLanceInfo {    
        static void Postfix(LanceHeaderWidget __instance, List<MechDef> mechs) {
            try {
                Settings settings = Helper.LoadSettings();

                LanceConfiguratorPanel LC = (LanceConfiguratorPanel)ReflectionHelper.GetPrivateField(__instance, "LC");
                if (LC.IsSimGame) {
                    float num2 = 0f;
                    int lanceTonnageRating = SimGameBattleSimulator.GetLanceTonnageRating(LC.sim, mechs, out num2);
                    Fields.cbill = 0f;
                    if (settings.CostByTons) {
                        foreach (MechDef def in mechs) {
                            Fields.cbill += (float)def.Chassis.Tonnage * settings.cbillsPerTon;
                        }
                    } else {
                        foreach (MechDef def in mechs) {
                            Fields.cbill += (float)def.Description.Cost * settings.percentageOfMechCost;
                        }
                    }
                    
                    TextMeshProUGUI simLanceTonnageText = (TextMeshProUGUI)ReflectionHelper.GetPrivateField(__instance, "simLanceTonnageText");
                    simLanceTonnageText.text = string.Format("Operation Costs: {0} ¢ / Lance Weight: {1} TONS", (int)Fields.cbill, (int)num2);
                }
            }
            catch (Exception e) {
                Logger.LogError(e);
            }
        }
    }
}