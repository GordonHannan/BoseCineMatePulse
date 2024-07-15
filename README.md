# BoseCineMatePulse

**BoseCineMatePulse** is a Windows tray utility designed to keep your Bose CineMate Home Theatre Speaker System (or other similar speakers) from going to sleep due to inactivity.

## Problem
The Bose CineMate speakers automatically go to sleep after a certain period of audio inactivity. This utility helps keep the speakers active by generating short inaudible frequencies.

## How It Works
The program generates inaudible tones at 1 Hz, 2 Hz, and 3 Hz. These tones are played periodically to prevent the speakers from going to sleep. The application runs in the system tray and automatically plays the wake-up sound every 20 minutes. You can also manually trigger the wake-up sound by clicking the tray icon.

## Setup and Usage

1. **Installation**:
    - **Build using VS Code**:
        ```sh
        dotnet publish -c Release -r win-x64 --self-contained
        ```
    - **Alternative Method**:
        - Run the `publish.bat` file. This will compile and zip the application into 2 files located in `/bin/Publish`.
        - Copy the files to a folder of your choice and run the `.exe` file.

2. **Running the Application**:
    - Ensure the application is running. It will appear as a tray icon.
    - The wake-up sound will play every 20 minutes to keep the speakers active.
    - You can manually trigger the wake-up sound by clicking the tray icon.

3. **Auto-Launch on Startup**:
    - Add the application to your PC's startup programs to ensure it runs automatically when your PC starts.

## Notes
- This utility is a fork of [BoseCineMatePulse](https://github.com/drittich/BoseCineMatePulse).
- The wake-up interval is currently set to 20 minutes. Bose speakers typically need a wake-up signal every 15 minutes, but this has not been extensively timed.

### Personal Setup
I have my Bose CineMate speakers connected to a composite audio to 3.5mm audio cable, which is then hooked into a Bluetooth adapter ([Bluetooth Adapter](https://www.amazon.ca/gp/product/B078J3GTRK/ref=ppx_yo_dt_b_search_asin_title?ie=UTF8&th=1)) and paired to my PC.
