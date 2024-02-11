using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RechargeRedux
{
    [BepInPlugin(modGUID, modName, modVersion)]
    public class RechargeRedux : BaseUnityPlugin
    {
        // Config
        public static bool RechargePaint { get; private set; }
        public static bool RechargeTZP { get; private set; }

        private const string modGUID = "achohbee.RechargeRedux";
        private const string modName = "RechargeRedux";
        private const string modVersion = "1.0.0";

        internal static ManualLogSource log = BepInEx.Logging.Logger.CreateLogSource(modName);

        private void InitConfig()
        {
            var paintConf = Config.Bind<Boolean>("General", "Rechargable Spray Paint", true);
            log.LogInfo("Setting RechargePaint to " + paintConf.Value);
            RechargePaint = paintConf.Value;
            paintConf.SettingChanged += (object sender, EventArgs e) => {
                log.LogInfo("Config refresh. Setting RechagePaint to " + paintConf.Value);
                RechargePaint = paintConf.Value; 
            };

            var tzpConf = Config.Bind<Boolean>("General", "Rechargable TZP-Inhalant", true);
            log.LogInfo("Setting RechargeTZP to " + tzpConf.Value);
            RechargeTZP = tzpConf.Value;
            tzpConf.SettingChanged += (object sender, EventArgs e) => {
                log.LogInfo("Config refresh. Setting RechargeTZP to " + tzpConf.Value);
                RechargeTZP = tzpConf.Value;
            };
        }

        void Awake()
        {
            InitConfig();

            log.LogInfo("Applying all patches");
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), modGUID);
        }
    }

}
