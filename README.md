***

### Program Overview: RED. PRO
**RED. PRO (Red Dot Pro)** is a tool designed for customizing on-screen overlays, crosshairs, and other visual elements for gaming or other screen-based activities. The program offers a variety of settings that allow users to create and fine-tune custom visual elements according to their preferences. Below is a breakdown of the key features:

![NVIDIA_Overlay_RJJdebx7yy](https://github.com/user-attachments/assets/9304d970-be01-46d6-ab04-1272c0e27280)

Tested in Windows 10/11 at:
- 1080p
- 1440p

#### Key Features:

1. **Crosshair Overlay**
2. **Custom .png Crosshair Overlay**
3. **Zoom Mode (aka Sniper Mode)**
4. **Glass Elements**
5. **Save/Load Settings System**

[Download from GitHub](https://github.com/mbnq/red.pro/releases) (.zip file)

[Example .png crosshairs](https://github.com/mbnq/red.pro/tree/master/png)

#### 1.) Detailed Features:

1. **Color Customization**:
   - **Red, Green, Blue (RGB) Sliders**: Adjust the RGB values to set the exact color for the overlay or crosshair.
   
2. **Size Adjustment**:
   - **Size Slider**: Modify the size of the overlay or crosshair to match the needs of different screen resolutions or user preferences.

3. **Transparency Control**:
   - **Transparency Slider**: Control the opacity of the overlay or crosshair, allowing for either subtle or prominent on-screen elements.

4. **Offset Configuration**:
   - **Offset X and Y Sliders**: Fine-tune the position of the overlay or crosshair on the screen by adjusting its horizontal (X) and vertical (Y) offsets.

5. **Refresh Rate Setting**:
   - **Refresh Rate Slider**: Set how frequently the overlay or crosshair updates on the screen, with adjustable intervals measured in milliseconds.

6. **Custom PNG Overlay**:
   - **Custom PNG Overlay**: Players can load custom crosshairs in transparent or non-transparent .png format. Each loaded .png file is copied to the userdata folder, and a file hash is calculated for each file to prevent duplicate storage.

7. **Glass Overlay**:
   - **Glass Overlay Elements**: Players can cut any region of the primary screen, similar to how the Windows screenshot tool works, to spawn a glass item that mirrors and displays the selected screen area in real-time. Additional options, such as zoom, offset, transparency, etc., can be adjusted. Players can spawn as many glass elements as needed; however, their options are not saved in userdata yet.

8. **Zoom Mode (aka Sniper Mode)**:
   - **Zoom Mode (aka Sniper Mode)**: Hold the right mouse button for more than 2 seconds to activate a sniper zoom overlay in the bottom-right corner of the primary screen. This feature is a tribute to the old Delta Force games. It can be toggled on/off, and the zoom level can be adjusted with a dedicated slider.

![NVIDIA_Overlay_Fkaxvz0hji](https://github.com/user-attachments/assets/e8f8645a-24ae-4174-8b97-5f08efe3c246)

![NVIDIA_Overlay_5g2mpavDJd](https://github.com/user-attachments/assets/294b423a-6ddb-4dad-8578-1a89205c30b0)

---

This program is still in development; bugs and glitches may occur. 
I'm a C# beginner and hobbyst, so any feedback would be very appreciated.

The performance of the program will depend on the CPU performance. 

Requirements: Windows 11 with [.NET 4.8 x64](https://dotnet.microsoft.com/en-us/download/dotnet-framework).

---

Dependencies:
  - MaterialSkin (https://licenses.nuget.org/MIT)
  - stoker_1 (https://www.shutterstock.com/pl/g/stoker_1)
