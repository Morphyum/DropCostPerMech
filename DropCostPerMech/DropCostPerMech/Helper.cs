using Newtonsoft.Json;
using System;
using System.IO;

namespace DropCostPerMech {
    public class SaveFields {
        public float cbill = 0;

        public SaveFields(float cbill) {
            this.cbill = cbill;
        }
    }

    public class Helper {

        public static Settings LoadSettings() {
            try {
                using (StreamReader r = new StreamReader("mods/DropCostPerMech/settings.json")) {
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
                string filePath = "mods/DropCostPerMech/saves/" + instanceGUID + "-" + unixTimestamp + ".json";
                (new FileInfo(filePath)).Directory.Create();
                using (StreamWriter writer = new StreamWriter(filePath, true)) {
                    SaveFields fields = new SaveFields(Fields.cbill);
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
                string filePath = "mods/DropCostPerMech/saves/" + instanceGUID + "-" + unixTimestamp + ".json";
                if (File.Exists(filePath)) {
                    using (StreamReader r = new StreamReader(filePath)) {
                        string json = r.ReadToEnd();
                        SaveFields save = JsonConvert.DeserializeObject<SaveFields>(json);
                        Fields.cbill = save.cbill;
                    }
                }
            }
            catch (Exception ex) {
                Logger.LogError(ex);
            }
        }
    }
}