using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Security.Cryptography;
using System.Text.RegularExpressions;
using System.Timers;
using System.Windows.Forms;

namespace MusicBeePlugin
{
    public partial class Plugin
    {
        #region Fields

        private readonly PluginInfo about = new PluginInfo();

        public int panelHeight;

        private float _lastPos = 0;

        private bool _seekbar = false;

        private int _seekMin = 0;

        // Declarations
        private MusicBeeApiInterface mbApiInterface;

        private Control panel;

        private System.Timers.Timer timer;

        ToolTip toolTip1 = new ToolTip();

        #endregion

        #region Properties

        public static bool _debugMode { get; private set; }

        public static int _duration { get; private set; }

        public static bool _fileDeletion { get; private set; }

        public static string _hash { get; private set; }

        public static string _fileHash { get; private set; }

        public static string _imageDirectory { get; private set; }

        public static bool _legend { get; private set; }

        public static string _path { get; private set; }

        public static int _spectHeight { get; private set; }

        public static int _spectWidth { get; private set; }

        public static int _spectBuffer { get; private set; }

        public static string _workingDirectory { get; private set; }

        #endregion

        #region Methods

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

        public static int RoundToTen(int i)
        {
            return ((int)Math.Round(i / 10.0)) * 10;
        }

        // Find Closest Power of Two to Determine Appropriate Height of Spectrogram
        public static int RoundToNextPowerOfTwo(int a)
        {
            int next = CeilToNextPowerOfTwo(a);
            int prev = next >> 1;
            return next - a <= a - prev ? next : prev;
        }

        // Disabled or Shutting Down
        public void Close(PluginCloseReason reason)
        {
        }

        // Configuration
        public bool Configure(IntPtr panelHandle)
        {

            SpectrogramConfig configWindow = new SpectrogramConfig(_workingDirectory);
            configWindow.ShowDialog();


            return true;
        }

        public void configurePanel(object sender, EventArgs e)
        {
            SpectrogramConfig configWindow = new SpectrogramConfig(_workingDirectory);
            configWindow.ShowDialog();
            SaveSettings();
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

        public void CreateFileHash()
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(mbApiInterface.NowPlaying_GetFileUrl()))
                {
                    var temp = md5.ComputeHash(stream);
                    _fileHash = BitConverter.ToString(temp).Replace("-", "").ToLowerInvariant();
                }
            }
        }

        // The duration of the current track. Used to determine if the file is a stream.
        public void CurrentDuration()
        {
            _duration = mbApiInterface.NowPlaying_GetDuration();

            LogMessageToFile("Current Song Duration: " + _duration);
        }

        // The title of the current song, stripped down to the characters which can be used in a file-name.
        public string CurrentTitle()
        {
            CreateFileHash();
            _spectHeight = RoundToNextPowerOfTwo(panel.Height);
            _spectWidth = RoundToTen(panel.Width);
            var buffer = 141 * ((decimal)_spectWidth / (_spectWidth + 282));
            _spectBuffer = (int)buffer;
            string processedTitle = _fileHash + _spectHeight + _spectWidth;
            


            LogMessageToFile("Title: " + processedTitle);

            return processedTitle;
        }

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

            var arguments = (@"-i " + trackInput + " -lavfi showspectrumpic=s=" + _spectWidth + "x" + _spectHeight + ":"
                             + ChannelMode + ":legend=" + ShowLegend + ":saturation=" + Saturation +
                            ":color=" + ColorScheme + ":scale=" + Scale + ":win_func=" + WindowFunction +
                            ":gain=" + Gain + " " + @"""" + _imageDirectory + titleInput + _hash + @"""" + ".png");

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

        // Add the Panel Header item for the Configuration Menu
        public List<ToolStripItem> GetMenuItems()
        {
            List<ToolStripItem> list = new List<ToolStripItem>();
            ToolStripMenuItem configure = new ToolStripMenuItem("Configure Spectrogram");

            configure.Click += configurePanel;

            list.Add(configure);

            return list;
        }

        // Check if an image already exists for this song and configuration.
        public void ImgCheck()
        {
            LogMessageToFile("Get file path.");
            _path = _imageDirectory + CurrentTitle() + _hash + ".png";
        }

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
            about.Author = "zkhcohen";
            about.Type = PluginType.PanelView;
            about.VersionMajor = 1;
            about.VersionMinor = 7;
            about.Revision = 5;
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

        // Update or Generate Image When Track Changes
        public void ReceiveNotification(string sourceFileUrl, NotificationType type)
        {
            switch (type)
            {


                case NotificationType.TrackChanged:

                    LogMessageToFile("\n\n\n Track changed.");

                    CurrentDuration();

                    _lastPos = 0;

                    if (_duration > 0)
                    {
                        ImgCheck();

                        // Set Seekbar Display
                        if (File.Exists(_workingDirectory + @"\seekbar.txt"))
                        {
                            _seekbar = true;

                            if (_legend == true)
                            {
                                _seekMin = _spectBuffer;
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
                        if (!File.Exists(_path))
                        {

                            LogMessageToFile("Path: " + _path);
                            LogMessageToFile("Beginning generation of image.");
                            RunCmd();

                        }
                    }
                    else 
                    {
                        _path = null;
                    }

                    // Refresh the Panel.
                    panel.Invalidate();

                    // Rebuild the Panel on Track Changes
                    panel.Paint += DrawPanel;
                    break;


            }
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
        }

        // Save Settings
        public void SaveSettings()
        {

            CreateConfigHash();
            InitializeSettings();
        }

        // Uninstall
        public void Uninstall()
        {
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
                        Rectangle rectLeft = new Rectangle(0, panel.Height - 10, _spectBuffer, 10);
                        Rectangle rectRight = new Rectangle(panel.Width - _spectBuffer, panel.Height - 10, _spectBuffer, 10);

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
                    image = new Bitmap(image, new Size(_spectWidth, _spectHeight));
                    e.Graphics.DrawImage(image, new Point(0, 0));


                }


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


                if ((currentPosX >= _spectBuffer && currentPosX <= (totalLength - _spectBuffer)))
                {
                    float adjustedLength = totalLength - 200;
                    getRelativeLocation = ((currentPosX - _spectBuffer) / adjustedLength) * totalTime;

                    return getRelativeLocation;

                }
                else if (currentPosX < _spectBuffer)
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

                        Rectangle rect = new Rectangle(_seekMin, panel.Height - 10, (int)currentCompletion, 10);
                        myGraphics.FillRectangle(blackFill, rect);

                        blackFill.Dispose();
                        myGraphics.Dispose();
                    });
                }
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

        #endregion
    }
}
