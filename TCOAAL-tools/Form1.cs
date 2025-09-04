using System;
using System.Diagnostics;
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

        public string VERSION = "v1.4.1";

        private bool hash_match = false;
        // TODO: Проверять не хешем, а дсон сериалайзером 
        static readonly HttpClient httpClient = new HttpClient();
        private const string AUTOSPLITTER_SHA256 = "cd68642216432147bb79c8a6392428d439823ebb316334d2878fefa7ea83052d";
        private const string LIVESPLIT_SHA256 = "a053284d552c2a31a883155d474b508041e1b3217f027f6938856cf337a10e1c";
        private const string PLUGINS_SHA256 = "8c030a8f8e010b330f98be1fe783cbf5fc83dacc06016ef18a9476d0eaf52b9c";
        private string[] AUTOSPLITTERSETTINGS_SHA256 = ["b0eaf64e22042257a4018003c20fd381d30968ab0b774f19672e5cced503c6b0", "fb9e70c7a5c7ee65ac3a7f67d0ba0cf21089b3fb5d5c0d8c430443932492fb73", "ae99fa614921146729202668120cb7abe7707eb3086eaa1dcfebd36cb1b61aa3", "0e59bf9f01c752871a2263745099e6e11a0eb6b52b28f5b55263d8bf4f1773e8", "89a8190290a2d86ced97ac13a977274b90783160811e09d215b0fb4c43521ecf"];

        private byte[] pluginFile = null;
        private byte[] pluginsListFile = null;
        private byte[] prefsFile = null;
        private byte[] autosplitterFile = null;
        private string versionString = null;

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
        private const string VERSION_URL = "https://raw.githubusercontent.com/SerJo2/TCOAAL-tools/refs/heads/master/TCOAAL-tools/Utils/version.txt";

        private Dictionary<string, bool> splitPrefs;
        private Autosplitter autosplitter;

        public Form1()
        {
            InitializeComponent();
            RetrievePlugin();
            VersionLabel.Text = VERSION;
            CheckVersion();
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
        private void Burial_CheckedChanged(object sender, EventArgs e)
        {
            category = "Burial";
        }

        private void Incest_CheckedChanged(object sender, EventArgs e)
        {
            category = "Incest%";
        }

        private void Andy_CheckedChanged(object sender, EventArgs e)
        {
            category = "Andy Route";
        }

        private void AllAchiv_CheckedChanged(object sender, EventArgs e)
        {
            category = "AllAchiv";
        }

        private void OpenGame_Click(object sender, EventArgs e)
        {
            DialogResult dialog = folderBrowser.ShowDialog();
            if (dialog == DialogResult.OK)
            {
                listPath = Path.Combine(folderBrowser.SelectedPath, @"www\js\plugins.js");
                pluginsDirPath = Path.Combine(folderBrowser.SelectedPath, @"www\js\plugins");
                livesplitPath = Path.Combine(folderBrowser.SelectedPath, @"www\js\plugins\LiveSplit.js");
                autosplitterPath = Path.Combine(folderBrowser.SelectedPath, @"Autosplitter.json");
                prefsPath = Path.Combine(folderBrowser.SelectedPath, @"AutosplitterSettings.json");

                if (!Directory.Exists(pluginsDirPath) || !File.Exists(listPath))
                {
                    ShowError(new Exception("Not a valid game folder"));
                }
                else
                {

                    selectedPath = folderBrowser.SelectedPath;
                    open = true;

                    // #TODO поместить все в одно if и убрать первый if нахуй проверять конфиг, прросто проверь есть ли он или нет хз, перезапиши его рил хз
                    if (CalculateSHA256(prefsPath) == AUTOSPLITTERSETTINGS_SHA256[0] || CalculateSHA256(prefsPath) == AUTOSPLITTERSETTINGS_SHA256[1] || CalculateSHA256(prefsPath) == AUTOSPLITTERSETTINGS_SHA256[2] || CalculateSHA256(prefsPath) == AUTOSPLITTERSETTINGS_SHA256[3] || CalculateSHA256(prefsPath) == AUTOSPLITTERSETTINGS_SHA256[4])
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

                    hash_match = true;

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
                        Burial.Enabled = true;
                        Incest.Enabled = true;
                        Andy.Enabled = true;
                        AllAchiv.Enabled = true;
                    }
                    else
                    {
                        InstallPlugin.Enabled = true;
                    }
                }
            }
            else
            {
                
            }
        }
        private void LoadAllPlugins(string prefsPath)
        {

        }

        private async void CheckVersion()
        {
            try
            {
                versionString = await httpClient.GetStringAsync(VERSION_URL);

            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }

            string vComp = VERSION.Trim().TrimStart('v', 'V');
            string vServer = versionString.Trim().TrimStart('v', 'V');
            int versionResult = 0;

            string[] parts1 = vComp.Split('.');
            string[] parts2 = vServer.Split('.');

            int maxLength = Math.Max(parts1.Length, parts2.Length);

            for (int i = 0; i < maxLength; i++)
            {

                int num1 = (i < parts1.Length) ? int.Parse(parts1[i]) : 0;
                int num2 = (i < parts2.Length) ? int.Parse(parts2[i]) : 0;

                if (num1 != num2)
                    versionResult = num1.CompareTo(num2);
            }

            if (versionResult != 0)
            {
                VersionLabel.ForeColor = Color.Red; // Версия На компе otdated
                VersionLabel.Text = "Version Outdated";
                UpdateButton.Enabled = true;
                UpdateButton.Visible = true;
            }
            else
                VersionLabel.ForeColor = Color.Green; // Версия Совпадает
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
                        case "Burial":
                            Burial.Checked = true;
                            break;
                        case "Incest%":
                            Incest.Checked = true;
                            break;
                        case "Andy Route":
                            Andy.Checked = true;
                            break;
                        case "AllAchiv":
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
                    splitPrefs["Burial"] = false;
                    splitPrefs["Incest%"] = false;
                    splitPrefs["Andy Route"] = false;
                    splitPrefs["AllAchiv"] = false;
                    break;
                case "Burial":
                    splitPrefs["Any%"] = false;
                    splitPrefs["Burial"] = true;
                    splitPrefs["Incest%"] = false;
                    splitPrefs["Andy Route"] = false;
                    splitPrefs["AllAchiv"] = false;
                    break;
                case "Incest%":
                    splitPrefs["Any%"] = false;
                    splitPrefs["Burial"] = false;
                    splitPrefs["Incest%"] = true;
                    splitPrefs["Andy Route"] = false;
                    splitPrefs["AllAchiv"] = false;
                    break;
                case "Andy Route":
                    splitPrefs["Any%"] = false;
                    splitPrefs["Burial"] = false;
                    splitPrefs["Incest%"] = false;
                    splitPrefs["Andy Route"] = true;
                    splitPrefs["AllAchiv"] = false;
                    break;
                case "AllAchiv":
                    splitPrefs["Any%"] = false;
                    splitPrefs["Burial"] = false;
                    splitPrefs["Incest%"] = false;
                    splitPrefs["Andy Route"] = false;
                    splitPrefs["AllAchiv"] = true;
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
                Burial.Enabled = true;
                Incest.Enabled = true;
                Andy.Enabled = true;
                AllAchiv.Enabled = true;
                InstallPlugin.Enabled = false;
            }
            else
            {
                InstallPlugin.Enabled = true;
            }

        }

        private void VersionLabel_Click(object sender, EventArgs e)
        {

        }

        private void UpdateButton_Click(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process.Start(new ProcessStartInfo
                {
                    FileName = "https://github.com/SerJo2/TCOAAL-tools/releases",
                    UseShellExecute = true
                });
            }
            catch (System.ComponentModel.Win32Exception noBrowser)
            {
                if (noBrowser.ErrorCode == -2147467259)
                    MessageBox.Show(noBrowser.Message);
            }
            catch (System.Exception other)
            {
                MessageBox.Show(other.Message);
            }
        }
    }
}
