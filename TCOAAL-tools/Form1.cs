using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Security.Cryptography;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.RegularExpressions;
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

        public string VERSION = "v1.5.0";

        private bool hash_match = false;
        // TODO: Проверять не хешем, а дсон сериалайзером 
        static readonly HttpClient httpClient = new HttpClient();


        private byte[] pluginFile = null;
        private byte[] pluginsListFile = null;
        private byte[] prefsFile = null;
        private byte[] autosplitterFile = null;
        private string versionString = null;

        private const string LIVESPLIT_PLUGIN_VERSION = "1.5.0";
        private const string AUTOSPLITTER_VERSION = "1.5.0";
        private const string AUTOSPLITTER_SETTINGS_VERSION = "1.5.0";

        private string _autosplitter_version = "1.5.0";

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
                    System.Diagnostics.Debug.WriteLine(GetJsonVersion(prefsPath));
                    System.Diagnostics.Debug.WriteLine(HasLiveSplitPlugin(listPath));
                    System.Diagnostics.Debug.WriteLine(GetJSVersion(livesplitPath));
                    System.Diagnostics.Debug.WriteLine(GetJsonVersion(autosplitterPath));

                    if (GetJsonVersion(prefsPath) == AUTOSPLITTER_SETTINGS_VERSION)
                    {
                        
                        if (HasLiveSplitPlugin(listPath))
                        {
                            
                            if (GetJSVersion(livesplitPath) == LIVESPLIT_PLUGIN_VERSION)
                            {
                                
                                if (GetJsonVersion(autosplitterPath) == AUTOSPLITTER_VERSION)
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
                System.Diagnostics.Debug.WriteLine(versionResult.ToString());
                VersionLabel.Text = "Version Outdated";
                UpdateButton.Enabled = true;
                UpdateButton.Visible = true;
            }
            else
                VersionLabel.ForeColor = Color.Green; // Версия Совпадает
        }

        public static string? GetJSVersion(string filePath)
        {
            string content = File.ReadAllText(filePath);
            Match match = Regex.Match(content, @"//\s*@version\s+([\d.]+)");
            return match.Success ? match.Groups[1].Value : null;
        }
        public static string GetJsonVersion(string filePath)
        {
            string jsonString = File.ReadAllText(filePath);
            using var document = JsonDocument.Parse(jsonString);
            var root = document.RootElement;

            if (root.TryGetProperty("version", out JsonElement versionElement))
            {
                return versionElement.GetString();
            }
                
            return null;
        }
        public static bool HasLiveSplitPlugin(string filePath)
        {
            if (!File.Exists(filePath))
                return false;

            string jsFileContent = File.ReadAllText(filePath);

            // Находим объявление массива
            int start = jsFileContent.IndexOf("$plugins =");
            if (start == -1) return false;
            int bracketStart = jsFileContent.IndexOf('[', start);
            if (bracketStart == -1) return false;

            // Находим конец массива (последнюю ']' в файле)
            int bracketEnd = jsFileContent.LastIndexOf(']');
            if (bracketEnd == -1 || bracketEnd <= bracketStart) return false;

            string jsonArray = jsFileContent.Substring(bracketStart, bracketEnd - bracketStart + 1);

            try
            {
                using JsonDocument doc = JsonDocument.Parse(jsonArray);
                foreach (JsonElement plugin in doc.RootElement.EnumerateArray())
                {
                    if (plugin.ValueKind != JsonValueKind.Object) continue;

                    bool nameOk = plugin.TryGetProperty("name", out JsonElement nameEl)
                                  && nameEl.GetString() == "LiveSplit";

                    bool statusOk = plugin.TryGetProperty("status", out JsonElement statusEl)
                                    && statusEl.GetBoolean() == true;

                    bool descOk = plugin.TryGetProperty("description", out JsonElement descEl)
                                  && descEl.GetString() == "";

                    bool paramsOk = plugin.TryGetProperty("parameters", out JsonElement paramsEl)
                                    && paramsEl.ValueKind == JsonValueKind.Object
                                    && !paramsEl.EnumerateObject().Any();

                    if (nameOk && statusOk && descOk && paramsOk)
                        return true;
                }
                return false;
            }
            catch (JsonException)
            {
                return false;
            }
        }

        public static bool InsertLiveSplitPlugin(string filePath)
        {
            if (!File.Exists(filePath))
                return false;

            string fileContent = File.ReadAllText(filePath);

            // Ищем объявление массива
            int start = fileContent.IndexOf("$plugins =");
            if (start == -1) return false;
            int bracketStart = fileContent.IndexOf('[', start);
            if (bracketStart == -1) return false;

            // (Опционально) проверяем, нет ли уже плагина
            // if (HasLiveSplitPlugin(filePath)) return false;

            string pluginJson =
                @"{
              ""name"": ""LiveSplit"",
              ""status"": true,
              ""description"": """",
              ""parameters"": {}
            },";

            // Вставляем после открывающей скобки
            string newContent = fileContent.Insert(bracketStart + 1, "\n" + pluginJson + "\n");
            File.WriteAllText(filePath, newContent);
            return true;
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
            if (!File.Exists(prefsPath)) return;

            string json = File.ReadAllText(prefsPath);
            using var document = JsonDocument.Parse(json);
            var root = document.RootElement;

            // Читаем версию
            if (root.TryGetProperty("version", out JsonElement versionElement))
            {
                _autosplitter_version = versionElement.GetString() ?? "1.4.3";
            }
            else
            {
                _autosplitter_version = "1.4.3";
            }

            var splitPrefs = new Dictionary<string, bool>();

            foreach (JsonProperty property in root.EnumerateObject())
            {
                if (property.Name == "version") continue;

                // Добавляем только булевы значения
                if (property.Value.ValueKind == JsonValueKind.True ||
                    property.Value.ValueKind == JsonValueKind.False)
                {
                    splitPrefs[property.Name] = property.Value.GetBoolean();
                }
            }

            this.splitPrefs = splitPrefs;
            PopulateSplitsList();
        }

        private void WriteAutosplitter(string prefsPath)
        {
            // Создаём объект для сериализации
            var obj = new Dictionary<string, object>
            {
                ["version"] = _autosplitter_version
            };

            // Добавляем все настройки из словаря
            foreach (var kvp in splitPrefs)
            {
                obj[kvp.Key] = kvp.Value;
            }

            JsonSerializerOptions options = new JsonSerializerOptions
            {
                Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            string outputText = JsonSerializer.Serialize(obj, options);
            File.WriteAllText(prefsPath, outputText);
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

            if (GetJsonVersion(prefsPath) == AUTOSPLITTER_SETTINGS_VERSION)
            {

                if (HasLiveSplitPlugin(listPath))
                {

                    if (GetJSVersion(livesplitPath) == LIVESPLIT_PLUGIN_VERSION)
                    {

                        if (GetJsonVersion(autosplitterPath) == AUTOSPLITTER_VERSION)
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
