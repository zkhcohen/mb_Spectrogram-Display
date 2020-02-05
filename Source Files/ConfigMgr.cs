using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace MusicBeePlugin
{
    [Serializable()]
    public class ConfigMgr
    {
        #region Properties

        public String ChannelMode { get; set; }

        public Boolean ClearImages { get; set; }

        public String ColorScheme { get; set; }

        public Boolean EnableDebugging { get; set; }

        public String FreqStart { get; set; }

        public String FreqStop { get; set; }

        public String Gain { get; set; }

        public String Saturation { get; set; }

        public String Scale { get; set; }

        public Boolean ShowLegend { get; set; }

        public String WindowFunction { get; set; }

        #endregion

        #region Methods

        public static void SerializeConfig(ConfigMgr data, string path)
        {

            using (StreamWriter file = new StreamWriter(path, false))
            {
                XmlSerializer controlsDefaultsSerializer = new XmlSerializer(typeof(ConfigMgr));
                controlsDefaultsSerializer.Serialize(file, data);
                file.Close();
            }
        }

        public ConfigMgr DeserializeConfig(string path)
        {

            try
            {
                StreamReader file = new StreamReader(path);
                XmlSerializer xSerial = new XmlSerializer(typeof(ConfigMgr));
                object oData = xSerial.Deserialize(file);
                var thisConfig = (ConfigMgr)oData;
                file.Close();
                return thisConfig;
            }
            catch (Exception e)
            {

                Console.Write(e.Message);
                return null;
            }
        }

        public bool Save(string path)
        {
            ConfigMgr.SerializeConfig(this, path);
            return true;
        }

        #endregion
    }
}
