using System.Drawing;
using System.IO;
using System.Security.Cryptography;
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

        private bool hash_match = false;

        static readonly HttpClient httpClient = new HttpClient();
        private const string AUTOSPLITTER_SHA256 = "3eeeac0123b15c1d98b2f73db168d0468f7eeebb64b6d8b84ae6e84ad433c7a0";
        private const string LIVESPLIT_SHA256 = "ae26a9faedfdb5f1c33a5d92220d6b491506b0974e9a943e5f83f79fe3841e9c";
        private const string PLUGINS_SHA256 = "8ee19b7dd412c3d0b39e2c60de043f13ec79a0468e22b3af193dd87f47f7be7a";
        private string[] AUTOSPLITTERSETTINGS_SHA256 = ["105d52365ea927fb6e5ef3420f67a73078b2ce588fbab1a93969803d66873646", "736bf7209ecbd03ad98623d9fef629258823fc36ae0c0fbf8bcf8751625b559f", "2011681dbc88d26f65a0882e4a10a52a68a036f52353c2032e8f4be5df3c927c"];

        private byte[] pluginFile = null;
        private byte[] pluginsListFile = null;
        private byte[] prefsFile = null;
        private byte[] autosplitterFile = null;

        private bool open = false;
        private bool loadingStatus = false;

        string listPath;
        string livesplitPath;
        string autosplitterPath;
        string pluginsDirPath;

        private const string PLUGIN_URL = "https://raw.githubusercontent.com/SerJo2/TCoAaL-Autosplitter/refs/heads/main/www/js/plugins/LiveSplit.js";
        private const string PLUGIN_LIST_URL = "https://raw.githubusercontent.com/SerJo2/TCoAaL-Autosplitter/refs/heads/main/www/js/plugins.js";
        private const string PREFS_URL = "https://raw.githubusercontent.com/SerJo2/TCoAaL-Autosplitter/refs/heads/main/AutosplitterSettings.json";
        private const string AUTOSPLITTER_URL = "https://raw.githubusercontent.com/SerJo2/TCoAaL-Autosplitter/refs/heads/main/Autosplitter.json";


        private Dictionary<string, bool> splitPrefs;
        private Autosplitter autosplitter;

        public Form1()
        {
            InitializeComponent();
            RetrievePlugin();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            folderBrowser = new FolderBrowserDialog();
            folderBrowser.Description = "Select the directory where the target game exe is located";
        }

        static string CalculateSHA256(string filename)
        {
            using (var sha256 = SHA256.Create())
            {
                if (!File.Exists(filename))
                {
                    return "";
                }
                using (var stream = File.OpenRead(filename))
                {
                    var hash = sha256.ComputeHash(stream);
                    return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                }
            }
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void Any_CheckedChanged(object sender, EventArgs e)
        {
            category = "Any%";
        }

        private void OpenGame_Click(object sender, EventArgs e)
        {
            DialogResult dialog = folderBrowser.ShowDialog();
            if (dialog == DialogResult.OK)
            {
                listPath = folderBrowser.SelectedPath + @"\www\js\plugins.js";
                pluginsDirPath = folderBrowser.SelectedPath + @"\www\js\plugins";
                livesplitPath = folderBrowser.SelectedPath + @"\www\js\plugins\LiveSplit.js";
                autosplitterPath = folderBrowser.SelectedPath + @"\Autosplitter.json";
                prefsPath = folderBrowser.SelectedPath + @"\AutosplitterSettings.json";

                if (!Directory.Exists(pluginsDirPath) || !File.Exists(listPath))
                {
                    ShowError(new Exception("Not a valid game folder"));
                }
                else
                {
                    

                    selectedPath = folderBrowser.SelectedPath;
                    open = true;

                    // #TODO поместить все в одно if и убрать первый if нахуй проверять конфиг, прросто проверь есть ли он или нет хз, перезапиши его рил хз
                    if (CalculateSHA256(prefsPath) == AUTOSPLITTERSETTINGS_SHA256[0])
                    {
                        if (CalculateSHA256(listPath) == PLUGINS_SHA256)
                        {
                            if (CalculateSHA256(livesplitPath) == LIVESPLIT_SHA256)
                            {
                                if (CalculateSHA256(autosplitterPath) == AUTOSPLITTER_SHA256)
                                {
                                    hash_match = true;
                                }
                            }
                        }
                    }

                    if (hash_match)
                    {
                        GameStatus.Text = "Plugin installed";
                        GameStatus.ForeColor = Color.Green;
                    }
                    else
                    {
                        GameStatus.Text = "Plugin not installed";
                        GameStatus.ForeColor = Color.Red;
                    }

                    if (hash_match)
                    {
                        LoadAutosplitter(prefsPath);
                        Save.Enabled = true;
                        Any.Enabled = true;
                    }
                    else
                    {
                        InstallPlugin.Enabled = true;
                    }



                }

            }
        }
        private void LoadAllPlugins(string prefsPath)
        {

        }
        private async void RetrievePlugin()
        {
            try
            {
                pluginFile = await httpClient.GetByteArrayAsync(PLUGIN_URL);
                pluginsListFile = await httpClient.GetByteArrayAsync(PLUGIN_LIST_URL);
                prefsFile = await httpClient.GetByteArrayAsync(PREFS_URL);
                autosplitterFile = await httpClient.GetByteArrayAsync(AUTOSPLITTER_URL);
                loadingStatus = true;
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine(ex.Message);
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
                    break;
                


            }
            try
            {
                WriteAutosplitter(prefsPath);
                MessageBox.Show("Success");
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        

        private void InstallPlugin_Click(object sender, EventArgs e)
        {
            if (!loadingStatus)
            {
                MessageBox.Show("Wait a few seconds... Connecting to github server");
                return;
            }
            File.WriteAllBytes(livesplitPath, pluginFile);
            File.WriteAllBytes(listPath, pluginsListFile);
            File.WriteAllBytes(prefsPath, prefsFile);
            File.WriteAllBytes(autosplitterPath, autosplitterFile);

            if (CalculateSHA256(prefsPath) == AUTOSPLITTERSETTINGS_SHA256[0] || CalculateSHA256(prefsPath) == AUTOSPLITTERSETTINGS_SHA256[1] || CalculateSHA256(prefsPath) == AUTOSPLITTERSETTINGS_SHA256[2])
            {
                if (CalculateSHA256(listPath) == PLUGINS_SHA256)
                {
                    if (CalculateSHA256(livesplitPath) == LIVESPLIT_SHA256)
                    {
                        if (CalculateSHA256(autosplitterPath) == AUTOSPLITTER_SHA256)
                        {
                            hash_match = true;
                        }
                    }
                }
            }

            if (hash_match)
            {
                GameStatus.Text = "Plugin installed";
                GameStatus.ForeColor = Color.Green;
            }
            else
            {
                GameStatus.Text = "Plugin not installed";
                GameStatus.ForeColor = Color.Red;
            }

            if (hash_match)
            {
                LoadAutosplitter(prefsPath);
                Save.Enabled = true;
                Any.Enabled = true;
                InstallPlugin.Enabled = false;
            }
            else
            {
                InstallPlugin.Enabled = true;
            }

        }
    }
}
