using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace MusicBeePlugin
{
    public partial class SpectrogramConfig : Form
    {

        // Initialize the configMgr class.
        private ConfigMgr configMgr;
        

        public string configPath()
        {
            string path = Directory.GetCurrentDirectory() + @"\config.xml";

            return path;

        }

        public string ffmpegPath()
        {
            string path = Directory.GetCurrentDirectory() + @"\path.txt";

            return path;

        }

        public string headerPath()
        {
            string path = Directory.GetCurrentDirectory() + @"\noheader.txt";

            return path;

        }

        public string seekbarPath()
        {
            string path = Directory.GetCurrentDirectory() + @"\seekbar.txt";

            return path;

        }


        public SpectrogramConfig(string path)
        {
            InitializeComponent();
            configMgr = new ConfigMgr();

            Directory.SetCurrentDirectory(path);


            // If the config.xml file exists, it will set the default text.
            if (File.Exists(configPath()))
            {

                setDefaultText();

            }

        }

        // Reads the pre-existing config.xml file and sets the text in each field based on that data.
        private void setDefaultText()
        {

            var deserializedObject = configMgr.DeserializeConfig(configPath());

            colorSchemeBox.Text = deserializedObject.ColorScheme;
            saturationBox.Text = deserializedObject.Saturation;
            gainBox.Text = deserializedObject.Gain;
            windowFunctionBox.Text = deserializedObject.WindowFunction;
            channelModeBox.Text = deserializedObject.ChannelMode;
            scaleBox.Text = deserializedObject.Scale;
            showFrequencyBox.Checked = deserializedObject.ShowLegend;
            clearImagesBox.Checked = deserializedObject.ClearImages;
            debugModeBox.Checked = deserializedObject.EnableDebugging;

            // Sets the default text for the Ffmpeg path box if one has been set.
            if (File.Exists(ffmpegPath()))
            {

                string path = File.ReadAllText(ffmpegPath());
                pathBox.Text = path;

            }

            if (File.Exists(seekbarPath()))
            {
                enableSeekbarBox.Checked = true;
            }
            else
            {
                enableSeekbarBox.Checked = false;
            }

            if (File.Exists(headerPath()))
            {
                disableHeaderBox.Checked = true;
            }
            else
            {
                disableHeaderBox.Checked = false;
            }

        }

        // Serializes the data when the Save Button is clicked.
        private void saveButton_Click(object sender, EventArgs e)
        {

            configMgr.ColorScheme = colorSchemeBox.Text.ToString().ToLower();
            configMgr.Saturation = saturationBox.Text.ToString();
            configMgr.Gain = gainBox.Text.ToString();
            configMgr.WindowFunction = windowFunctionBox.Text.ToString().ToLower();
            configMgr.ChannelMode = channelModeBox.Text.ToString().ToLower();
            configMgr.Scale = scaleBox.Text.ToString();
            configMgr.ClearImages = clearImagesBox.Checked;
            configMgr.ShowLegend = showFrequencyBox.Checked;
            configMgr.EnableDebugging = debugModeBox.Checked;

            configMgr.Save(configPath());


            if (enableSeekbarBox.Checked)
            {

                if (!File.Exists(seekbarPath()))
                {
                    File.Create(seekbarPath());
                }

            }
            else if (!enableSeekbarBox.Checked)
            {

                if (File.Exists(seekbarPath()))
                {
                    File.Delete(seekbarPath());
                }

            }



            if (disableHeaderBox.Checked)
            {

                if (!File.Exists(headerPath()))
                {
                    File.Create(headerPath());
                }

            }
            else if (!disableHeaderBox.Checked)
            {

                if (File.Exists(headerPath()))
                {
                    File.Delete(headerPath());
                }

            }

            MessageBox.Show("Settings Saved.");



        }

        // Resets the variables in the config file to the defaults listed below.
        private void resetDefaultsButton_Click(object sender, EventArgs e)
        {

            configMgr.ColorScheme = "intensity";
            configMgr.Saturation = "1";
            configMgr.Gain = "1";
            configMgr.WindowFunction = "hann";
            configMgr.ChannelMode = "combined";
            configMgr.Scale = "log";
            configMgr.ClearImages = false;
            configMgr.ShowLegend = true;
            configMgr.EnableDebugging = false;
            
            configMgr.Save(configPath());
            setDefaultText();

            


            if (File.Exists(seekbarPath()))
            {
                File.Delete(seekbarPath());
                enableSeekbarBox.Checked = false;
            }

            if (File.Exists(headerPath()))
            {
                File.Delete(headerPath());
                disableHeaderBox.Checked = false;
            }

            MessageBox.Show("Settings reset to defaults.");

        }

        // If someone has entered something into the Ffmpeg path box, this button saves it.
        private void setPathButton_Click(object sender, EventArgs e)
        {

            if (pathBox.Text.ToString() != "")
            {

                using (StreamWriter sw = File.CreateText(ffmpegPath()))
                {
                    sw.WriteLine(pathBox.Text.ToString());
                    MessageBox.Show("Ffmpeg path set.");
                }

            }
            else
            {

                MessageBox.Show("No path entered.");

            }

        }

        // If a path exists, this button clears it.
        private void clearPathButton_Click(object sender, EventArgs e)
        {


            if (File.Exists(ffmpegPath()))
            {

                File.Delete(ffmpegPath());
                setDefaultText();
                MessageBox.Show("Path deleted.");

            }
            else
            {

                MessageBox.Show("No path currently exists.");

            }


        }
        

        private void enableSeekbarBox_CheckedChanged(object sender, EventArgs e)
        {

            
        }

        private void disableHeaderBox_CheckedChanged(object sender, EventArgs e)
        {
            
            
        }
    }
}
