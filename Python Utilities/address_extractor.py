'''
This is a quick, hacky implementation to get me usable memory addresses to test with.
Not expected to last long in production.
'''
import sys, os

findclasslist = {
    'public class AppModel : SingleInstance<AppModel>': [{
        'private UserWrapper _userWrapper;': '        public static int AppModelUserWrapper = {}; // AppModel._userWrapper\n',
    }],
    'public class UserWrapper ': [{
        'public readonly HeroesWrapper Heroes;': '        public static int UserWrapperHeroes = {}; // UserWrapper.Heroes\n',
        'public readonly ShardWrapper Shards;': '        public static int UserWrapperShards = {}; // UserWrapper.Shards\n',
        'public readonly ArenaWrapper Arena;': '        public static int UserWrapperArena = {}; // UserWrapper.Arena\n',
        'public readonly CapitolWrapper Capitol;': '        public static int UserWrapperCapitol = {}; // UserWrapper.Capitol\n',
    }],
    'public abstract class HeroesWrapperReadOnly ': [{
        'protected readonly UpdatableArtifactData ArtifactData;': '        public static int HeroesWrapperArtifactData = {}; // HeroesWrapperReadOnly.ArtifactData\n',
        'protected readonly UpdatableHeroData HeroData;': '        public static int HeroesWrapperHeroData = {}; // HeroesWrapperReadOnly.HeroData\n',
    }],
    'public abstract class ArenaWrapperReadOnly ': [{
        'protected Nullable<ArenaLeagueId> LeagueId;': '        public static int ArenaWrapperLeagueId = {}; // ArenaWrapperReadOnly.LeagueId\n',
    }],
    'public abstract class CapitolWrapperReadOnly ': [{
        'protected readonly UpdatableVillageData VillageData;': '        public static int CapitolWrapperVillageData = {}; // CapitolWrapperReadOnly.VillageData\n',
    }],
    'public class UserArtifactData ': [{
        'public List<Artifact> Artifacts;': '        public static int UserArtifactDataArtifacts = {}; // UserArtifactData.Artifactsa\n',
        'public Dictionary<int, HeroArtifactData> ArtifactDataByHeroId;': '        public static int UserArtifactArtifactDataByHeroId = {}; // UserArtifactData.ArtifactDataByHeroId\n',
    }],
    'public class UserHeroData ': [{
        'public Dictionary<int, Hero> HeroById;': '        public static int UserHeroDataHeroById = {}; // UserHeroData.HeroById\n',
    }],
    'public class UserVillageData ': [{
        'public Dictionary<Element, Dictionary<StatKindId, int>> CapitolBonusLevelByStatByElement;': '        public static int UserVillageDataCapitolBonusLevelByStatByElement = {}; // UserVillageData.UserVillageDataCapitolBonusLevelByStatByElement\n',
    }],
}

def find_address_in_script(lines, searchstr: str):
    result = 0
    for i in range(len(lines)):
        if searchstr in lines[i]:
            result = lines[i-1].split(': ')[1][:-1]
            break
    return result

def dump_class(vernum, metadata_path):
    metadata_path = os.path.join(metadata_path, '')    
    print('Metadata Full Path', metadata_path)
    outfile = open('../RaidExtractor.Core/Native/RaidStaticInformation.cs', 'w')
    outfile.write('namespace RaidExtractor.Core.Native\n')
    outfile.write('{\n')
    outfile.write('    class RaidStaticInformation\n')
    outfile.write('    {\n')
    outfile.write(str.format('        public static string ExpectedRaidVersion = "\\\\{}\\\\";\n', vernum))

    x = open(str.format('{}\\script.json', metadata_path), encoding='utf-8')
    lines = x.read().split('\n')
    x.close()

    memory_location = find_address_in_script(lines, 'SingleInstance<AppModel>.get_Instance()')
    outfile.write(str.format('        public static int MemoryLocation = {};\n', memory_location))

    external_storage = find_address_in_script(lines, 'ArtifactStorageResolver_TypeInfo')
    outfile.write(str.format('        public static int ExternalStorageAddress = {};\n', external_storage))

    x = open(str.format('{}\\dump.cs', metadata_path), encoding='utf-8')
    lines = x.read().split('\n')
    x.close()

    outfile.write('\n')

    classesfound = 0
    for i in range(len(lines)):
        for k, v in findclasslist.items():
            if k in lines[i]:
                classesfound += 1
                print(k, i)

                j = i + 1
                while lines[j].strip() != '}':
                    j += 1
                print('End line', j)

                for item in v:
                    subclassesfound = 0
                    for nk, nv in item.items():
                        for finder in range(i, j+1):
                            if nk in lines[finder]:
                                subclassesfound += 1
                                outfile.write(str.format(nv, lines[finder].split('// ')[1]))
                    if subclassesfound != len(item):
                        print(str.format('*** ERROR *** Expected {} subclasses, found {} subclasses.', len(item), subclassesfound))
                outfile.write('\n')            

    if classesfound != len(findclasslist):
        print(str.format('*** ERROR *** Expected {} classes, found {} classes.', len(findclasslist), classesfound))

    outfile.write('        public static int DictionaryEntries = 0x18; // Dictionary.Entries\n')
    outfile.write('        public static int DictionaryCount = 0x20; // Dictionary.Count\n')
    outfile.write('        public static int ListCount = 0x18; // List.Count\n')

    outfile.write('    }\n')
    outfile.write('}\n')
    outfile.close()

if __name__ == '__main__':
    if len(sys.argv) > 2:
        print('Version', sys.argv[1])
        print('Path to Metadata', sys.argv[2])
        dump_class(sys.argv[1], sys.argv[2])
    else:
        print('Please invoke this program with the Version first (e.g. 230) path to the Metadata.')
