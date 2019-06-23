![iFLAG logo](http://simracer.cz/iracing/iFlag-logo/logo-full.svg)

Signals
=======
_as defined in [flags.cs](../iFlag/flags.cs) and  [overlays.cs](../iFlag/overlays.cs)_

A complete list of signals and other visual features of _iFLAG_.


Mandatory Modules
-----------------

### Racing Flags

| Visual                           | Signal   | Description ([wikipedia](https://en.wikipedia.org/wiki/Racings)) | Flag primitive |
| -------------------------------- | -------- | ----------- | -------- |
| ![](signals/green.gif)           | Green flag | The green flag signals a clear track to race on. | `SIMPLE` |
| ![](signals/red.gif)             | Red flag | The red flag means the race is stopped. | `FLASHING` |
| ![](signals/blue.gif)            | Blue flag | This flag encourages a driver to move aside to allow faster traffic to pass. | `DIAGONAL_STRIPE` |
| ![](signals/debris.gif)          | Debris flag | The red-striped yellow flag, also known as the "surface flag", indicates a potential traction hazard. | `STRIPPED` |
| ![](signals/yellow.gif)          | Yellow flag | The yellow flag means local caution. | `WAVING` |
| ![](signals/black.gif)           | Black flag | The black flag orders a particular driver into the pit area. | `INVERTED` |
| ![](signals/crossed.gif)         | Disqualify flag | This flag signals a car is no longer being scored. | `CROSSED` |
| ![](signals/furled-black.gif)    | Furled Black flag | This flag indicates a penalty for bad conduct. | `FURLED` |
| ![](signals/meatball.gif)        | Meat Ball flag | This flag indicates an internal hazard in a participant's vehicle. | `MEATBALL` |
| ![](signals/white.gif)           | White flag | The white flag signals that the final lap is in progress. | `SIMPLE` |
| ![](signals/checkered.gif)       | Checkered flag | The chequered flag is displayed at the start/finish line to indicate that the race is officially finished. | `CHECKERED` |


### Safety Car Procedure

| Visual                           | Signal   | Description | Flag primitive |
| -------------------------------- | -------- | ----------- | -------------- |
| ![](signals/sc.gif)              | Caution flag (road) | Full course caution on road track types | `SAFETYCAR` |
| ![](signals/caution.gif)         | Caution flag (other) | Full course caution on other track types | `FLASHING` |
| ![](signals/one-to-green.gif)    | One Lap to Green | | `INVERTED` |
| ![](signals/green.gif)           | Green, green, green! | The green flag signals end of the caution and restart of the race. | `SIMPLE` |



Optional Modules
----------------
These can be enabled or disabled in _iFLAG_ options menu.

### Race Start Lights

| Visual                           | Signal   | Description | Flag primitive |
| -------------------------------- | -------- | ----------- | -------------- |
| ![](signals/start-ready.gif)     | Ready! | This represents all start lights off. | `HALF` |
| ![](signals/start-set.gif)       | Set! | This represents all red start lights on. | `SIMPLE` |
| ![](signals/green.gif)           | Go, go, go! | Clearly, this represents all green start lights on. | `SIMPLE` |


### Spotter Overlay

| Visual                           | Signal   | Description | Flag primitive |
| -------------------------------- | -------- | ----------- | -------------- |
| ![](signals/warn-left.gif)       | + Car(s) left! | Spotter warning about one or more cars with overlap on the left side | `WARN_L` |
| ![](signals/warn-right.gif)      | + Car(s) right! | Spotter warning about one or more cars with overlap on the right side | `WARN_R` |
| ![](signals/warn-left-right.gif) | + Cars left and right! | Spotter warning about cars with overlap on both sides of the vehicle | `WARN_LR` |


### Incident Overlay

| Visual                           | Signal   | Description | Flag primitive |
| -------------------------------- | -------- | ----------- | -------------- |
| ![](signals/incident-small.gif) ![](signals/incident-big.gif) ![](signals/incident-exploded.gif) | + Incident (Xx) | Lights up for 3 seconds every incident count increases. Small, big or exploded X depending on an option | `CROSSED` |


### Pit Exit Blue

| Visual                           | Signal   | Description | Flag primitive |
| -------------------------------- | -------- | ----------- | -------------- |
| ![](signals/blue.gif)            | Blue flag | Shows on pit exit with faster car on track within 100 meter behind | `DIAGONAL_STRIPE` |


### Closed Pits Overlay

| Visual                                  | Signal   | Description | Flag primitive |
| --------------------------------------- | -------- | ----------- | -------------- |
| ![](signals/closed-pits.gif)            | + Pits Closed | Shows when pits are closed | `CORNERS` |
| ![](signals/entering-closed-pits.gif)   | + Entering Closed Pits! | Warning on pit entry when pits are closed | `CROSSED` |


### Pit Speed Limit

| Visual                                  | Signal   | Description | Flag primitive |
| --------------------------------------- | -------- | ----------- | -------------- |
| ![](signals/pit-speed-too-high.gif)     | Too fast! | Shows on pit entry & lane when coming in too hot | `TOO_HIGH` |
| ![](signals/pit-speed-high.gif)         | Little over limit | Shows on pit entry & lane when coming in just slightly over the limit | `TOO_HIGH` |
| ![](signals/pit-speed-enough.gif)       | On speed limit | Shows on pit entry & lane when coming in spot on speed limit | `ENOUGH` |
| ![](signals/pit-speed-low.gif)          | Little below limit | Shows on pit entry & lane when coming in just slightly below the limit | `TOO_LOW` |
| ![](signals/pit-speed-too-low.gif)      | Too slow! | Shows on pit entry & lane when coming in too cold | `TOO_LOW` |


### Repairs Progress

| Visual                                  | Signal   | Description | Flag primitive |
| --------------------------------------- | -------- | ----------- | -------------- |
| ![](signals/repairs-mandatory.gif)      | Mandatory Repairs | Being shown when mandatory repairs are progressing | `WRENCH` |
| ![](signals/repairs-optional.gif)       | Optional Repairs | Being shown when optional repairs are in progress | `WRENCH` |
| ![](signals/repairs-done.gif)           | Repairs Done | Being shown when all repairs are done | `WRENCH` |



Miscelanous
-----------
These are used for various system purposes.

| Visual                           | Signal   | Description | Flag primitive |
| -------------------------------- | -------- | ----------- | -------------- |
| ![](signals/f.gif)               | "F flag" | Displayed as greeting on _iFLAG_ startup. It is also used as a check pattern displayed when setting board orientation in the options menu. | `F` |
| ![](signals/iracing.gif)         | iRacing logo | This is actually not used for anything yet..  | `IRACING_LOGO` |


---
© 2015-2019
[Petr.Vostřel.cz](http://petr.vostrel.cz),
[simracer.cz](http://simracer.cz)
