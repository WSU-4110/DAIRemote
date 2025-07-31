namespace UDPServerManager;

partial class ConnectionPromptForm
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
        System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConnectionPromptForm));
        this.allowButton = new Button();
        this.denyButton = new Button();
        this.blockButton = new Button();
        this.messageLabel = new Label();
        this.tableLayoutPanel1 = new TableLayoutPanel();
        this.tableLayoutPanel1.SuspendLayout();
        this.SuspendLayout();
        // 
        // allowButton
        // 
        this.allowButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        this.allowButton.Location = new Point(3, 3);
        this.allowButton.Name = "allowButton";
        this.allowButton.Size = new Size(63, 23);
        this.allowButton.TabIndex = 0;
        this.allowButton.Text = "Allow";
        this.allowButton.UseVisualStyleBackColor = true;
        this.allowButton.Click += this.allowButton_Click;
        // 
        // denyButton
        // 
        this.denyButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
        this.denyButton.Location = new Point(86, 3);
        this.denyButton.Name = "denyButton";
        this.denyButton.Size = new Size(75, 23);
        this.denyButton.TabIndex = 1;
        this.denyButton.Text = "Deny";
        this.denyButton.UseVisualStyleBackColor = true;
        this.denyButton.Click += this.denyButton_Click;
        // 
        // blockButton
        // 
        this.blockButton.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
        this.blockButton.Location = new Point(353, 3);
        this.blockButton.Name = "blockButton";
        this.blockButton.Size = new Size(75, 23);
        this.blockButton.TabIndex = 2;
        this.blockButton.Text = "Block";
        this.blockButton.UseVisualStyleBackColor = true;
        this.blockButton.Click += this.blockButton_Click;
        // 
        // messageLabel
        // 
        this.messageLabel.AutoSize = true;
        this.messageLabel.Font = new Font("Segoe UI", 16F);
        this.messageLabel.Location = new Point(12, 9);
        this.messageLabel.Name = "messageLabel";
        this.messageLabel.Size = new Size(0, 30);
        this.messageLabel.TabIndex = 3;
        // 
        // tableLayoutPanel1
        // 
        this.tableLayoutPanel1.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        this.tableLayoutPanel1.AutoSize = true;
        this.tableLayoutPanel1.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        this.tableLayoutPanel1.ColumnCount = 3;
        this.tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 19.35484F));
        this.tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 48.3871F));
        this.tableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 32.25806F));
        this.tableLayoutPanel1.Controls.Add(this.blockButton, 2, 0);
        this.tableLayoutPanel1.Controls.Add(this.allowButton, 0, 0);
        this.tableLayoutPanel1.Controls.Add(this.denyButton, 1, 0);
        this.tableLayoutPanel1.Location = new Point(12, 77);
        this.tableLayoutPanel1.Name = "tableLayoutPanel1";
        this.tableLayoutPanel1.RowCount = 1;
        this.tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        this.tableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
        this.tableLayoutPanel1.Size = new Size(431, 29);
        this.tableLayoutPanel1.TabIndex = 4;
        // 
        // ConnectionPromptForm
        // 
        this.AutoScaleDimensions = new SizeF(7F, 15F);
        this.AutoScaleMode = AutoScaleMode.Font;
        this.AutoSize = true;
        this.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        this.ClientSize = new Size(458, 118);
        this.Controls.Add(this.tableLayoutPanel1);
        this.Controls.Add(this.messageLabel);
        this.Icon = (Icon)resources.GetObject("$this.Icon");
        this.MaximizeBox = false;
        this.Name = "ConnectionPromptForm";
        this.ShowInTaskbar = false;
        this.StartPosition = FormStartPosition.CenterScreen;
        this.Text = "ConnectionPromptForm";
        this.tableLayoutPanel1.ResumeLayout(false);
        this.ResumeLayout(false);
        this.PerformLayout();
    }

    #endregion

    private Button allowButton;
    private Button denyButton;
    private Button blockButton;
    private Label messageLabel;
    private TableLayoutPanel tableLayoutPanel1;
}