namespace DAIRemote;

partial class HotkeyManager
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
    private void InitializeComponent()
    {
        this.HotkeyInputBox = new TextBox();
        this.HotkeyFormBtnPanel = new TableLayoutPanel();
        this.HotkeyFormClearBtn = new Button();
        this.HotkeyFormOkBtn = new Button();
        this.HotkeyFormCancelBtn = new Button();
        this.HotkeyFormBtnPanel.SuspendLayout();
        this.SuspendLayout();
        // 
        // HotkeyInputBox
        // 
        this.HotkeyInputBox.AcceptsReturn = true;
        this.HotkeyInputBox.Dock = DockStyle.Top;
        this.HotkeyInputBox.Location = new Point(0, 0);
        this.HotkeyInputBox.Margin = new Padding(2);
        this.HotkeyInputBox.Name = "HotkeyInputBox";
        this.HotkeyInputBox.PlaceholderText = "Press a key/key combination";
        this.HotkeyInputBox.ReadOnly = true;
        this.HotkeyInputBox.Size = new Size(254, 23);
        this.HotkeyInputBox.TabIndex = 0;
        // 
        // HotkeyFormBtnPanel
        // 
        this.HotkeyFormBtnPanel.AutoSize = true;
        this.HotkeyFormBtnPanel.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        this.HotkeyFormBtnPanel.ColumnCount = 3;
        this.HotkeyFormBtnPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.3333321F));
        this.HotkeyFormBtnPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.3333321F));
        this.HotkeyFormBtnPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33.3333321F));
        this.HotkeyFormBtnPanel.Controls.Add(this.HotkeyFormClearBtn, 0, 0);
        this.HotkeyFormBtnPanel.Controls.Add(this.HotkeyFormOkBtn, 1, 0);
        this.HotkeyFormBtnPanel.Controls.Add(this.HotkeyFormCancelBtn, 2, 0);
        this.HotkeyFormBtnPanel.Dock = DockStyle.Fill;
        this.HotkeyFormBtnPanel.GrowStyle = TableLayoutPanelGrowStyle.FixedSize;
        this.HotkeyFormBtnPanel.Location = new Point(0, 23);
        this.HotkeyFormBtnPanel.Name = "HotkeyFormBtnPanel";
        this.HotkeyFormBtnPanel.RowCount = 1;
        this.HotkeyFormBtnPanel.RowStyles.Add(new RowStyle());
        this.HotkeyFormBtnPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
        this.HotkeyFormBtnPanel.Size = new Size(254, 28);
        this.HotkeyFormBtnPanel.TabIndex = 1;
        // 
        // HotkeyFormClearBtn
        // 
        this.HotkeyFormClearBtn.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        this.HotkeyFormClearBtn.Location = new Point(3, 3);
        this.HotkeyFormClearBtn.Name = "HotkeyFormClearBtn";
        this.HotkeyFormClearBtn.Size = new Size(75, 23);
        this.HotkeyFormClearBtn.TabIndex = 0;
        this.HotkeyFormClearBtn.Text = "Clear";
        this.HotkeyFormClearBtn.UseVisualStyleBackColor = true;
        // 
        // HotkeyFormOkBtn
        // 
        this.HotkeyFormOkBtn.DialogResult = DialogResult.OK;
        this.HotkeyFormOkBtn.Location = new Point(87, 3);
        this.HotkeyFormOkBtn.Name = "HotkeyFormOkBtn";
        this.HotkeyFormOkBtn.Size = new Size(75, 23);
        this.HotkeyFormOkBtn.TabIndex = 1;
        this.HotkeyFormOkBtn.Text = "Ok";
        this.HotkeyFormOkBtn.UseVisualStyleBackColor = true;
        // 
        // HotkeyFormCancelBtn
        // 
        this.HotkeyFormCancelBtn.DialogResult = DialogResult.Cancel;
        this.HotkeyFormCancelBtn.Location = new Point(171, 3);
        this.HotkeyFormCancelBtn.Name = "HotkeyFormCancelBtn";
        this.HotkeyFormCancelBtn.Size = new Size(75, 23);
        this.HotkeyFormCancelBtn.TabIndex = 2;
        this.HotkeyFormCancelBtn.Text = "Cancel";
        this.HotkeyFormCancelBtn.UseVisualStyleBackColor = true;
        // 
        // HotkeyManager
        // 
        this.AutoScaleDimensions = new SizeF(96F, 96F);
        this.AutoScaleMode = AutoScaleMode.Dpi;
        this.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        this.ClientSize = new Size(254, 51);
        this.Controls.Add(this.HotkeyFormBtnPanel);
        this.Controls.Add(this.HotkeyInputBox);
        this.FormBorderStyle = FormBorderStyle.FixedSingle;
        this.MaximizeBox = false;
        this.MinimumSize = new Size(265, 74);
        this.Name = "HotkeyManager";
        this.ShowIcon = false;
        this.ShowInTaskbar = false;
        this.SizeGripStyle = SizeGripStyle.Hide;
        this.StartPosition = FormStartPosition.CenterScreen;
        this.Text = "Hotkey Input";
        this.TopMost = true;
        this.HotkeyFormBtnPanel.ResumeLayout(false);
        this.ResumeLayout(false);
        this.PerformLayout();
    }

    #endregion

    private TextBox HotkeyInputBox;
    private TableLayoutPanel HotkeyFormBtnPanel;
    private Button HotkeyFormClearBtn;
    public Button HotkeyFormOkBtn;
    public Button HotkeyFormCancelBtn;
}