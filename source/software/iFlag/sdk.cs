using System;
using System.Windows.Forms;
using iRSDKSharp;

namespace iFlag
{
    public partial class mainForm : Form
    {
        iRacingSDK sdk = new iRacingSDK();

        // iRacing SDK flag constants
        const long irsdk_checkered = 0x00000001;
        const long irsdk_white = 0x00000002;
        const long irsdk_green = 0x00000004;
        const long irsdk_yellow = 0x00000008;
        const long irsdk_red = 0x00000010;
        const long irsdk_blue = 0x00000020;
        const long irsdk_debris = 0x00000040;
        const long irsdk_crossed = 0x00000080;
        const long irsdk_yellowWaving = 0x00000100;
        const long irsdk_oneLapToGreen = 0x00000200;
        const long irsdk_greenHeld = 0x00000400;
        // const long irsdk_tenToGo         = 0x00000800; // not displaying this one
        // const long irsdk_fiveToGo        = 0x00001000; // not displaying this one
        // const long irsdk_randomWaving    = 0x00002000; // not displaying this one
        const long irsdk_caution = 0x00004000;
        const long irsdk_cautionWaving = 0x00008000;
        const long irsdk_black = 0x00010000;
        const long irsdk_disqualify = 0x00020000;
        // const long irsdk_servicible      = 0x00040000; // car is allowed service (not a flag)
        const long irsdk_furled = 0x00080000;
        const long irsdk_repair = 0x00100000;
        const long irsdk_startHidden = 0x10000000;
        const long irsdk_startReady = 0x20000000;
        const long irsdk_startSet = 0x40000000;
        const long irsdk_startGo = 0x80000000;
        
        // This value is so funky, I'd better make a comment on it.
        // It is a value of SDK's SessionFlags for no flags at all.
        // Am guessing it is just a random number, similar to `flagOnDisplay` here.
        const long irsdk_noFlag = 268697600;

        // iRacing CarLeftRight signal constants
        const int irsdk_LROff = 0; 
        const int irsdk_LRClear = 1;
        const int irsdk_LRCarLeft = 2;
        const int irsdk_LRCarRight = 3;
        const int irsdk_LRCarLeftRight = 4;
        const int irsdk_LR2CarsLeft = 5;
        const int irsdk_LR2CarsRight = 6;

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
