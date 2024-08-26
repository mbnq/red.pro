/* 
    www.mbnq.pl 2024 
    mbnq00 on gmail

    New file for global functions
*/

using System.Windows.Forms;
using System.Drawing;
public static class mbFunctions
{

    public struct PointCoordinates
    {
        public int X { get; }
        public int Y { get; }

        public PointCoordinates(int x, int y)
        {
            X = x;
            Y = y;
        }
    }
    // Public static method to get the center point of the primary screen
    public static PointCoordinates mGetPrimaryScreenCenter()
    {
        // Get the primary screen
        Screen primaryScreen = Screen.PrimaryScreen;

        // Get the working area of the primary screen (excludes taskbar)
        Rectangle workingArea = primaryScreen.Bounds;

        // Calculate the center point
        int centerX = workingArea.Left + workingArea.Width / 2;
        int centerY = workingArea.Top + workingArea.Height / 2;

        // Return a new PointCoordinates instance
        return new PointCoordinates(centerX, centerY);
    }

}