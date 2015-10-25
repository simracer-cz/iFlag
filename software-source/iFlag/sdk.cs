using System;
using System.Windows.Forms;
using iRSDKSharp;

namespace iFlag
{
    public partial class mainForm : Form
    {
        iRacingSDK sdk = new iRacingSDK();

        // iRacing SDK flag constants
        const uint irsdk_checkered = 0x00000001;
        const uint irsdk_white = 0x00000002;
        const uint irsdk_green = 0x00000004;
        const uint irsdk_yellow = 0x00000008;
        const uint irsdk_red = 0x00000010;
        const uint irsdk_blue = 0x00000020;
        const uint irsdk_debris = 0x00000040;
        const uint irsdk_crossed = 0x00000080;
        const uint irsdk_yellowWaving = 0x00000100;
        const uint irsdk_oneLapToGreen = 0x00000200;
        const uint irsdk_greenHeld = 0x00000400;
        // const uint irsdk_tenToGo         = 0x00000800; // not displaying this one
        // const uint irsdk_fiveToGo        = 0x00001000; // not displaying this one
        // const uint irsdk_randomWaving    = 0x00002000; // not displaying this one
        const uint irsdk_caution = 0x00004000;
        const uint irsdk_cautionWaving = 0x00008000;
        const uint irsdk_black = 0x00010000;
        const uint irsdk_disqualify = 0x00020000;
        // const uint irsdk_servicible      = 0x00040000; // car is allowed service (not a flag)
        const uint irsdk_furled = 0x00080000;
        const uint irsdk_repair = 0x00100000;
        const uint irsdk_startHidden = 0x10000000;
        const uint irsdk_startReady = 0x20000000;
        const uint irsdk_startSet = 0x40000000;
        const uint irsdk_startGo = 0x80000000;
        
        private void startSDK()
        {
        }

        // Takes care of detecting a live iRacing session
        //
        private void detectSDK()
        {
            if (sdk.IsConnected() && sdk.VarHeaders.Count > 0)
            {
                indicateSimConnected(true);
            }
            else
            {
                indicateSimConnected(false);

                if (sdk.IsInitialized)
                {
                    sdk.Shutdown();
                }
                else
                {
                    sdk.Startup();
                }
            }
        }

    }
}
