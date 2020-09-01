using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ProcessMemoryUtilities.Managed;
using ProcessMemoryUtilities.Native;
using RaidExtractor.Native;

namespace RaidExtractor
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void SaveButton_Click(object sender, EventArgs e)
        {
            var process = Process.GetProcessesByName("Raid").FirstOrDefault();
            if (process == null)
            {
                MessageBox.Show("Raid needs to be running before running RaidExtractor", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var handle = NativeWrapper.OpenProcess(ProcessAccessFlags.Read, true, process.Id);
            try
            {
                var gameAssembly = process.Modules.OfType<ProcessModule>().FirstOrDefault(m => m.ModuleName == "GameAssembly.dll");
                if (gameAssembly == null)
                {
                    MessageBox.Show("Unable to locate GameAssembly.dll in memory", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                var klass = IntPtr.Zero;
                NativeWrapper.ReadProcessMemory(handle, gameAssembly.BaseAddress + 0x2FD67D0, ref klass);

                var appModel = klass;
                NativeWrapper.ReadProcessMemory(handle, appModel + 0x18, ref appModel);
                NativeWrapper.ReadProcessMemory(handle, appModel + 0xC0, ref appModel);
                NativeWrapper.ReadProcessMemory(handle, appModel + 0x0, ref appModel);
                NativeWrapper.ReadProcessMemory(handle, appModel + 0xB8, ref appModel);
                NativeWrapper.ReadProcessMemory(handle, appModel + 0x8, ref appModel);

                var userWrapper = appModel;
                NativeWrapper.ReadProcessMemory(handle, userWrapper + 0x140, ref userWrapper);

                var artifactsPointer = userWrapper;
                NativeWrapper.ReadProcessMemory(handle, artifactsPointer + 0x38, ref artifactsPointer);
                NativeWrapper.ReadProcessMemory(handle, artifactsPointer + 0x48, ref artifactsPointer);
                NativeWrapper.ReadProcessMemory(handle, artifactsPointer + 0x20, ref artifactsPointer);

                var artifactCount = 0;
                NativeWrapper.ReadProcessMemory(handle, artifactsPointer + 0x18, ref artifactCount);

                var arrayPointer = artifactsPointer;
                NativeWrapper.ReadProcessMemory(handle, artifactsPointer + 0x10, ref arrayPointer);

                var pointers = new IntPtr[artifactCount];
                NativeWrapper.ReadProcessMemoryArray(handle, arrayPointer + 0x20, pointers);

                var artifacts = new JArray();
                var artifactStruct = new ArtifactStruct();
                var artifactBonusStruct = new ArtifactBonusStruct();
                var bonusValueStruct = new BonusValueStruct();
                foreach (var pointer in pointers)
                {
                    NativeWrapper.ReadProcessMemory(handle, pointer, ref artifactStruct);
                    NativeWrapper.ReadProcessMemory(handle, artifactStruct.PrimaryBonus, ref artifactBonusStruct);
                    NativeWrapper.ReadProcessMemory(handle, artifactBonusStruct.Value, ref bonusValueStruct);

                    var artifact = new JObject();
                    artifacts.Add(artifact);

                    artifact["id"] = artifactStruct.Id;
                    artifact["sellPrice"] = artifactStruct.SellPrice;
                    artifact["price"] = artifactStruct.Price;
                    artifact["level"] = artifactStruct.Level;
                    artifact["isActivated"] = artifactStruct.IsActivated;
                    artifact["kind"] = artifactStruct.KindId.ToString();
                    artifact["rank"] = artifactStruct.RankId.ToString();
                    artifact["rarity"] = artifactStruct.RarityId.ToString();
                    artifact["setKind"] = artifactStruct.SetKindId.ToString();
                    if (artifactStruct.RequiredFraction != HeroFraction.Unknown)
                        artifact["requiredFraction"] = artifactStruct.RequiredFraction.ToString();
                    artifact["isSeen"] = artifactStruct.IsSeen;
                    artifact["failedUpgrades"] = artifactStruct.FailedUpgrades;

                    artifact.Add("primaryBonus", new JObject());
                    artifact["primaryBonus"]["kind"] = artifactBonusStruct.KindId.ToString();
                    artifact["primaryBonus"]["isAbsolute"] = bonusValueStruct.IsAbsolute;
                    artifact["primaryBonus"]["value"] = Math.Round(bonusValueStruct.Value / (double)uint.MaxValue, 2);

                    var bonusesPointer = artifactStruct.SecondaryBonuses;
                    var bonusCount = 0;
                    NativeWrapper.ReadProcessMemory(handle, bonusesPointer + 0x18, ref bonusCount);
                    NativeWrapper.ReadProcessMemory(handle, bonusesPointer + 0x10, ref bonusesPointer);

                    artifact.Add("secondaryBonuses", new JArray());

                    var bonuses = new IntPtr[bonusCount];
                    if (bonusCount > 0) NativeWrapper.ReadProcessMemoryArray(handle, bonusesPointer + 0x20, bonuses, 0, bonuses.Length);

                    foreach (var bonusPointer in bonuses)
                    {
                        var bonus = new JObject();
                        ((JArray)artifact["secondaryBonuses"]).Add(bonus);

                        NativeWrapper.ReadProcessMemory(handle, bonusPointer, ref artifactBonusStruct);
                        NativeWrapper.ReadProcessMemory(handle, artifactBonusStruct.Value, ref bonusValueStruct);

                        bonus["kind"] = artifactBonusStruct.KindId.ToString();
                        bonus["isAbsolute"] = bonusValueStruct.IsAbsolute;
                        bonus["value"] = Math.Round(bonusValueStruct.Value / (double)uint.MaxValue, 2);
                        bonus["enhancement"] = Math.Round(artifactBonusStruct.PowerUpValue / (double)uint.MaxValue, 2);
                        bonus["level"] = artifactBonusStruct.Level;
                    }
                }

                if (SaveJSONDialog.ShowDialog() != DialogResult.OK) return;

                File.WriteAllText(SaveJSONDialog.FileName, artifacts.ToString(Formatting.Indented));
            }
            finally
            {
                NativeWrapper.CloseHandle(handle);
            }
        }
    }
}
