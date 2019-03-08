using System;
using System.Windows.Forms;

namespace iFlag
{
    public partial class mainForm : Form
    {
        int overlaysOnDisplay = 0;
        string overlaysOnDisplayLabel = "";

                                                  // Overlays bit field constants
        const int if_spotterOverlay = 16;

        private void startOverlays()
        {
        }

                                                  // Try to match the overlay modules
                                                  // Returns bit field of matches.
        private int matchOverlays()
        {
            if (!sdk.IsConnected()) return 0;

            return 0
            + matchSpotterOverlay()
            ;
        }

                                                  // Try to match signals from the spotter
        private int matchSpotterOverlay()
        {
            if (!this.spotterOverlayModuleMenuItem.Checked) return 0;

            int carLeftRight = (int)sdk.GetData("CarLeftRight");

                 if (carLeftRight == SPOTTER_CAR_LEFT || carLeftRight == SPOTTER_CARS_LEFT) return overlay(if_spotterOverlay, "Left side!", WARN_L_OVERLAY, new byte[] { COLOR_BLACK, COLOR_PURPLE });
            else if (carLeftRight == SPOTTER_CAR_RIGHT || carLeftRight == SPOTTER_CARS_RIGHT) return overlay(if_spotterOverlay, "Right side!", WARN_R_OVERLAY, new byte[] { COLOR_BLACK, COLOR_PURPLE });
            else if (carLeftRight == SPOTTER_CARS_LEFT_RIGHT) return overlay(if_spotterOverlay, "Both sides!", WARN_LR_OVERLAY, new byte[] { COLOR_BLACK, COLOR_PURPLE });
            else return 0;
        }

                                                  // Pour the specified overlay into the overlay matrix awaiting boradcast
                                                  // logging the flags with time codes into console
                                                  // (^^ this might be eventually going into a file in the future.)
        public int overlay(int overlayID, string overlayName, byte[, ,] pattern, byte[] color)
        {
            overlaysOnDisplayLabel += String.Format(" +{0}", overlayName);
            patternToMatrix(ref overlayMatrix, pattern, color);
            return overlayID;
        }
    }
}
