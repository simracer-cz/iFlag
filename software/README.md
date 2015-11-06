iFlag Software
==============

	v0.53

_iFlag_ software translates provides the interconnection of iRacing and the matrix. It continuously listens for iRacing session and other signals, processes them and issues commands telling the device exactly what to display
on the LED matrix.

The software doesn't come with any installer, it is only an EXE you run. Copy the `software/iFlag` folder to your harddrive anywhere you like (except Program Files folders) and run it from there. It is useful to make a shortcut in Startup items to have iFlag ready as soon as you start the computer.

![Screenshot of the software window](screenshot.png)


* __Options__ button lets you specify a few _iFlag_ settings explained further.
* __"White flag"__ is the name of the currently shown flag/signal.
* __Matrix__ visually indicates USB device connection status, green being connected and red otherwise, with connection details (device firmware version and port) to the right of it
* __iRacing__ visually indicates running iRacing session, green with iRacing launched and red otherwise.


Operating Instructions
----------------------

_iFlag_ will scan for the LED matrix hardware (this may take a few seconds). Once it is found, the red indicator labeled "Matrix" in the lower left corner will turn green to indicate the hardware readiness and that you are ready to run.
