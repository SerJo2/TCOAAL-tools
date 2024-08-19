using System.IO;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Channels;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace TCOAAL_tools
{
    public partial class Form1 : Form
    {
        string category;
        private FolderBrowserDialog folderBrowser;
        private string selectedPath;
        private string prefsPath;

        private Dictionary<string, bool> splitPrefs;
        private Autosplitter autosplitter;

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            folderBrowser = new FolderBrowserDialog();
            folderBrowser.Description = "Select the directory where the target game exe is located";
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void Any_CheckedChanged(object sender, EventArgs e)
        {
            category = "Any%";
        }

        private void Incest_CheckedChanged(object sender, EventArgs e)
        {
            category = "Incest%";
        }

        private void AllAchiv_CheckedChanged(object sender, EventArgs e)
        {
            category = "All Achivements";
        }

        private void OpenGame_Click(object sender, EventArgs e)
        {
            DialogResult dialog = folderBrowser.ShowDialog();
            if (dialog == DialogResult.OK)
            {
                string listPath = folderBrowser.SelectedPath + @"\www\js\plugins.js";
                string pluginsDirPath = folderBrowser.SelectedPath + @"\www\js\plugins";

                if (!Directory.Exists(pluginsDirPath) || !File.Exists(listPath))
                {
                    ShowError(new Exception("Not a valid game folder"));
                }
                else
                {
                    prefsPath = folderBrowser.SelectedPath + @"\AutosplitterSettings.json";
                    LoadAutosplitter(prefsPath);
                    Save.Enabled = true;
                    Any.Enabled = true;
                    Incest.Enabled = true;
                    AllAchiv.Enabled = true;
                }

            }
        }

        private void LoadAutosplitter(string prefsPath)
        {
            if (File.Exists(prefsPath))
            {
                string prefsText = File.ReadAllText(prefsPath);
                splitPrefs = JsonSerializer.Deserialize<Dictionary<string, bool>>(prefsText);
                PopulateSplitsList();
            }
        }

        private void WriteAutosplitter(string prefsPath)
        {
            JsonSerializerOptions options = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            // Write settings
            string outputText = JsonSerializer.Serialize(splitPrefs, options);
            StreamWriter writer = new StreamWriter(prefsPath);
            writer.Write(outputText);
            writer.Close();
        }

        private void PopulateSplitsList()
        {
            foreach (var split in splitPrefs)
            {
                if (split.Value)
                {
                    switch (split.Key)
                    {
                        case "Any%":
                            Any.Checked = true;
                            break;
                        case "Incest%":
                            Incest.Checked = true;
                            break;
                        case "All Achivements":
                            AllAchiv.Checked = true;
                            break;
                    }
                }
                
            }
            
        }

        private void ShowError(Exception ex)
        {
            MessageBox.Show(ex.Message + "\n\n" + ex.StackTrace, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            Console.WriteLine(ex.StackTrace);
        }


        private void Save_Click(object sender, EventArgs e)
        {
            switch (category)
            {
                case "Any%":
                    splitPrefs["Any%"] = true;
                    splitPrefs["Incest%"] = false;
                    splitPrefs["All achivements"] = false;
                    break;
                case "Incest%":
                    splitPrefs["Any%"] = false;
                    splitPrefs["Incest%"] = true;
                    splitPrefs["All achivements"] = false;
                    break;
                case "All Achivements":
                    splitPrefs["Any%"] = false;
                    splitPrefs["Incest%"] = false;
                    splitPrefs["All achivements"] = true;
                    break;


            }
            try
            {
                WriteAutosplitter(prefsPath);
                MessageBox.Show("Success");
            }
            catch (Exception ex)
            {
                // error :(
                ShowError(ex);
            }
        }
    }
}
