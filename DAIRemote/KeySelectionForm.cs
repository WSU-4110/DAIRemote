using DAIRemote;
using System;
using System.Windows.Forms;

public class KeySelectionForm : Form
{
    private HotkeyManager hotkeyManager;

    public KeySelectionForm(HotkeyManager hotkeyManager)
    {
        this.hotkeyManager = hotkeyManager;
        this.Text = "Select a Hotkey";
        this.KeyPreview = true;
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.StartPosition = FormStartPosition.CenterScreen;
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);

        hotkeyManager.SetHotkey(e.KeyCode);

        MessageBox.Show($"Hotkey '{e.KeyCode}' set successfully!", "Hotkey Set", MessageBoxButtons.OK, MessageBoxIcon.Information);

        this.Close();
    }
}
