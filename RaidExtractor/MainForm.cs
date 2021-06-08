using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Windows.Forms;
using Newtonsoft.Json;
using RaidExtractor.Core;
using System.IO.Compression;
using Newtonsoft.Json.Serialization;

namespace RaidExtractor
{
    public partial class MainForm : Form
    {
        private readonly Extractor raidExtractor;

        public MainForm()
        {
            InitializeComponent();
            this.Icon = Properties.Resources.AppIcon;
            raidExtractor = new Extractor();
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            var result = GetDump();
            if (result == null) return;
            result.FileVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version.ToString(2);
            if (SaveJSONDialog.ShowDialog() != DialogResult.OK) return;

            try
            {
                File.WriteAllText(SaveJSONDialog.FileName, JsonConvert.SerializeObject(result, Program.SerializerSettings));

                if (SaveZipFile.Checked)
                {
                    File.Delete(Path.ChangeExtension(SaveJSONDialog.FileName, ".ZIP"));

                    using (var memoryStream = new MemoryStream())
                    {
                        using (ZipArchive archive = ZipFile.Open(Path.ChangeExtension(SaveJSONDialog.FileName, ".ZIP"), ZipArchiveMode.Create))
                        {
                            var artifactFile = archive.CreateEntry("artifacts.json");

                            using (var entryStream = artifactFile.Open())
                            {
                                using (var streamWriter = new StreamWriter(entryStream))
                                {
                                    streamWriter.Write(JsonConvert.SerializeObject(result, Formatting.Indented, Program.SerializerSettings));
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private AccountDump GetDump()
        {
            try
            {
                return raidExtractor.GetDump();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
        }
    }
}
