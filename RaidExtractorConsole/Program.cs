using System;
using System.IO;
using System.IO.Compression;
using CommandLine;
using RaidExtractor.Core;
using Newtonsoft.Json;

namespace RaidExtractorConsole
{
    class Program
    {
        public class Options
        {
            [Option('o', "output", Required = false, Default = "artifacts.json",
              HelpText = "Destination output file name.\nDefaults to artifacts.json")]
            public string OutputFile { get; set; }

            [Option('t', "type", Required = false, Default = "json", HelpText = "Output Type: 'json' for JSON file output, 'zip' for ZIP file output.")]
            public string DumpType { get; set; }
        }

        static void Main(string[] args)
        {
            var options = new Options();

            CommandLine.Parser.Default.ParseArguments<Options>(args)
                .WithParsed<Options>(o =>
                {
                    Extractor raidExtractor = new Extractor();
                    AccountDump dump;
                    try
                    {
                        dump = raidExtractor.GetDump();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"There was an error during Extraction: {ex.Message}");
                        return;
                    }

                    var outFile = o.OutputFile;

                    var json = JsonConvert.SerializeObject(dump, Formatting.Indented);
                    if (o.DumpType.ToLower() == "zip")
                    {
                        if (!outFile.ToLower().Contains("zip")) outFile += ".zip";
                        File.Delete(outFile);

                        using (var memoryStream = new MemoryStream())
                        {
                            using (ZipArchive archive = ZipFile.Open(outFile, ZipArchiveMode.Create))
                            {
                                var artifactFile = archive.CreateEntry("artifacts.json");

                                using (var entryStream = artifactFile.Open())
                                {
                                    using (var streamWriter = new StreamWriter(entryStream))
                                    {
                                        streamWriter.Write(json);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        if (o.DumpType.ToLower() != "json") Console.WriteLine("Unknown Output type. Outputting file in JSON format.");
                        File.WriteAllText(outFile, json);
                    }
                    Console.WriteLine($"Output file {outFile} has been created.");
                });
        }
    }
}
