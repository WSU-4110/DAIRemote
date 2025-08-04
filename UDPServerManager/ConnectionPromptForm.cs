namespace UDPServerManager;
public partial class ConnectionPromptForm : Form
{
    public DialogResult UserChoice { get; private set; } = DialogResult.No;

    public ConnectionPromptForm(string message, string caption, Dictionary<DialogResult, string> buttonText)
    {
        InitializeComponent();

        // Set form properties
        this.Text = caption;
        messageLabel.Text = message;

        // Configure buttons based on the provided text
        if (buttonText.ContainsKey(DialogResult.Yes))
        {
            allowButton.Text = buttonText[DialogResult.Yes];
            allowButton.Visible = true;
        }
        else
        {
            allowButton.Visible = false;
        }

        if (buttonText.ContainsKey(DialogResult.No))
        {
            denyButton.Text = buttonText[DialogResult.No];
            denyButton.Visible = true;
        }
        else
        {
            denyButton.Visible = false;
        }

        if (buttonText.ContainsKey(DialogResult.Cancel))
        {
            blockButton.Text = buttonText[DialogResult.Cancel];
            blockButton.Visible = true;
        }
        else
        {
            blockButton.Visible = false;
        }
    }

    public static DialogResult ShowConnectionPrompt(string ipAddress, string port, string name)
    {
        var buttonText = new Dictionary<DialogResult, string>
            {
                { DialogResult.Yes, "Allow" },
                { DialogResult.No, "Deny" },
                { DialogResult.Cancel, "Block" }
            };

        return ConnectionPromptForm.Show(
            $"Allow connection from {name}?\n{ipAddress}:{port}",
            "Pending Connection",
            buttonText);
    }

    private void allowButton_Click(object sender, EventArgs e)
    {
        UserChoice = DialogResult.Yes;
        this.Close();
    }

    private void denyButton_Click(object sender, EventArgs e)
    {
        UserChoice = DialogResult.No;
        this.Close();
    }

    private void blockButton_Click(object sender, EventArgs e)
    {
        UserChoice = DialogResult.Cancel;
        this.Close();
    }

    public static DialogResult Show(string text, string caption,
        Dictionary<DialogResult, string> buttonText)
    {
        using (var form = new ConnectionPromptForm(text, caption, buttonText))
        {
            form.ShowDialog();
            return form.UserChoice;
        }
    }
}
