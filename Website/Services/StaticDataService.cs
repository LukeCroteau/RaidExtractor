using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Website.Models;

namespace Website.Services
{
    public class StaticDataService : BackgroundService
    {
        private readonly SemaphoreSlim _lock;

        private DateTime _lastUpdate;
        private Dictionary<string, string> _localization = null;

        public StaticDataService()
        {
            _lock = new SemaphoreSlim(1,1);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromHours(4), stoppingToken);
                await RefreshData();
            }            
        }

        private async Task RefreshData()
        {
            await _lock.WaitAsync();
            try
            {
                if (DateTime.UtcNow.Subtract(_lastUpdate) < TimeSpan.FromHours(3.9)) return;

                var data = await RequestData();
                var json = JObject.Parse(data);

                var localization = json["StaticDataLocalization"].ToObject<Dictionary<string, string>>();
                var heroTypes = json["HeroData"]["HeroTypes"].ToObject<List<HeroType>>().ToDictionary(h => h.Id);
                var skillTypes = json["SkillData"]["SkillTypes"].ToObject<List<SkillType>>().ToDictionary(h => h.Id);

                _lastUpdate = DateTime.UtcNow;
            }
            catch
            {
                // Failed
                return;
            }
            finally
            {
                _lock.Release();
            }
        }

        private async Task<string> RequestData()
        {
            var fileInfo = new FileInfo("static_data.json");
            if (fileInfo.Exists && DateTime.UtcNow.Subtract(fileInfo.LastWriteTimeUtc) < TimeSpan.FromHours(3.9)) return await File.ReadAllTextAsync("static_data.json");

            using var client = new WebClient();
            var data = await client.DownloadStringTaskAsync("https://raw.githubusercontent.com/Da-Teach/RaidStaticData/master/static_data.json");

            await File.WriteAllTextAsync("static_data.json", data);
            return data;
        }
    }
}
