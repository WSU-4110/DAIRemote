namespace DAIRemote
{
    partial class DAIRemoteApplicationUI
    {
       
        private System.ComponentModel.IContainer components = null;

        
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


        private void InitializeComponent()
        {
            BtnSaveDisplayConfig = new Button();
            checkBoxStartup = new CheckBox();
            BtnCycleAudioOutputs = new Button();
            DisplayLoadProfilesLayout = new FlowLayoutPanel();
            DisplayDeleteProfilesLayout = new FlowLayoutPanel();
            lblCurrentHotkey = new Label();
            hotkeyComboBox = new ComboBox();
            btnSetHotkey = new Button();
            SuspendLayout();
            // 
            // BtnSaveDisplayConfig
            // 
            BtnSaveDisplayConfig.AccessibleName = "BtnSaveDisplayConfig";
            BtnSaveDisplayConfig.BackColor = Color.LightSkyBlue;
            BtnSaveDisplayConfig.Location = new Point(29, 20);
            BtnSaveDisplayConfig.Margin = new Padding(4, 5, 4, 5);
            BtnSaveDisplayConfig.Name = "BtnSaveDisplayConfig";
            BtnSaveDisplayConfig.Size = new Size(286, 67);
            BtnSaveDisplayConfig.TabIndex = 0;
            BtnSaveDisplayConfig.Text = "Add Display Profile";
            BtnSaveDisplayConfig.UseVisualStyleBackColor = false;
            BtnSaveDisplayConfig.Click += BtnAddDisplayConfig_Click;
            // 
            // checkBoxStartup
            // 
            checkBoxStartup.AutoSize = true;
            checkBoxStartup.ForeColor = SystemColors.Control;
            checkBoxStartup.Location = new Point(29, 441);
            checkBoxStartup.Margin = new Padding(4, 5, 4, 5);
            checkBoxStartup.Name = "checkBoxStartup";
            checkBoxStartup.Size = new Size(272, 29);
            checkBoxStartup.TabIndex = 4;
            checkBoxStartup.Text = "Launch application on startup";
            checkBoxStartup.UseVisualStyleBackColor = true;
            checkBoxStartup.CheckedChanged += CheckBoxStartup_CheckedChanged;
            // 
            // BtnCycleAudioOutputs
            // 
            BtnCycleAudioOutputs.AccessibleName = "BtnCycleAudioOutputs";
            BtnCycleAudioOutputs.BackColor = Color.LightSkyBlue;
            BtnCycleAudioOutputs.Location = new Point(29, 107);
            BtnCycleAudioOutputs.Margin = new Padding(4, 5, 4, 5);
            BtnCycleAudioOutputs.Name = "BtnCycleAudioOutputs";
            BtnCycleAudioOutputs.Size = new Size(286, 67);
            BtnCycleAudioOutputs.TabIndex = 5;
            BtnCycleAudioOutputs.Text = "Cycle Audio Devices";
            BtnCycleAudioOutputs.UseVisualStyleBackColor = false;
            BtnCycleAudioOutputs.Click += BtnCycleAudioOutputs_Click;
            // 
            // DisplayLoadProfilesLayout
            // 
            DisplayLoadProfilesLayout.Location = new Point(404, 20);
            DisplayLoadProfilesLayout.Margin = new Padding(4, 5, 4, 5);
            DisplayLoadProfilesLayout.Name = "DisplayLoadProfilesLayout";
            DisplayLoadProfilesLayout.Size = new Size(680, 297);
            DisplayLoadProfilesLayout.TabIndex = 6;
            // 
            // DisplayDeleteProfilesLayout
            // 
            DisplayDeleteProfilesLayout.Location = new Point(404, 327);
            DisplayDeleteProfilesLayout.Margin = new Padding(4, 5, 4, 5);
            DisplayDeleteProfilesLayout.Name = "DisplayDeleteProfilesLayout";
            DisplayDeleteProfilesLayout.Size = new Size(680, 315);
            DisplayDeleteProfilesLayout.TabIndex = 7;
            // 
            // lblCurrentHotkey
            // 
            lblCurrentHotkey.AutoSize = true;
            lblCurrentHotkey.ForeColor = Color.White;
            lblCurrentHotkey.Location = new Point(31, 374);
            lblCurrentHotkey.Margin = new Padding(4, 0, 4, 0);
            lblCurrentHotkey.Name = "lblCurrentHotkey";
            lblCurrentHotkey.Size = new Size(184, 25);
            lblCurrentHotkey.TabIndex = 0;
            lblCurrentHotkey.Text = "Current Hotkey: None";
            lblCurrentHotkey.Click += lblCurrentHotkey_Click;
            // 
            // hotkeyComboBox
            // 
            hotkeyComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            hotkeyComboBox.Items.AddRange(new object[] { "None", "LButton", "RButton", "Cancel", "MButton", "XButton1", "XButton2", "Back", "Tab", "LineFeed", "Clear", "Return", "Enter", "ShiftKey", "ControlKey", "Menu", "Pause", "CapsLock", "Capital", "HangulMode", "HanguelMode", "KanaMode", "JunjaMode", "FinalMode", "HanjaMode", "KanjiMode", "Escape", "IMEConvert", "IMENonconvert", "IMEAccept", "IMEAceept", "IMEModeChange", "Space", "Prior", "PageUp", "PageDown", "Next", "End", "Home", "Left", "Up", "Right", "Down", "Select", "Print", "Execute", "Snapshot", "PrintScreen", "Insert", "Delete", "Help", "D0", "D1", "D2", "D3", "D4", "D5", "D6", "D7", "D8", "D9", "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", "LWin", "RWin", "Apps", "Sleep", "NumPad0", "NumPad1", "NumPad2", "NumPad3", "NumPad4", "NumPad5", "NumPad6", "NumPad7", "NumPad8", "NumPad9", "Multiply", "Add", "Separator", "Subtract", "Decimal", "Divide", "F1", "F2", "F3", "F4", "F5", "F6", "F7", "F8", "F9", "F10", "F11", "F12", "F13", "F14", "F15", "F16", "F17", "F18", "F19", "F20", "F21", "F22", "F23", "F24", "NumLock", "Scroll", "LShiftKey", "RShiftKey", "LControlKey", "RControlKey", "LMenu", "RMenu", "BrowserBack", "BrowserForward", "BrowserRefresh", "BrowserStop", "BrowserSearch", "BrowserFavorites", "BrowserHome", "VolumeMute", "VolumeDown", "VolumeUp", "MediaNextTrack", "MediaPreviousTrack", "MediaStop", "MediaPlayPause", "LaunchMail", "SelectMedia", "LaunchApplication1", "LaunchApplication2", "Oem1", "OemSemicolon", "Oemplus", "Oemcomma", "OemMinus", "OemPeriod", "OemQuestion", "Oem2", "Oemtilde", "Oem3", "OemOpenBrackets", "Oem4", "Oem5", "OemPipe", "OemCloseBrackets", "Oem6", "OemQuotes", "Oem7", "Oem8", "Oem102", "OemBackslash", "ProcessKey", "Packet", "Attn", "Crsel", "Exsel", "EraseEof", "Play", "Zoom", "NoName", "Pa1", "OemClear", "KeyCode", "Shift", "Control", "Alt", "Modifiers" });
            hotkeyComboBox.Location = new Point(31, 327);
            hotkeyComboBox.Margin = new Padding(4, 5, 4, 5);
            hotkeyComboBox.Name = "hotkeyComboBox";
            hotkeyComboBox.Size = new Size(284, 33);
            hotkeyComboBox.TabIndex = 1;
            hotkeyComboBox.SelectedIndexChanged += hotkeyComboBox_SelectedIndexChanged;
            // 
            // btnSetHotkey
            // 
            btnSetHotkey.AccessibleName = "btnSetHotkey";
            btnSetHotkey.BackColor = Color.LightSkyBlue;
            btnSetHotkey.Location = new Point(29, 239);
            btnSetHotkey.Name = "btnSetHotkey";
            btnSetHotkey.Size = new Size(286, 60);
            btnSetHotkey.TabIndex = 0;
            btnSetHotkey.Text = "Set Hotkey";
            btnSetHotkey.UseVisualStyleBackColor = false;
            btnSetHotkey.Click += btnSetHotkey_Click;
            // 
            // DAIRemoteApplicationUI
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(50, 50, 50);
            ClientSize = new Size(1101, 658);
            Controls.Add(btnSetHotkey);
            Controls.Add(lblCurrentHotkey);
            Controls.Add(hotkeyComboBox);
            Controls.Add(DisplayDeleteProfilesLayout);
            Controls.Add(DisplayLoadProfilesLayout);
            Controls.Add(BtnCycleAudioOutputs);
            Controls.Add(checkBoxStartup);
            Controls.Add(BtnSaveDisplayConfig);
            Margin = new Padding(4, 5, 4, 5);
            Name = "DAIRemoteApplicationUI";
            Text = "DAIRemote";
            Load += DAIRemoteApplicationUI_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button BtnSaveDisplayConfig;
        private CheckBox checkBoxStartup;
        private Button BtnCycleAudioOutputs;
        private FlowLayoutPanel DisplayLoadProfilesLayout;
        private FlowLayoutPanel DisplayDeleteProfilesLayout;
        private Label lblCurrentHotkey;
        private ComboBox hotkeyComboBox;
        private Button btnSetHotkey;

    }
}