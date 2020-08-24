using System;
using System.Windows.Forms;

namespace iFlag
{
    public partial class mainForm : Form
    {
        float[] carsLapPct;
        float carLapPct;
        bool[] carsOnPitRoad;
        int[] carsTrackSurface;
        int carSurface;
        float carsProximity;
        float speed;
        float kph = 1 / 3.600F;
        bool onPitRoad;
        bool onPitExitRoad;
        bool onPitEntryRoad;
        bool inPitStall;

        private void startCar()
        {
        }

        private void getCarDetails()
        {
            carsLapPct = (float[])sdk.GetData("CarIdxLapDistPct");
            carLapPct = carsLapPct[carID];

            carsOnPitRoad = (bool[])sdk.GetData("CarIdxOnPitRoad");
            carsTrackSurface = (int[])sdk.GetData("CarIdxTrackSurface");

            carSurface = carsTrackSurface[carID];

            carsProximity = -0.1F;
            for (var idx = 0; idx < carsLapPct.Length; idx++)
            if (idx != carID && idx != paceCarID && carsTrackSurface[ idx ] > 1)
            for (var i = 0; i <= 1; i++)
            {
                var distance = carsLapPct[ idx ] - carLapPct + i;
                if (distance < 0) carsProximity = Math.Max(carsProximity, distance);
            }
            carsProximity *= trackLength; // to meters

            onPitRoad = (bool)sdk.GetData("OnPitRoad");
            onPitExitRoad = !onPitRoad && carSurface == irsdk_AproachingPits && carLapPct < 0.5;
            onPitEntryRoad = !onPitRoad && carSurface == irsdk_AproachingPits && carLapPct > 0.5;
            inPitStall = carSurface == irsdk_InPitStall;

            speed = (float)sdk.GetData("Speed");
        }
    }
}
