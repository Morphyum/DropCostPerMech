﻿using BattleTech;
using BattleTech.Framework;
using BattleTech.Save;
using BattleTech.Save.SaveGameStructure;
using BattleTech.UI;
using BattleTech.UI.TMProWrapper;
using Harmony;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace DropCostPerMech {
    [HarmonyPatch(typeof(GameInstanceSave), MethodType.Constructor)]
    [HarmonyPatch(new Type[] { typeof(GameInstance), typeof(SaveReason) })]
    public static class GameInstanceSave_Constructor_Patch {
        static void Postfix(GameInstanceSave __instance) {
            Helper.SaveState(__instance.InstanceGUID, __instance.SaveTime);
        }
    }

    [HarmonyPatch(typeof(GameInstance), "Load")]
    public static class GameInstance_Load_Patch {
        static void Prefix(GameInstanceSave save) {
            Helper.LoadState(save.InstanceGUID, save.SaveTime);
        }
    }

    [HarmonyPatch(typeof(AAR_ContractObjectivesWidget), "FillInObjectives")]
    public static class AAR_ContractObjectivesWidget_FillInObjectives {

        static void Postfix(AAR_ContractObjectivesWidget __instance) {
            try {
                Settings settings = Helper.LoadSettings();

                string missionObjectiveResultString = $"DROP COSTS DEDUCTED: ¢{Fields.FormattedDropCost}";
                if (settings.CostByTons) {
                    missionObjectiveResultString += $"{Environment.NewLine}AFTER {settings.freeTonnageAmount} TON CREDIT WORTH ¢{string.Format("{0:n0}", settings.freeTonnageAmount * settings.cbillsPerTon)}";
                }
                MissionObjectiveResult missionObjectiveResult = new MissionObjectiveResult(missionObjectiveResultString, "7facf07a-626d-4a3b-a1ec-b29a35ff1ac0", false, true, ObjectiveStatus.Succeeded, false);
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
                int newMoneyResults = Mathf.FloorToInt(__instance.MoneyResults - Fields.DropCost);
                ReflectionHelper.InvokePrivateMethode(__instance, "set_MoneyResults", new object[] { newMoneyResults });
            }
            catch (Exception e) {
                Logger.LogError(e);
            }
        }
    }

    [HarmonyPatch(typeof(LanceHeaderWidget), "RefreshLanceInfo")]
    public static class LanceHeaderWidget_RefreshLanceInfo {
        static void Postfix(LanceHeaderWidget __instance, List<MechDef> mechs, LocalizableText ___simLanceTonnageText, LanceConfiguratorPanel ___LC) {
            try {
                Settings settings = Helper.LoadSettings();
                string freeTonnageText = "";

               
                if (___LC.IsSimGame) {
                    int lanceTonnage = 0;
                    float dropCost = 0f;
                    if (settings.CostByTons) {
                        foreach (MechDef def in mechs) {
                            dropCost += (def.Chassis.Tonnage * settings.cbillsPerTon);
                            lanceTonnage += (int)def.Chassis.Tonnage;
                        }
                    }
                    else {
                        foreach (MechDef def in mechs) {
                            dropCost += (Helper.CalculateCBillValue(def) * settings.percentageOfMechCost);
                            lanceTonnage += (int)def.Chassis.Tonnage;
                        }
                    }
                    if (settings.CostByTons && settings.someFreeTonnage) {
                        freeTonnageText = $"({settings.freeTonnageAmount} TONS FREE)";
                        dropCost = Math.Max(0f, (lanceTonnage - settings.freeTonnageAmount) * settings.cbillsPerTon);
                    }

                    string formattedDropCost = string.Format("{0:n0}", dropCost);
                    Fields.DropCost = dropCost;
                    Fields.LanceTonnage = lanceTonnage;
                    Fields.FormattedDropCost = formattedDropCost;
                    Fields.FreeTonnageText = freeTonnageText;
                    
                    // longer strings interfere with messages about incorrect lance configurations
                    ___simLanceTonnageText.SetText($"DROP COST: ¢{Fields.FormattedDropCost}   LANCE WEIGHT: {Fields.LanceTonnage} TONS {Fields.FreeTonnageText}");
                }
            }
            catch (Exception e) {
                Logger.LogError(e);
            }
        }
    }
}