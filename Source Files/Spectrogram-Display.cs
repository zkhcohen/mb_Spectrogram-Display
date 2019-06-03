﻿using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Security.Cryptography;
using System.Collections.Generic;
using System.Timers;

namespace MusicBeePlugin
{
    
    public partial class Plugin
    {

        private readonly PluginInfo about = new PluginInfo();


        // Declarations
        private MusicBeeApiInterface mbApiInterface;
        private Control panel;
        public int panelHeight;
        private System.Timers.Timer timer;
        private int _seekMin = 0;
        private bool _seekbar = false;
        private float _lastPos = 0;


        ToolTip toolTip1 = new ToolTip();


        // Initialization
        public PluginInfo Initialise(IntPtr apiInterfacePtr)
        {
            
            mbApiInterface = new MusicBeeApiInterface();
            mbApiInterface.Initialise(apiInterfacePtr);

            string wdTemp = mbApiInterface.Setting_GetPersistentStoragePath() + @"Dependencies\";

            // Debugging for the dependencies.
            if (!Directory.Exists(wdTemp))
            {
                MessageBox.Show("Please copy the dependency folder here: \n\n" + wdTemp +
                    "\n\n" + "NOTE: You MAY have to re-enable the add-in through Edit Preferences, AND remove then re-add it to the panel layout.");
                LogMessageToFile("Dependencies not found at: " + wdTemp);
            }
            else if (!File.Exists(wdTemp + "ffmpeg.exe") && !File.Exists(wdTemp + "path.txt"))
            {
                MessageBox.Show("Please manually edit or delete the 'path.txt' file, OR put ffmpeg.exe here: \n\n" + wdTemp);
                LogMessageToFile("Path.txt not found at: " + wdTemp);
            }

            InitializeSettings();
                

            // Create log file.
            if (File.Exists(_workingDirectory + "MBSpectrogramLog.txt"))
            {
                try
                {
                    File.Delete(_workingDirectory + "MBSpectrogramLog.txt");
                }
                catch (System.IO.IOException e)
                {
                    LogMessageToFile("File Deletion error: " + e.Message);
                }
            }


            // If file deletion has been enabled, delete the saved images as soon as the plugin loads.
            if (_fileDeletion == true)
            {

                try
                {
                    Directory.Delete(_imageDirectory, true);
                    LogMessageToFile("Spectrogram Images Deleted.");
                }
                catch (System.IO.IOException e)
                {
                    LogMessageToFile("File Deletion error: " + e.Message);
                }

            }


            Directory.CreateDirectory(_imageDirectory);

            about.PluginInfoVersion = PluginInfoVersion;
            about.Name = "Spectrogram-Display";
            about.Description = "This plugin displays the spectrogram of the song being played.";
            about.Author = "Zachary Cohen";
            about.Type = PluginType.PanelView;
            about.VersionMajor = 1;
            about.VersionMinor = 7;
            about.Revision = 2;
            about.MinInterfaceVersion = MinInterfaceVersion;
            about.MinApiRevision = MinApiRevision;
            about.ReceiveNotifications = ReceiveNotificationFlags.PlayerEvents | ReceiveNotificationFlags.TagEvents;
            about.ConfigurationPanelHeight = 0;
            

            //Disables panel header and title. This is only useful for a small number of users...
            if (!File.Exists(_workingDirectory + "noheader.txt"))
            {
                about.TargetApplication = "Spectrogram";
            }
            else
            {
                LogMessageToFile("No header enabled.");
            }


            CurrentDuration();
            CreateConfigHash();

            
            return about;
        }


        // Configuration
        public bool Configure(IntPtr panelHandle)
        {

            SpectrogramConfig configWindow = new SpectrogramConfig(_workingDirectory);
            configWindow.ShowDialog();


            return true;
        }


        // Add the Panel Header item for the Configuration Menu
        public List<ToolStripItem> GetMenuItems()
        {
            List<ToolStripItem> list = new List<ToolStripItem>();
            ToolStripMenuItem configure = new ToolStripMenuItem("Configure Spectrogram");

            configure.Click += configurePanel;

            list.Add(configure);

            return list;
        }

        public void configurePanel(object sender, EventArgs e)
        {
            SpectrogramConfig configWindow = new SpectrogramConfig(_workingDirectory);
            configWindow.ShowDialog();
            SaveSettings();
        }

        

        // Save Settings
        public void SaveSettings()
        {

            CreateConfigHash();
            InitializeSettings();

        }


        // Disabled or Shutting Down
        public void Close(PluginCloseReason reason)
        {
        }


        // Uninstall
        public void Uninstall()
        {
        }


        // GUI Settings
        public int OnDockablePanelCreated(Control panel)
        {
            // Set the Display Settings
            float dpiScaling = 0; // 0 allows dynamic resizing. < 0 allows resizing and fitting to frame. > 0 is static.

            //Enable below if DPI-scaling is off on your display:
            //using (Graphics g = panel.CreateGraphics()) {
            //   dpiScaling = g.DpiY / 96f;
            //}
            

            // Draw the UI
            panel.Paint += DrawPanel;
            panel.Click += PanelClick;
            panel.MouseMove += PanelMouseMove;

            this.panel = panel;
            panelHeight = Convert.ToInt32(110 * dpiScaling); // was set to 50


            return panelHeight;
        }


        // Check if Spectrogram legend and debugging mode are enabled.
        public void InitializeSettings()
        {

            ConfigMgr configMgrLeg = new ConfigMgr();
            string tempPath = mbApiInterface.Setting_GetPersistentStoragePath() + @"Dependencies\config.xml";
            var deserializedObject = configMgrLeg.DeserializeConfig(tempPath);
            _legend = deserializedObject.ShowLegend;
            _debugMode = deserializedObject.EnableDebugging;
            _fileDeletion = deserializedObject.ClearImages;
            _workingDirectory = mbApiInterface.Setting_GetPersistentStoragePath() + @"Dependencies\";
            _imageDirectory = mbApiInterface.Setting_GetPersistentStoragePath() + @"Dependencies\Spectrogram_Images\";

        }
      
        public static bool _legend { get; private set; }
        public static bool _debugMode { get; private set; }
        public static bool _fileDeletion { get; private set; }
        public static string _workingDirectory { get; private set; }
        public static string _imageDirectory { get; private set; }

        
        // Start the Seekbar Timer
        private void initTimer()
        {
            timer = new System.Timers.Timer();
            timer.Interval = 100;
            timer.Elapsed += new ElapsedEventHandler(onTime);
            timer.Enabled = true;
        }
        

        // Draw the Seekbar on Timer Ticks
        private void onTime(object sender, ElapsedEventArgs e)
        {
            if (!_seekbar)
            {
                LogMessageToFile("Timer disabled.");
                timer.Stop();
                timer.Dispose();
            }
            else
            {
                if (panel.InvokeRequired)
                {
                    panel.BeginInvoke((MethodInvoker)delegate ()
                    {
                        Graphics myGraphics = panel.CreateGraphics();
                        SolidBrush blackFill = new SolidBrush(Color.Black);

                        float currentPos = mbApiInterface.Player_GetPosition();
                        float totalTime = mbApiInterface.NowPlaying_GetDuration();
                        float totalLength = this.panel.Width;

                        if (currentPos < _lastPos)
                        {
                            panel.Invalidate();
                        }

                        _lastPos = currentPos;


                        float currentCompletion = (currentPos / totalTime) * (totalLength - (_seekMin * 2));

                        Rectangle rect = new Rectangle(_seekMin, panel.Height - 10, Convert.ToInt32(currentCompletion), 10);


                        myGraphics.FillRectangle(blackFill, rect);

                        blackFill.Dispose();
                        myGraphics.Dispose();
                    });
                }
            }

        }
        

        // Logging
        public void LogMessageToFile(string msg)
        {

            if (_debugMode == true)
            {
                System.IO.StreamWriter sw = System.IO.File.AppendText(
                    _workingDirectory + "MBSpectrogramLog.txt");
                try
                {
                    string logLine = System.String.Format(
                        "{0:G}: {1}", System.DateTime.Now, msg);
                    sw.WriteLine(logLine);
                }
                finally
                {
                    sw.Close();
                }
            }
        }

        // Convert to Time
        private string convTime(float ms)
        {
            TimeSpan t = TimeSpan.FromMilliseconds(ms);

            if (ms > 3600000)
            {
                string answer = string.Format("{0:D2}:{1:D2}:{2:D2}",
                                    t.Hours,
                                    t.Minutes,
                                    t.Seconds);
                return answer;
            }
            else
            {
                string answer = string.Format("{0:D2}:{1:D2}",
                                    t.Minutes,
                                    t.Seconds);

                return answer;
            }

        }

        // Find Position of Cursor in Song / Panel
        private float findPos()
        {


            Point point = panel.PointToClient(Cursor.Position);
            float currentPosX = point.X;

            float getRelativeLocation;
            float totalLength = this.panel.Width;
            float totalTime = _duration;
            

            if (_legend == true)
            {


                if ((currentPosX >= 100 && currentPosX <= (totalLength - 100)))
                {
                    float adjustedLength = totalLength - 200;
                    getRelativeLocation = ((currentPosX - 100) / adjustedLength) * totalTime;

                    return getRelativeLocation;

                }
                else if (currentPosX < 100)
                {

                    return 0;

                }
                else
                {

                    return totalTime;


                }

            }
            else
            {

                // Calculate Where in the Active Song you 'Clicked' (where you'd like to seek to)
                totalLength = this.panel.Width;
                getRelativeLocation = (currentPosX / totalLength) * totalTime;


                // Set the Time in Milliseconds
                return getRelativeLocation;

            }


        }


        // Set Tooltip to Show Time
        private void PanelMouseMove(object sender, EventArgs e)
        {
            if (panel.InvokeRequired)
            {
                panel.BeginInvoke((MethodInvoker)delegate ()
                {
                    toolTip1.ShowAlways = true;
                    toolTip1.SetToolTip(panel, convTime(findPos()));
                });
            }
            else
            {

                toolTip1.ShowAlways = true;
                toolTip1.SetToolTip(panel, convTime(findPos()));

            }

        }


        // Panel Click Event (seekbar)
        private void PanelClick(object sender, EventArgs e)
        {

            MouseEventArgs me = (MouseEventArgs)e;
            if (me.Button == System.Windows.Forms.MouseButtons.Left)
            {

                mbApiInterface.Player_SetPosition((int)Math.Round(findPos()));

            }
            else if (me.Button == System.Windows.Forms.MouseButtons.Right)
            {
                
               mbApiInterface.Player_PlayPause();
                
            }

        }


        // Creates an MD5 hash of the settings file to determine whether it's been changed (so old images can be reused).
        public void CreateConfigHash()
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(_workingDirectory + "config.xml"))
                {
                    var temp = md5.ComputeHash(stream);
                    _hash = BitConverter.ToString(temp).Replace("-", "").ToLowerInvariant();
                }
            }
        }

        public static string _hash { get; private set; }


        // The duration of the current track. Used to make unique file-names and to determine if the file is a stream.
        public void CurrentDuration()
        {
            _duration = mbApiInterface.NowPlaying_GetDuration();

            LogMessageToFile("Current Song Duration: " + _duration);

        }


        public static int _duration { get; private set; }


        // Check if an image already exists for this song and configuration.
        public void ImgCheck()
        {
            LogMessageToFile("Get file path.");
            _path = _imageDirectory + CurrentTitle() + _duration + _hash + ".png";

        }

        public static string _path { get; private set; }


        // Find Closest Power of Two to Determine Appropriate Height of Spectrogram
        public static int RoundToNextPowerOfTwo(int a)
        {
            int next = CeilToNextPowerOfTwo(a);
            int prev = next >> 1;
            return next - a <= a - prev ? next : prev;
        }

        public static int CeilToNextPowerOfTwo(int number)
        {
            int a = number;
            int powOfTwo = 1;

            while (a > 1)
            {
                a = a >> 1;
                powOfTwo = powOfTwo << 1;
            }
            if (powOfTwo != number)
            {
                powOfTwo = powOfTwo << 1;
            }
            return powOfTwo;
        }


        // The title of the current song, stripped down to the characters which can be used in a file-name.
        public string CurrentTitle()
        {

            _spectHeight = RoundToNextPowerOfTwo(panel.Height);

            var rawTitle = mbApiInterface.NowPlaying_GetFileTag(MetaDataType.TrackTitle) + "-" +
                       mbApiInterface.NowPlaying_GetFileTag(MetaDataType.Artist) + "-" + 
                       mbApiInterface.NowPlaying_GetFileProperty(FilePropertyType.Kind) + "-" +
                       panel.Width + "-" + _spectHeight;
            

            var processedTitle = Regex.Replace(rawTitle, @"[ / : * % ? < > | ! ]", "");
            processedTitle = processedTitle.Replace("\"", "");
            

            LogMessageToFile("Raw Title: " + rawTitle + " ////// Processed Title: " + processedTitle);

            return processedTitle;

        }

        public static int _spectHeight { get; private set; }


        // The CLI Commands to be Sent to FFMPEG 
        public string FfmpegArguments(string trackInput, string titleInput)
        {


            ConfigMgr configMgrRead = new ConfigMgr();
            string tempPath = _workingDirectory + @"config.xml";


            var deseralizedObject = configMgrRead.DeserializeConfig(tempPath);

            var ColorScheme = deseralizedObject.ColorScheme;
            var Saturation = deseralizedObject.Saturation;
            var Gain = deseralizedObject.Gain;
            var WindowFunction = deseralizedObject.WindowFunction;
            var ChannelMode = deseralizedObject.ChannelMode;
            var Scale = deseralizedObject.Scale;
            var ShowLegend = (deseralizedObject.ShowLegend) ? "enabled" : "disabled";
            
            var arguments = (@"-i " + trackInput + " -lavfi showspectrumpic=s=" + panel.Width + "x" + _spectHeight + ":"
                             + ChannelMode + ":legend=" + ShowLegend + ":saturation=" + Saturation +
                            ":color=" + ColorScheme + ":scale=" + Scale + ":win_func=" + WindowFunction +
                            ":gain=" + Gain + " " + @"""" + _imageDirectory + titleInput + _duration + _hash + @"""" + ".png");
            
            LogMessageToFile("FFMPEG Arguments: " + arguments);

            return arguments;
        }
        

        // Sets location of Ffmpeg
        public string FfmpegPath()
        {

            string ffmpegPath;

            if (File.Exists(_workingDirectory + @"path.txt"))
            {

                ffmpegPath = File.ReadAllText(_workingDirectory + @"path.txt");
                LogMessageToFile("FFMPEG Custom Path Set To: " + ffmpegPath);

            }
            else
            {

                ffmpegPath = _workingDirectory + "ffmpeg";

            }

            return ffmpegPath;

        }
        

        // The Function for Triggering the Generation of Spectrogram Images
        public void RunCmd()
        {

            /*if (mbApiInterface.NowPlaying_GetFileProperty(FilePropertyType.Size) != "N/A")
            {*/

                // Start a Background ffmpeg Process with the Arguments we Feed it   
                var proc = new Process();
                proc.StartInfo.WorkingDirectory = _imageDirectory;
                proc.StartInfo.FileName = FfmpegPath();
                proc.StartInfo.Arguments = FfmpegArguments(@"""" + mbApiInterface.NowPlaying_GetFileUrl() + @"""", CurrentTitle());
                proc.StartInfo.CreateNoWindow = true;
                proc.StartInfo.RedirectStandardError = true;
                proc.StartInfo.UseShellExecute = false;

                if (!proc.Start())
                {

                    MessageBox.Show("Ffmpeg didn't start properly.");
                    LogMessageToFile("Ffmpeg didn't start properly.");
                    return;

                }

                var reader = proc.StandardError;
                string line;
                while ((line = reader.ReadLine()) != null) Console.WriteLine(line);

                proc.Close();

                //SetLastPlayed();

                LogMessageToFile("Image generated.");

            /*}
            else
            {
                LogMessageToFile("Stream detected.");
            }*/
            
        }
        
        
        // Update or Generate Image When Track Changes
        public void ReceiveNotification(string sourceFileUrl, NotificationType type)
        {
            switch (type)
            {


                case NotificationType.TrackChanged:

                    LogMessageToFile("\n\n\n Track changed.");

                    CurrentDuration();
                    ImgCheck();

                    _lastPos = 0;

                    // Set Seekbar Display
                    if (File.Exists(_workingDirectory + @"\seekbar.txt"))
                    {
                        _seekbar = true;

                        if (_legend == true)
                        {
                            _seekMin = 100;
                        }
                        else
                        {
                            _seekMin = 0;
                        }

                        initTimer();
                    }
                    else
                    {
                        _seekbar = false;
                    }

                    //LogMessageToFile("Size: " + mbApiInterface.NowPlaying_GetFileProperty(FilePropertyType.Size));

                    // If the Spectrogram Image for the Song that Just Started Playing Doesn't Exist, Create One (if it's not a stream: size "N/A").
                    if (_duration > 0 && !File.Exists(_path))
                    {

                        LogMessageToFile("Path: " + _path);
                        LogMessageToFile("Beginning generation of image.");
                        RunCmd();

                    }

                    // Refresh the Panel.
                    panel.Invalidate();

                    // Rebuild the Panel on Track Changes
                    panel.Paint += DrawPanel;
                    break;


            }
        }
        

        // Draw Plugin Panel and Load Image 
        private void DrawPanel(object sender, PaintEventArgs e)
        {
            _lastPos = 0;

            // Set Colors
            var bg = panel.BackColor;
            var text1 = panel.ForeColor;
            var text2 = text1;
            var highlight = Color.FromArgb(2021216);
            e.Graphics.Clear(bg);

            // Load Spectrogram Image if it Exists Already
            if (File.Exists(_path))
            {

                
                LogMessageToFile("Image found.");
                var image = Image.FromFile(_path, true);

                if (_seekbar)
                {
                    image = new Bitmap(image, new Size(panel.Width, panel.Height - 10));

                    if (_legend)
                    {
                        SolidBrush blackFill = new SolidBrush(Color.Black);
                        Rectangle rectLeft = new Rectangle(0, panel.Height - 10, 100, 10);
                        Rectangle rectRight = new Rectangle(panel.Width - 100, panel.Height - 10, 100, 10);

                        e.Graphics.FillRectangle(blackFill, rectLeft);
                        e.Graphics.FillRectangle(blackFill, rectRight);
                        blackFill.Dispose();

                    }
                    
                }
                else
                {
                    image = new Bitmap(image, new Size(panel.Width, panel.Height));
                }


                e.Graphics.DrawImage(image, new Point(0, 0));

                
            }
            else if (_duration <= 0)
            {

                String Placeholder = _workingDirectory + @"placeholder.png";

                if (File.Exists(Placeholder))
                {

                    LogMessageToFile("Image found.");
                    var image = Image.FromFile(Placeholder, true);
                    image = new Bitmap(image, new Size(panel.Width, _spectHeight));
                    e.Graphics.DrawImage(image, new Point(0, 0));
                    

                }


            }
        }

    }
}