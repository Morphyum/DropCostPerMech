using Harmony;
using System.Reflection;

namespace DropCostPerMech
{
    public class DropCostPerMech
    {
        public static void Init() {
            var harmony = HarmonyInstance.Create("de.morphyum.DropCostPerMech");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
    }
}
