﻿using BattleTech;
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
                string freeTonnage = "";
                LanceConfiguratorPanel LC = (LanceConfiguratorPanel)ReflectionHelper.GetPrivateField(__instance, "LC");
                if (LC.IsSimGame) {
                    int lanceTonnage = 0;
                    float dropCost = 0f;
                    if (settings.CostByTons) {
                        foreach (MechDef def in mechs) {
                            dropCost += (def.Chassis.Tonnage * settings.cbillsPerTon);
                            lanceTonnage += (int)def.Chassis.Tonnage;
                            Logger.LogCompactLine($"CostByTons - dropCost: {dropCost} lanceTonnage:{lanceTonnage}");
                        }
                    } else {
                        foreach (MechDef def in mechs) {
                            dropCost += (def.Description.Cost * settings.percentageOfMechCost);
                            lanceTonnage += (int)def.Chassis.Tonnage;
                            Logger.LogCompactLine($"CostByPrice - dropCost: {dropCost} lanceTonnage:{lanceTonnage}");
                        }
                    }
                    
                    TextMeshProUGUI simLanceTonnageText = (TextMeshProUGUI)ReflectionHelper.GetPrivateField(__instance, "simLanceTonnageText");

                    if (settings.CostByTons && settings.someFreeTonnage)
                    {
                        freeTonnage = $" {freeTonnage} FREE TONS";
                        dropCost = Math.Max(0f, (lanceTonnage - (settings.freeTonnageAmount * settings.cbillsPerTon)));
                    }
                    simLanceTonnageText.text = string.Format($"OPERATION COSTS: {(int)dropCost} ¢ / LANCE WEIGHT: {lanceTonnage} TONS{freeTonnage}");
                }
            }
            catch (Exception e) {
                Logger.LogError(e);
            }
        }
    }
}