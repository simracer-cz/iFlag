using System;
using System.Windows.Forms;

namespace iFlag
{
    public partial class mainForm : Form
    {
        int overlaysOnDisplay = 0;
        string overlaysOnDisplayLabel = "";

        bool showIncidentOverlay;

                                                  // Overlays bit field constants
        const int if_spotterOverlay = 16;
        const int if_incidentOverlay = 32;

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
            + matchIncidentOverlay()
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

                                                  // Shows an incident overlay along with the incident count
                                                  // in the UI message in case incident occurs
        private int matchIncidentOverlay()
        {
            if (!this.incidentOverlayModuleMenuItem.Checked) return 0;

            newIncidentCount = (int)sdk.GetData("PlayerCarTeamIncidentCount");
            int incidentGain = newIncidentCount - incidentCount;

            if (newIncidentCount > incidentCount)
            {
                incidentCount = newIncidentCount;
                showIncidentOverlay = true;
                incidentTimer.Start();
            }
            if (showIncidentOverlay == true)
            {
                return overlay(if_incidentOverlay, String.Format("Incident({0}x)", incidentGain), INCIDENT_OVERLAY, new byte[] { COLOR_BLACK, COLOR_RED });
            }
            else return 0;
        }

                                                  // Pour the specified overlay into the overlay matrix awaiting boradcast
                                                  // logging the flags with time codes into console
                                                  // (^^ this might be eventually going into a file in the future.)
        public int overlay(int overlayID, string overlayName, byte[, ,] pattern, byte[] color)
        {
            overlaysOnDisplayLabel += String.Format("+ {0}", overlayName);
            patternToMatrix(ref overlayMatrix, pattern, color);
            return overlayID;
        }

        private void incidentTimer_Tick(object sender, EventArgs e)
        {
            incidentTimer.Stop();
            showIncidentOverlay = false;
        }
    }
}
