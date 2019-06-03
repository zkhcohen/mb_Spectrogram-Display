namespace MusicBeePlugin
{
    partial class SpectrogramConfig
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        public void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.colorSchemeBox = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.scaleBox = new System.Windows.Forms.ComboBox();
            this.clearImagesBox = new System.Windows.Forms.CheckBox();
            this.showFrequencyBox = new System.Windows.Forms.CheckBox();
            this.windowFunctionBox = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.gainBox = new System.Windows.Forms.TextBox();
            this.saturationBox = new System.Windows.Forms.TextBox();
            this.channelModeBox = new System.Windows.Forms.ComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this.saveButton = new System.Windows.Forms.Button();
            this.resetDefaultsButton = new System.Windows.Forms.Button();
            this.pathLabel = new System.Windows.Forms.Label();
            this.pathBox = new System.Windows.Forms.TextBox();
            this.setPathButton = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.clearPathButton = new System.Windows.Forms.Button();
            this.debugModeText = new System.Windows.Forms.Label();
            this.debugModeBox = new System.Windows.Forms.CheckBox();
            this.enableSeekbarBox = new System.Windows.Forms.CheckBox();
            this.disableHeaderBox = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 31);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(76, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Color Scheme:";
            // 
            // colorSchemeBox
            // 
            this.colorSchemeBox.FormattingEnabled = true;
            this.colorSchemeBox.Items.AddRange(new object[] {
            "Channel",
            "Intensity",
            "Rainbow",
            "Moreland",
            "Nebulae",
            "Fire",
            "Fiery",
            "Fruit",
            "Cool",
            "Magma",
            "Green",
            "Viridis",
            "Plasma",
            "Cividis",
            "Terrain"});
            this.colorSchemeBox.Location = new System.Drawing.Point(15, 53);
            this.colorSchemeBox.MaxDropDownItems = 15;
            this.colorSchemeBox.Name = "colorSchemeBox";
            this.colorSchemeBox.Size = new System.Drawing.Size(124, 21);
            this.colorSchemeBox.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 222);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(37, 13);
            this.label2.TabIndex = 2;
            this.label2.Text = "Scale:";
            // 
            // scaleBox
            // 
            this.scaleBox.FormattingEnabled = true;
            this.scaleBox.Items.AddRange(new object[] {
            "lin",
            "sqrt",
            "cbrt",
            "log",
            "4thrt",
            "5thrt"});
            this.scaleBox.Location = new System.Drawing.Point(66, 219);
            this.scaleBox.Name = "scaleBox";
            this.scaleBox.Size = new System.Drawing.Size(73, 21);
            this.scaleBox.TabIndex = 3;
            // 
            // clearImagesBox
            // 
            this.clearImagesBox.AutoSize = true;
            this.clearImagesBox.Location = new System.Drawing.Point(15, 254);
            this.clearImagesBox.Name = "clearImagesBox";
            this.clearImagesBox.Size = new System.Drawing.Size(139, 17);
            this.clearImagesBox.TabIndex = 5;
            this.clearImagesBox.Text = "Clear Images on Restart";
            this.clearImagesBox.UseVisualStyleBackColor = true;
            // 
            // showFrequencyBox
            // 
            this.showFrequencyBox.AutoSize = true;
            this.showFrequencyBox.Location = new System.Drawing.Point(15, 286);
            this.showFrequencyBox.Name = "showFrequencyBox";
            this.showFrequencyBox.Size = new System.Drawing.Size(145, 17);
            this.showFrequencyBox.TabIndex = 6;
            this.showFrequencyBox.Text = "Show Frequency Legend";
            this.showFrequencyBox.UseVisualStyleBackColor = true;
            // 
            // windowFunctionBox
            // 
            this.windowFunctionBox.FormattingEnabled = true;
            this.windowFunctionBox.Items.AddRange(new object[] {
            "Rect",
            "Bartlett",
            "Hann",
            "Hanning",
            "Hamming",
            "Blackman",
            "Welch",
            "Flattop",
            "Bharris",
            "Bnuttall",
            "Bhann",
            "Sine",
            "Nuttall",
            "Lanczos",
            "Gauss",
            "Tukey",
            "Dolph",
            "Cauchy",
            "Parzen",
            "Poisson",
            "Bohman"});
            this.windowFunctionBox.Location = new System.Drawing.Point(15, 129);
            this.windowFunctionBox.Name = "windowFunctionBox";
            this.windowFunctionBox.Size = new System.Drawing.Size(124, 21);
            this.windowFunctionBox.TabIndex = 8;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 113);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(93, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Window Function:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(165, 30);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(58, 13);
            this.label4.TabIndex = 9;
            this.label4.Text = "Saturation:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(165, 59);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(32, 13);
            this.label5.TabIndex = 10;
            this.label5.Text = "Gain:";
            // 
            // gainBox
            // 
            this.gainBox.Location = new System.Drawing.Point(235, 54);
            this.gainBox.Name = "gainBox";
            this.gainBox.Size = new System.Drawing.Size(28, 20);
            this.gainBox.TabIndex = 11;
            // 
            // saturationBox
            // 
            this.saturationBox.Location = new System.Drawing.Point(235, 28);
            this.saturationBox.Name = "saturationBox";
            this.saturationBox.Size = new System.Drawing.Size(28, 20);
            this.saturationBox.TabIndex = 12;
            // 
            // channelModeBox
            // 
            this.channelModeBox.FormattingEnabled = true;
            this.channelModeBox.Items.AddRange(new object[] {
            "Combined",
            "Separate"});
            this.channelModeBox.Location = new System.Drawing.Point(15, 181);
            this.channelModeBox.MaxDropDownItems = 15;
            this.channelModeBox.Name = "channelModeBox";
            this.channelModeBox.Size = new System.Drawing.Size(124, 21);
            this.channelModeBox.TabIndex = 18;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(12, 165);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(79, 13);
            this.label8.TabIndex = 17;
            this.label8.Text = "Channel Mode:";
            // 
            // saveButton
            // 
            this.saveButton.Location = new System.Drawing.Point(170, 129);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(93, 119);
            this.saveButton.TabIndex = 19;
            this.saveButton.Text = "Save";
            this.saveButton.UseVisualStyleBackColor = true;
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // resetDefaultsButton
            // 
            this.resetDefaultsButton.Location = new System.Drawing.Point(170, 254);
            this.resetDefaultsButton.Name = "resetDefaultsButton";
            this.resetDefaultsButton.Size = new System.Drawing.Size(93, 49);
            this.resetDefaultsButton.TabIndex = 20;
            this.resetDefaultsButton.Text = "Reset to Defaults";
            this.resetDefaultsButton.UseVisualStyleBackColor = true;
            this.resetDefaultsButton.Click += new System.EventHandler(this.resetDefaultsButton_Click);
            // 
            // pathLabel
            // 
            this.pathLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 7.3F);
            this.pathLabel.Location = new System.Drawing.Point(16, 317);
            this.pathLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.pathLabel.Name = "pathLabel";
            this.pathLabel.Size = new System.Drawing.Size(263, 21);
            this.pathLabel.TabIndex = 0;
            this.pathLabel.Text = "Optional - Path to Preexisting Installation of FFMPEG:";
            // 
            // pathBox
            // 
            this.pathBox.Location = new System.Drawing.Point(15, 345);
            this.pathBox.Margin = new System.Windows.Forms.Padding(2);
            this.pathBox.Name = "pathBox";
            this.pathBox.Size = new System.Drawing.Size(248, 20);
            this.pathBox.TabIndex = 21;
            // 
            // setPathButton
            // 
            this.setPathButton.Location = new System.Drawing.Point(15, 391);
            this.setPathButton.Margin = new System.Windows.Forms.Padding(2);
            this.setPathButton.Name = "setPathButton";
            this.setPathButton.Size = new System.Drawing.Size(103, 24);
            this.setPathButton.TabIndex = 22;
            this.setPathButton.Text = "Set Path";
            this.setPathButton.UseVisualStyleBackColor = true;
            this.setPathButton.Click += new System.EventHandler(this.setPathButton_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.8F);
            this.label6.Location = new System.Drawing.Point(64, 366);
            this.label6.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(167, 13);
            this.label6.TabIndex = 23;
            this.label6.Text = "Note: Include \'/ffpmeg\' in the path.";
            // 
            // clearPathButton
            // 
            this.clearPathButton.Location = new System.Drawing.Point(159, 391);
            this.clearPathButton.Margin = new System.Windows.Forms.Padding(2);
            this.clearPathButton.Name = "clearPathButton";
            this.clearPathButton.Size = new System.Drawing.Size(103, 24);
            this.clearPathButton.TabIndex = 24;
            this.clearPathButton.Text = "Clear Path";
            this.clearPathButton.UseVisualStyleBackColor = true;
            this.clearPathButton.Click += new System.EventHandler(this.clearPathButton_Click);
            // 
            // debugModeText
            // 
            this.debugModeText.Location = new System.Drawing.Point(12, 431);
            this.debugModeText.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.debugModeText.Name = "debugModeText";
            this.debugModeText.Size = new System.Drawing.Size(196, 50);
            this.debugModeText.TabIndex = 25;
            this.debugModeText.Text = "Enable Debug Mode. Generates new images and logs to /Dependencies/MBSpectrogramLo" +
    "g.txt.";
            // 
            // debugModeBox
            // 
            this.debugModeBox.AutoSize = true;
            this.debugModeBox.Location = new System.Drawing.Point(225, 446);
            this.debugModeBox.Margin = new System.Windows.Forms.Padding(2);
            this.debugModeBox.Name = "debugModeBox";
            this.debugModeBox.Size = new System.Drawing.Size(15, 14);
            this.debugModeBox.TabIndex = 26;
            this.debugModeBox.UseVisualStyleBackColor = true;
            // 
            // enableSeekbarBox
            // 
            this.enableSeekbarBox.AutoSize = true;
            this.enableSeekbarBox.Location = new System.Drawing.Point(15, 522);
            this.enableSeekbarBox.Name = "enableSeekbarBox";
            this.enableSeekbarBox.Size = new System.Drawing.Size(191, 17);
            this.enableSeekbarBox.TabIndex = 28;
            this.enableSeekbarBox.Text = "EXPERIMENTAL: Enable Seekbar";
            this.enableSeekbarBox.UseVisualStyleBackColor = true;
            this.enableSeekbarBox.CheckedChanged += new System.EventHandler(this.enableSeekbarBox_CheckedChanged);
            // 
            // disableHeaderBox
            // 
            this.disableHeaderBox.AutoSize = true;
            this.disableHeaderBox.Location = new System.Drawing.Point(15, 487);
            this.disableHeaderBox.Name = "disableHeaderBox";
            this.disableHeaderBox.Size = new System.Drawing.Size(188, 17);
            this.disableHeaderBox.TabIndex = 29;
            this.disableHeaderBox.Text = "EXPERIMENTAL: Disable Header";
            this.disableHeaderBox.UseVisualStyleBackColor = true;
            this.disableHeaderBox.CheckedChanged += new System.EventHandler(this.disableHeaderBox_CheckedChanged);
            // 
            // SpectrogramConfig
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(275, 559);
            this.Controls.Add(this.disableHeaderBox);
            this.Controls.Add(this.enableSeekbarBox);
            this.Controls.Add(this.debugModeBox);
            this.Controls.Add(this.debugModeText);
            this.Controls.Add(this.clearPathButton);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.setPathButton);
            this.Controls.Add(this.pathBox);
            this.Controls.Add(this.pathLabel);
            this.Controls.Add(this.resetDefaultsButton);
            this.Controls.Add(this.saveButton);
            this.Controls.Add(this.channelModeBox);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.saturationBox);
            this.Controls.Add(this.gainBox);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.windowFunctionBox);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.showFrequencyBox);
            this.Controls.Add(this.clearImagesBox);
            this.Controls.Add(this.scaleBox);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.colorSchemeBox);
            this.Controls.Add(this.label1);
            this.Name = "SpectrogramConfig";
            this.Text = "Configure Spectrogram";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public System.Windows.Forms.Label label1;
        public System.Windows.Forms.ComboBox colorSchemeBox;
        public System.Windows.Forms.Label label2;
        public System.Windows.Forms.ComboBox scaleBox;
        public System.Windows.Forms.CheckBox clearImagesBox;
        public System.Windows.Forms.CheckBox showFrequencyBox;
        public System.Windows.Forms.ComboBox windowFunctionBox;
        public System.Windows.Forms.Label label3;
        public System.Windows.Forms.Label label4;
        public System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox gainBox;
        private System.Windows.Forms.TextBox saturationBox;
        public System.Windows.Forms.ComboBox channelModeBox;
        public System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.Button resetDefaultsButton;
        private System.Windows.Forms.Label pathLabel;
        private System.Windows.Forms.TextBox pathBox;
        private System.Windows.Forms.Button setPathButton;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button clearPathButton;
        private System.Windows.Forms.Label debugModeText;
        private System.Windows.Forms.CheckBox debugModeBox;
        private System.Windows.Forms.CheckBox enableSeekbarBox;
        private System.Windows.Forms.CheckBox disableHeaderBox;
    }


}

