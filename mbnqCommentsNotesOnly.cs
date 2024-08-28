/*
 *  New ideas
 *  - toggle crouch to hold
 *  - anti-recoil
 *  - improve performance
 *  - fps
 *  - advanced settings
 *  - rotate crosshair
 * 
 * 
 * 
 */

/*
    // Path to the registry key where DPI settings are stored
    string dpiRegistryPath = @"HKEY_CURRENT_USER\Control Panel\Desktop\WindowMetrics";
    string valueName = "AppliedDPI";

    // Retrieve the DPI value from the registry
    object dpiValue = Registry.GetValue(dpiRegistryPath, valueName, 96);

    if (dpiValue != null)
    {
        int dpi = (int)dpiValue;

        // Calculate the scaling factor
        float scalingFactor = dpi / 96.0f;

        Console.WriteLine($"Scaling Factor New: {scalingFactor * 100}% {dpiValue}");
    }
    else
    {
        Console.WriteLine("Failed to retrieve DPI setting from registry.");
    }
*/


// Get the screen bounds (entire screen size)
// Calculate the centered X position

// Debug.WriteLine($"New Test: {(Screen.PrimaryScreen.Bounds.Width * 1.25)}");

// Debug.WriteLine($"New Test: {System.Windows.SystemParameters.FullPrimaryScreenWidth.ToString()}");
// Debug.WriteLine($"New Test: {System.Windows.SystemParameters.PrimaryScreenHeight.ToString()}");
// Debug.WriteLine($"Centered X: {centeredX}, Centered Y: {centeredY},  test: {Screen.PrimaryScreen.WorkingArea}");

// Calculate the center of the screen
// int centeredX = FormStartPosition.CenterScreen; // controlPanel.MainDisplay.Left + (zoomSizeSet / 2);
// int centeredY = controlPanel.MainDisplay.Top + (controlPanel.SizeValue);

/*
             Point TestLocation = new Point(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);

            foreach (var screen in Screen.AllScreens)
            {
                Debug.WriteLine($"Screen {screen.DeviceName}: {screen.Bounds.Width}x{screen.Bounds.Height}");
            }
*/

// using System.Diagnostics;

// Debug.WriteLineIf










