using Newtonsoft.Json;
using System;
using System.IO;

namespace DropCostPerMech {
    public class SaveFields {
        public float DropCost = 0;
        public int LanceTonnage = 0;
        public string FormattedDropCost;
        public string FreeTonnageText;

        public SaveFields(float dropCost, int lanceTonnage, string formattedDropCost, string freeTonnageText) {
            DropCost = dropCost;
            LanceTonnage = lanceTonnage;
            FormattedDropCost = formattedDropCost;
            FreeTonnageText = freeTonnageText;
        }
    }

    public class Helper {

        public static Settings LoadSettings() {
            try {
                using (StreamReader r = new StreamReader($"{DropCostPerMech.ModDirectory}/settings.json")) {
                    string json = r.ReadToEnd();
                    return JsonConvert.DeserializeObject<Settings>(json);
                }
            }
            catch (Exception ex) {
                Logger.LogError(ex);
                return null;
            }
        }

        public static void SaveState(string instanceGUID, DateTime saveTime) {
            try {
                int unixTimestamp = (int)(saveTime.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
                string filePath = $"{DropCostPerMech.ModDirectory}/saves/" + instanceGUID + "-" + unixTimestamp + ".json";
                (new FileInfo(filePath)).Directory.Create();
                using (StreamWriter writer = new StreamWriter(filePath, true)) {
                    SaveFields fields = new SaveFields(Fields.DropCost, Fields.LanceTonnage, Fields.FormattedDropCost, Fields.FreeTonnageText);
                    string json = JsonConvert.SerializeObject(fields);
                    writer.Write(json);
                }
            }
            catch (Exception ex) {
                Logger.LogError(ex);
            }
        }

        public static void LoadState(string instanceGUID, DateTime saveTime) {
            try {
                int unixTimestamp = (int)(saveTime.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
                string filePath = $"{DropCostPerMech.ModDirectory}/saves/" + instanceGUID + "-" + unixTimestamp + ".json";
                if (File.Exists(filePath)) {
                    using (StreamReader r = new StreamReader(filePath)) {
                        string json = r.ReadToEnd();
                        SaveFields save = JsonConvert.DeserializeObject<SaveFields>(json);
                        Fields.DropCost = save.DropCost;
                        Fields.LanceTonnage = save.LanceTonnage;
                        Fields.FormattedDropCost = save.FormattedDropCost;
                        Fields.FreeTonnageText = save.FreeTonnageText;

                    }
                }
            }
            catch (Exception ex) {
                Logger.LogError(ex);
            }
        }
    }
}