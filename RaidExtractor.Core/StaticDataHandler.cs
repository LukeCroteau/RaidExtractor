using System;
using Microsoft.Win32;
using System.Collections.Generic;
#if DEV_BUILD
using Il2CppDumper;
#endif
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace RaidExtractor.Core
{

    public class StaticDataHandler
    {
#region Il2Cpp Attributes
        // Duplicated from the Il2CppDumper project as they are innaccessible there.
        public const int FIELD_ATTRIBUTE_STATIC = 0x0010;
        public const int TYPE_ATTRIBUTE_SEALED = 0x00000100;
#endregion
        public const uint IL2CPPMAGIC_PE = 0x905A4D;

        struct GameDataEntry
        {
            public enum DataType
            {
                FieldOffset,
                SystemMethod,
                SystemType
            };

            public DataType Type;
            public string Name; // The name by which this is referred to in the extraor code.
            public string ClassName; // The name of the class containing the method.
            public string MemberName; // The member, or type name.


            public GameDataEntry(DataType dataType, string name, string className, string memberName)
            {
                Type = dataType;
                Name = name;
                ClassName = className;
                MemberName = memberName;
            }
        };

        // List of class/member offsets to be accessed in the game.
        GameDataEntry[] SearchEntries =
        {
            new GameDataEntry(GameDataEntry.DataType.SystemMethod, "MemoryLocation", "Client.App.SingleInstance<AppModel>", "get_Instance"),
            new GameDataEntry(GameDataEntry.DataType.SystemType, "ExternalStorageAddress", "", "ArtifactStorageResolver"),

            new GameDataEntry(GameDataEntry.DataType.FieldOffset, "AppModelUserWrapper", "AppModel", "_userWrapper"),
            new GameDataEntry(GameDataEntry.DataType.FieldOffset, "UserWrapperHeroes", "UserWrapper", "Heroes"),
            new GameDataEntry(GameDataEntry.DataType.FieldOffset, "UserWrapperShards", "UserWrapper", "Shards"),
            new GameDataEntry(GameDataEntry.DataType.FieldOffset, "UserWrapperArena", "UserWrapper", "Arena"),
            new GameDataEntry(GameDataEntry.DataType.FieldOffset, "UserWrapperCapitol", "UserWrapper", "Capitol"),

            new GameDataEntry(GameDataEntry.DataType.FieldOffset, "HeroesWrapperArtifactData", "HeroesWrapperReadOnly", "ArtifactData"),
            new GameDataEntry(GameDataEntry.DataType.FieldOffset, "HeroesWrapperHeroData", "HeroesWrapperReadOnly", "HeroData"),

            new GameDataEntry(GameDataEntry.DataType.FieldOffset, "ArenaWrapperLeagueId", "ArenaWrapperReadOnly", "LeagueId"),

            new GameDataEntry(GameDataEntry.DataType.FieldOffset, "CapitolWrapperVillageData", "CapitolWrapperReadOnly", "VillageData"),

            new GameDataEntry(GameDataEntry.DataType.FieldOffset, "UserArtifactDataArtifacts", "UserArtifactData", "Artifacts"),
            new GameDataEntry(GameDataEntry.DataType.FieldOffset, "UserArtifactArtifactDataByHeroId", "UserArtifactData", "ArtifactDataByHeroId"),

            new GameDataEntry(GameDataEntry.DataType.FieldOffset, "UserHeroDataHeroById", "UserHeroData", "HeroById"),
            new GameDataEntry(GameDataEntry.DataType.FieldOffset, "UserHeroDataBattlePresets", "UserHeroData", "BattlePresets"),

            new GameDataEntry(GameDataEntry.DataType.FieldOffset, "UserVillageDataCapitolBonusLevelByStatByElement", "UserVillageData", "CapitolBonusLevelByStatByElement"),

            new GameDataEntry(GameDataEntry.DataType.FieldOffset, "ShardWrapperData", "ShardWrapperReadOnly", "ShardData"),

            new GameDataEntry(GameDataEntry.DataType.FieldOffset, "ShardData", "UserShardData", "Shards"),
            new GameDataEntry(GameDataEntry.DataType.FieldOffset, "ShardSummonData", "UserShardData", "SummonResults")
        };

        [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.1.24.0 (Newtonsoft.Json v11.0.0.0)")]
        public class DataValue
        {
            [Newtonsoft.Json.JsonProperty("name", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
            public string Name { get; set; }

            [Newtonsoft.Json.JsonProperty("value", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
            public int Value { get; set; }
        };

        [System.CodeDom.Compiler.GeneratedCode("NJsonSchema", "10.1.24.0 (Newtonsoft.Json v11.0.0.0)")]
        public class DataValues
        {
            [Newtonsoft.Json.JsonProperty("version", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]
            public string Version { get; set; }

            [Newtonsoft.Json.JsonProperty("values", Required = Newtonsoft.Json.Required.Default, NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore)]

            public System.Collections.Generic.ICollection<DataValue> Values { get; set; }
        };

        private readonly DataValues CurrentData;

        private Dictionary<string, object> CurrentValues = new Dictionary<string, object>();

        private static readonly StaticDataHandler instance = new StaticDataHandler();

        public StaticDataHandler()
        {
            try
            {
                // Load existing data values from embedded resource.
                var serializer = new JsonSerializer();
                var assembly = typeof(Extractor).Assembly;

                using (var stream = assembly.GetManifestResourceStream("RaidExtractor.Core.datavalues.json"))
                using (var sr = new StreamReader(stream))
                using (var textReader = new JsonTextReader(sr))
                {
                    CurrentData = serializer.Deserialize<DataValues>(textReader);
                }

                SetVariable("ExpectedRaidVersion", CurrentData.Version);
                foreach (DataValue v in CurrentData.Values)
                {
                    SetVariable(v.Name, v.Value);
                }
            }
            catch
            {
#if !DEV_BUILD
                // Don't throw exception for dev build as we can generate new values.
                throw new Exception("Unable to load static data values.");
#endif
            }
        }

        public static StaticDataHandler Instance
        {
            get
            {
                return instance;
            }
        }

        public static JsonSerializerSettings SerializerSettings = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented,
            ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy
                {
                    ProcessDictionaryKeys = true,
                    OverrideSpecifiedNames = false
                },
            },
        };

        public void UpdateValuesFromGame(string exePath)
        {
#if DEV_BUILD
            Il2Cpp il2Cpp;
            Il2CppExecutor executor;
            Metadata metadata;

            string il2CppPath;
            string metadataPath;

            string[] pathParts = exePath.Split('\\');
            string version = "\\" + pathParts[pathParts.Length - 2] + "\\";
            SetVariable("ExpectedRaidVersion", version);

            if (!FindGameFiles(Directory.GetParent(exePath).FullName, out il2CppPath, out metadataPath))
            {
                throw new Exception("Failed to find the game files.");
            }

            if (!ExtractCurrentData(il2CppPath, metadataPath, out il2Cpp, out executor, out metadata))
            {
                throw new Exception("Failed to extract the data from the game.");
            }

            DataValues newValues = new DataValues();
            newValues.Version = version;
            newValues.Values = new List<DataValue>();
            foreach (GameDataEntry entry in SearchEntries)
            {
                switch (entry.Type)
                {
                    case GameDataEntry.DataType.FieldOffset:
                        SetVariable(entry.Name, GetFieldOffset(il2Cpp, metadata, executor, entry.ClassName, entry.MemberName));
                        break;
                    case GameDataEntry.DataType.SystemMethod:
                        SetVariable(entry.Name, GetMethodAddress(il2Cpp, metadata, executor, entry.ClassName, entry.MemberName));
                        break;
                    case GameDataEntry.DataType.SystemType:
                        SetVariable(entry.Name, GetTypeInfoAddress(il2Cpp, metadata, executor, entry.MemberName));
                        break;
                }

                DataValue v = new DataValue();
                v.Name = entry.Name;
                v.Value = (int)GetValue(entry.Name);
                newValues.Values.Add(v);
            }

            string filename = Path.GetTempPath() + "\\datavalues.json";
            File.WriteAllText(filename, JsonConvert.SerializeObject(newValues, SerializerSettings));
            Console.WriteLine("Written class data to " + filename + ". Copy to porject directory before rebuilding.");
#else
            throw new Exception("Update required. Game version does not match expected version.");
#endif
        }


        public object GetValue(string name)
        {
            if (!CurrentValues.ContainsKey(name))
            {
                return null;
            }

            return CurrentValues[name];
        }

#if DEV_BUILD
        private bool FindGameFiles(string baseDir, out string il2CppPath, out string metadataPath)
        {
            il2CppPath = "";
            metadataPath = "";

            if (!Directory.Exists(baseDir))
            {
                return false;
            }

            il2CppPath = baseDir + "\\GameAssembly.dll";
            metadataPath = baseDir + "\\Raid_Data\\il2cpp_data\\Metadata\\global-metadata.dat";

            if (!File.Exists(il2CppPath) || !File.Exists(metadataPath))
            {
                return false;
            }

            return true;
        }

        private bool ExtractCurrentData(string il2cppPath, string metadataPath, out Il2Cpp il2Cpp, out Il2CppExecutor executor, out Metadata metadata)
        {
            il2Cpp = null;
            executor = null;
            metadata = null;
            var metadataBytes = File.ReadAllBytes(metadataPath);
            metadata = new Metadata(new MemoryStream(metadataBytes));

            var il2cppBytes = File.ReadAllBytes(il2cppPath);
            var il2cppMagic = BitConverter.ToUInt32(il2cppBytes, 0);
            var il2CppMemory = new MemoryStream(il2cppBytes);

            if (il2cppMagic != IL2CPPMAGIC_PE)
            {
                throw new Exception("Unexpected il2cpp magic number.");
            }

            il2Cpp = new PE(il2CppMemory);

            il2Cpp.SetProperties(metadata.Version, metadata.maxMetadataUsages);

            if (il2Cpp.Version >= 27 && il2Cpp is ElfBase elf && elf.IsDumped)
            {
                metadata.Address = Convert.ToUInt64(Console.ReadLine(), 16);
            }

            try
            {
                var flag = il2Cpp.PlusSearch(metadata.methodDefs.Count(x => x.methodIndex >= 0), metadata.typeDefs.Length, metadata.imageDefs.Length);
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    if (!flag && il2Cpp is PE)
                    {
                        il2Cpp = PELoader.Load(il2cppPath);
                        il2Cpp.SetProperties(metadata.Version, metadata.maxMetadataUsages);
                        flag = il2Cpp.PlusSearch(metadata.methodDefs.Count(x => x.methodIndex >= 0), metadata.typeDefs.Length, metadata.imageDefs.Length);
                    }
                }
                if (!flag)
                {
                    flag = il2Cpp.Search();
                }
                if (!flag)
                {
                    flag = il2Cpp.SymbolSearch();
                }
                if (!flag)
                {
                    var codeRegistration = Convert.ToUInt64(Console.ReadLine(), 16);
                    var metadataRegistration = Convert.ToUInt64(Console.ReadLine(), 16);
                    il2Cpp.Init(codeRegistration, metadataRegistration);
                }
            }
            catch
            {
                throw new Exception("ERROR: An error occurred while processing.");
            }

            executor = new Il2CppExecutor(metadata, il2Cpp);

            return true;
        }

        private int GetFieldOffset(Il2Cpp il2Cpp, Metadata metadata, Il2CppExecutor executor, string classname, string membername)
        {
            int typeDefIndex = 0;
            Il2CppTypeDefinition typeDef = FindClassEntry(metadata, executor, classname, out typeDefIndex);
            if (typeDef == null)
            {
                return 0;
            }

            var fieldEnd = typeDef.fieldStart + typeDef.field_count;
            for (var i = typeDef.fieldStart; i < fieldEnd; ++i)
            {
                var isStatic = false;
                var fieldDef = metadata.fieldDefs[i];
                var fieldType = il2Cpp.types[fieldDef.typeIndex];

                if ((fieldType.attrs & FIELD_ATTRIBUTE_STATIC) != 0)
                {
                    isStatic = true;
                }

                string memberName = metadata.GetStringFromIndex(fieldDef.nameIndex);
                if (memberName == membername)
                { 
                    return il2Cpp.GetFieldOffsetFromIndex(typeDefIndex, i - typeDef.fieldStart, i, typeDef.IsValueType, isStatic);
                }

            }

            throw new Exception("Field " + membername + " not found in " + classname);
        }

        private int GetMethodAddress(Il2Cpp il2Cpp, Metadata metadata, Il2CppExecutor executor, string methodType, string methodName)
        {
            foreach (var i in metadata.metadataUsageDic[Il2CppMetadataUsage.kIl2CppMetadataUsageMethodRef])
            {
                var methodSpec = il2Cpp.methodSpecs[i.Value];

                (var methodSpecTypeName, var methodSpecMethodName) = executor.GetMethodSpecName(methodSpec, true);

                if (methodSpecTypeName == methodType && methodSpecMethodName == methodName)
                {
                    return (int)il2Cpp.GetRVA(il2Cpp.metadataUsages[i.Key]);
                }
            }

            return 0;
        }

        private int GetTypeInfoAddress(Il2Cpp il2Cpp, Metadata metadata, Il2CppExecutor executor, string typeToFind)
        {
            foreach (var i in metadata.metadataUsageDic[Il2CppMetadataUsage.kIl2CppMetadataUsageTypeInfo])
            {
                var type = il2Cpp.types[i.Value];
                var typeName = executor.GetTypeName(type, true, false);

                if (typeName.Contains(typeToFind))
                {
                    return (int)il2Cpp.GetRVA(il2Cpp.metadataUsages[i.Key]);
                }
            }

            return 0;
        }

        private Il2CppTypeDefinition FindClassEntry(Metadata metadata, Il2CppExecutor executor, string className, out int typeDefIndex)
        {
            typeDefIndex = 0;
            foreach (var imageDef in metadata.imageDefs)
            {
                var imageName = metadata.GetStringFromIndex(imageDef.nameIndex);
                var typeEnd = imageDef.typeStart + imageDef.typeCount;

                for (typeDefIndex = imageDef.typeStart; typeDefIndex < typeEnd; typeDefIndex++)
                {
                    var typeDef = metadata.typeDefs[typeDefIndex];

                    // Ignore if the class is sealed.
                    if (!typeDef.IsValueType && !typeDef.IsEnum && (typeDef.flags & TYPE_ATTRIBUTE_SEALED) != 0)
                        continue;

                    var typeName = executor.GetTypeDefName(typeDef, false, true);

                    if (typeName == className)
                    {
                        return typeDef;
                    }
                }
            }

            throw new Exception("Class " + className + " not found.");
        }
#endif
        private void SetVariable(string name, object value)
        {
            if (!CurrentValues.ContainsKey(name))
            {
                CurrentValues.Add(name, value);
            }
            CurrentValues[name] = value;
        }
    }
}
