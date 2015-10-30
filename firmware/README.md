iFlag Firmware
==============

	v0.16

To activate the Arduino board, you will need to use Arduino IDE to compile the firmware and flash the Arduino UNO's memory with it, so that the blank generic board becomes an _iFlag_ board capable of displaying instructions send to it.

1. Get, install and run the official [ _Arduino IDE_ ](https://www.arduino.cc/en/Main/Software). It's free!
2. Get [ _Colorduino Library for Arduino IDE_ ](https://github.com/lincomatic/Colorduino)
  - Choose _"Add Library"_ from _Sketch_ > _Import Library_ menu of the IDE.
  - Select the downloaded Colorduino library ZIP file.
3. Open `firmware.ino` in the IDE.
4. Connect the Arduino board to the USB port.
  - Choose _"Arduino UNO"_ from _Tools_ > _Board_ menu of the IDE.
  - Choose the actual port from _Tools_ > _Port_ menu of the IDE.
5. Click on _"Upload"_ button in the IDE toolbar on the top (it's the second from the left).

These steps will cause the source code to compile and get uploaded to the Arduino board which will permanently flash its memory with it.

	SCREENSHOT OF THE IDE


Self Test
---------

After you've successfully completed these steps, you should see a self test routine running every time you plug in (or reset) the board. The routine is there to test out the individual LED chips of the matrix allowing you to inspect and asses the condition of your matrix. The firmware will briefly light up two of the three color chips inside each LED at the same time, which results in the matrix lighting up with solid pink, teal and cyan colors in close succession.

	VIDEO



Next Step
---------

With the device alive the next step is to install [ _iFlag_ Software ](../software/README.md) to your computer.


---
© 2015
[ Petr.Vostřel.cz ](http://petr.vostrel.cz),
[ simracer.cz ](http://simracer.cz),
[ 4xracing.co.uk ](http://4xracing.co.uk)

