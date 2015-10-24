using System;
using System.Windows.Forms;
using iRSDKSharp;

namespace iFlag
{
    public partial class mainForm : Form
    {
        iRacingSDK sdk = new iRacingSDK();
        
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
