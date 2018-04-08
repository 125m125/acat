using ACAT.Lib.Core.PreferencesManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ACAT.Extensions.Default.Actuators.SerialActuator
{
    [Serializable]
    class Settings: PreferencesBase
    {
        /// <summary>
        /// Name of the settings file
        /// </summary>
        [NonSerialized, XmlIgnore]
        public static String SettingsFilePath;

        public String PortName;

        /// Add additional properties here

        /// <summary>
        /// Loads the settings from the settings file
        /// </summary>
        /// <returns>true on success</returns>
        public static Settings Load()
        {
            Settings retVal = PreferencesBase.Load<Settings>(SettingsFilePath);
            Save(retVal, SettingsFilePath);
            return retVal;
        }

        /// <summary>
        /// Saves settings
        /// </summary>
        /// <returns>true on success</returns>
        public override bool Save()
        {
            return Save<Settings>(this, SettingsFilePath);
        }
    }
}
