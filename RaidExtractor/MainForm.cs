using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Windows.Forms;
using Newtonsoft.Json;
using RaidExtractor.Core;

namespace RaidExtractor
{
    public partial class MainForm : Form
    {
        private readonly Extractor raidExtractor;

        public MainForm()
        {
            InitializeComponent();
            raidExtractor = new Extractor();
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            var result = GetDump();
            if (result == null) return;
            if (SaveJSONDialog.ShowDialog() != DialogResult.OK) return;

            File.WriteAllText(SaveJSONDialog.FileName, JsonConvert.SerializeObject(result, Formatting.Indented));
        }

        private async void UploadButton_Click(object sender, EventArgs e)
        {
            var result = GetDump();
            if (result == null) return;

            var client = new AccountClient(new HttpClient());
            client.BaseUrl = AppSettings.Default.BaseUrl;

            var key = await client.UploadAsync(result);
            Process.Start(AppSettings.Default.BaseUrl + "/account/" + key);
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
