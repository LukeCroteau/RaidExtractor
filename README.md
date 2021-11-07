# RaidExtractor
A tool made to extract information from the windows version of "Raid: Shadow Legends". Currently it supports v.253 (as seen in Plarium Play launcher).

# RAIDEXTRACTOR IS BEING DEPRECATED... FOR A NEW VERSION
We've worked really hard to integrate RaidExtractor into the Raid Toolkit family of software and tools.

Please check out the new [Raid Extractor](https://github.com/raid-toolkit/extractor) here.

This repository will soon go dormant!

## Current Version instructions.

This application has 2 Modes:
* A Windows GUI application. The functionality is slim, but it works!
* A Non-GUI mode, which simply outputs the files as required.
  * Include **--nogui** or **-g** when running the application to run without a GUI.
  * Use **-o "output file name and path"** to specify the output file.
  * Use **-t "zip/json"** to pick the output mode. By default, this is set to **json**
  * If No Parameters are specified other than **--nogui/-g**, the application will create an *artifacts.json* file right where the application is run.

## How to use the "Last Champions" feature.
Please see the Pull Request [Here](https://github.com/LukeCroteau/RaidExtractor/pull/59#issue-622569910) for information on how to use the "Last Champions"/Battle Presets feature.

## Requirements
* .NET Framework 4.8

## Future versions
* Account stats
* Add a Wiki

## Projects Using Extracted Data!
Several other projects already exist that like to use the data this project extracts!

* [Brago](https://laughing-engelbart-62bcb5.netlify.app/) - Basic Raid Gear Observer. Recommends artifacts to sell and enchant.
* [Raid Tools](https://raidtools.club/) - This tool lets you import your data in an attempt to give you a better look at what you have, and what you need!
* [Raidtool Calculator](https://github.com/Jekoh497/RaidShadowLegend) - This Excel Spreadsheet helps use your artifacts and attempts to generate Clan Boss timings for you.
* [RaidChamps](https://raidchamps.com/) - This tool gives you an interactive sort and filter on your champion statuses, information about their skills, etc. Also a page for sharing your teams including all their total stats, artifacts, completed sets, masteries and leader's aura!

## Feedback
Please use the Github Issues page for this project, located at https://github.com/LukeCroteau/RaidExtractor/issues for any currently known issues, or to report any bugs!
