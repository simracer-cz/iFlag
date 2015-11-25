Signals
=======
_as defined in [flags.cs](../iFlag/flags.cs)_

A complete list of signals and other visual features of _iFlag_.


Mandatory
---------

### Racing Flags

| Visual                                                                | Signal   | Description ([wikipedia](https://en.wikipedia.org/wiki/Racings)) | Flag primitive |
| --------------------------------------------------------------------- | -------- | ----------- | -------- |
| ![](signals/green.gif)           | Green flag | The green flag signals a clear track to race on. | `SIMPLE` |
| ![](signals/red.gif)             | Red flag | The red flag means the race is stopped. | `FLASHING` |
| ![](signals/blue.gif)            | Blue flag | This flag encourages a driver to move aside to allow faster traffic to pass. | `DIAGONAL_STRIPE` |
| ![](signals/debris.gif)          | Debris flag | The red-striped yellow flag, also known as the "surface flag", indicates a potential traction hazard. | `STRIPPED` |
| ![](signals/yellow.gif)          | Yellow flag | The yellow flag means local caution. | `WAVING` |
| ![](signals/black.gif)           | Black flag | The black flag orders a particular driver into the pit area. | `INVERTED` |
| ![](signals/crossed.gif)         | Disqualification | This flag signals a car is no longer being scored. | `CROSSED` |
| ![](signals/furled-black.gif)    | Furled black flag | This flag indicates a penalty for bad conduct. | `FURLED` |
| ![](signals/meatball.gif)        | Meat ball flag | This flag indicates an internal hazard in a participant's vehicle. | `MEATBALL` |
| ![](signals/white.gif)           | White flag | The white flag signals that the final lap is in progress. | `SIMPLE` |
| ![](signals/checkered.gif)       | Checkered flag | The chequered flag is displayed at the start/finish line to indicate that the race is officially finished. | `CHECKERED` |


### Safety Car Procedure

| Visual                                                                | Signal   | Description | Flag primitive |
| --------------------------------------------------------------------- | -------- | ----------- | -------------- |
| ![](signals/sc.gif)              | Full course caution flag | Full course caution | `SAFETYCAR` |
| ![](signals/one-to-green.gif)    | One Lap to Green | | `INVERTED` |
| ![](signals/green.gif)           | Green, green, green! | The green flag signals end of the caution and restart of the race. | `SIMPLE` |



Optional
--------
These can be enabled or disabled in _iFlag_ options menu.

### Race Start Lights

| Visual                                                                | Signal   | Description | Flag primitive |
| --------------------------------------------------------------------- | -------- | ----------- | -------------- |
| ![](signals/start-ready.gif)     | Ready! | This represents all start lights off. | `HALF` |
| ![](signals/start-set.gif)       | Set! | This represents all red start lights on. | `SIMPLE` |
| ![](signals/green.gif)           | Go, go, go! | Clearly, this represents all green start lights on. | `SIMPLE` |


### Pit Stop Signals

| Visual                                                                | Signal   | Description | Flag primitive |
| --------------------------------------------------------------------- | -------- | ----------- | -------------- |
| ![](signals/pits-speedlimit.gif) | Pit speed limit |  | `CIRCLE` |
| ![](signals/pits-hold.gif)       | Hold! |  | `SQUARE` |
| ![](signals/pits-go.gif)         | Go! |  | `SQUARE` |
| ![](signals/pits-gogogo.gif)     | Go, go, go! |  | `SQUARE` |



Miscelanous
-----------
These are used for various system purposes.

| Visual                                                                | Signal   | Description | Flag primitive |
| --------------------------------------------------------------------- | -------- | ----------- | -------------- |
| ![](signals/f.gif)               | "F flag" | Displayed as greeting on iFlag startup. It is also used as a check pattern displayed when setting board orientation in the options menu. | `F` |
| ![](signals/iracing.gif)         | iRacing logo | This is actually not used for anything yet..  | `IRACING_LOGO` |


---
© 2015
[Petr.Vostřel.cz](http://petr.vostrel.cz),
[simracer.cz](http://simracer.cz),
[4xracing.co.uk](http://4xracing.co.uk)
