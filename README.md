# BoseCineMatePulse

A Windows tray utility for keeping Bose CineMate Home Theatre Speaker System from going to sleep.

My old CineMate speaker go to sleep after a certain period of audio inactivity. This is a utility to help keep the speakers active.

This program works by playing a short inaudible 10Hz tone periodically to keep the speakers awake. It just needs to stay running - there is not much to control,
so it simply runs as a tray icon.

The wake-up sound will be played every 20 minutes, but you can invoke it manually by clicking the tray icon.

Fork of https://github.com/drittich/BoseCineMatePulse., I believe Bose needs it every 15 minutes but I haven't sat here and timed it yet.

Build in VS Code using:
dotnet publish -c Release -r win-x64 --self-contained

Alternatively, run the publish.bat file and it will complile and zip into 2 files in /bin/Publish. You can then copy the files to a folder of your choice and run the .exe file.
