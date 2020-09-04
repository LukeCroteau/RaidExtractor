using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using Website.Models;

namespace Website.Repositories
{
    public interface IAccountDumpRepository
    {
        string Store(AccountDump accountDump);
        AccountDump Get(string key);
        void Cleanup();
    }

    public class AccountDumpRepository : IAccountDumpRepository
    {
        private static readonly char[] _characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789".ToCharArray();
        private readonly ConcurrentDictionary<string, AccountDumpRecord> _dumps;
        private readonly Random _random;

        public AccountDumpRepository()
        {
            _dumps = new ConcurrentDictionary<string, AccountDumpRecord>();
            _random = new Random();
        }

        private class AccountDumpRecord
        {
            public DateTimeOffset CreatedOn { get; set; }
            public AccountDump AccountDump { get; set; }
        }

        private string GenerateKey()
        {
            var key = new StringBuilder(16);
            for (var i = 0; i < 16; i++) key.Append(_characters[_random.Next(0, _characters.Length - 1)]);
            return key.ToString();
        }

        public string Store(AccountDump accountDump)
        {
            string key;
            do
            {
                key = GenerateKey();
            } while (_dumps.ContainsKey(key));

            _dumps[key] = new AccountDumpRecord
            {
                AccountDump = accountDump,
                CreatedOn = DateTimeOffset.UtcNow,
            };

            return key;
        }

        public AccountDump Get(string key)
        {
            return _dumps.TryGetValue(key, out var record) ? record.AccountDump : null;
        }

        public void Cleanup()
        {
            var remove = _dumps.Where(p => p.Value.CreatedOn < DateTimeOffset.UtcNow.AddHours(8)).Select(p => p.Key).ToArray();
            foreach (var key in remove) _dumps.TryRemove(key, out _);
        }
    }
}
