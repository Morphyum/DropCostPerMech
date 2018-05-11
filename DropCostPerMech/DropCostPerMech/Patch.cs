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
                MissionObjectiveResult missionObjectiveResult = new MissionObjectiveResult("Drop Cost: "+ Mathf.FloorToInt(LanceHeaderWidget_RefreshLanceInfo.cbill) + " C-Bills", "7facf07a-626d-4a3b-a1ec-b29a35ff1ac0", false, true, ObjectiveStatus.Succeeded, false);
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
                int newMoneyResults = Mathf.FloorToInt(__instance.MoneyResults - LanceHeaderWidget_RefreshLanceInfo.cbill);
                ReflectionHelper.InvokePrivateMethode(__instance, "set_MoneyResults", new object[] { newMoneyResults });
            }
            catch (Exception e) {
                Logger.LogError(e);
            }
        }
    }

    [HarmonyPatch(typeof(LanceHeaderWidget), "RefreshLanceInfo")]
    public static class LanceHeaderWidget_RefreshLanceInfo {
        public static float cbill;
        static void Postfix(LanceHeaderWidget __instance, List<MechDef> mechs) {
            try {
                LanceConfiguratorPanel LC = (LanceConfiguratorPanel)ReflectionHelper.GetPrivateField(__instance, "LC");
                if (LC.IsSimGame) {
                    float num2 = 0f;
                    int lanceTonnageRating = SimGameBattleSimulator.GetLanceTonnageRating(LC.sim, mechs, out num2);
                    cbill = 0f;
                    foreach (MechDef def in mechs) {
                        cbill += (float)def.Description.Cost * 0.001f;
                    }
                    TextMeshProUGUI simLanceTonnageText = (TextMeshProUGUI)ReflectionHelper.GetPrivateField(__instance, "simLanceTonnageText");
                    simLanceTonnageText.text = string.Format("{0} C-Bills, {1} TONS", (int)cbill, (int)num2);
                }
            }
            catch (Exception e) {
                Logger.LogError(e);
            }
        }
    }
}