# RaidExtractor
A tool made to extract information from the windows version of "Raid: Shadow Legends". Currently it supports v0.230 (as seen in Plarium Play launcher) and currently only extracts artifacts and current champions.

This application has 2 Modes:
* A Windows GUI application. The functionality is slim, but it works!
* A Non-GUI mode, which simply outputs the files as required.
  * Include **--nogui** or **-n** when running the application to run without a GUI.
  * Use **-o "output file name and path"** to specify the output file.
  * Use **-t "zip/json"** to pick the output mode. By default, this is set to **json**
  * If No Parameters are specified other than **--nogui/-n**, the application will create an *artifacts.json* file right where the application is run.

## Requirements
* .NET Framework 4.8

## Future versions
* Add Great Hall and Arena data

## Projects Using Extracted Data!
Several other projects already exist that like to use the data this project extracts!

* [Brago ](https://laughing-engelbart-62bcb5.netlify.app/) - Basic Raid Gear Observer. Recommends artifacts to sell and enchant.
* Raid Tools - https://raidtools.club/ - This tool lets you import your data in an attempt to give you a better look at what you have, and what you need!
* Raidtool Calculator - https://github.com/Jekoh497/RaidShadowLegend - This Excel Spreadsheet helps use your artifacts and attempts to generate Clan Boss timings for you.

## Feedback
Please use the Github Issues page for this project, located at https://github.com/LukeCroteau/RaidExtractor/issues for any currently known issues, or to report any bugs!
