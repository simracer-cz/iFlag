Power
=====

The light performance of the matrix is voltage-dependent, where higher is better. USB power while having enough punch (mA), it is only 5V, which is not enough to properly feed the matrix. Low voltage like that causes the LED chips to "starve", which results in their reduced brightness. Less demanding chip colors like red will perform better when under-powered and this manifest itself with a red-ish hue color shift usually clearly visible on signals like a white flag for example.

Providing an external DC power ensures enough voltage for the matrix to shine (literaly!).


What Adapter?
-------------

The overall power consumtion of the unit sits only at around 200mA with all LED chips at full brightness white and around 100mA at with all LEDs completely off. Bench measurements show that to get a uncompromised color quality, mainly to get the white to be white, you need a little more than the 7V required by Arduino alone.

For _iFlag_ use DC adapter of following rating:

| Adapter Parameter  | Absolute Minimum        | Recommended |
| ------------------ | ----------------------- | ----------- |
| Voltage (U)        |                      9V |   10V - 12V |
| Max Current (I)    |                   200mA |       300mA |
| Connector Plug     |             5.0 x 2.1mm |       _N/A_ |
| Polarity           | `+` inside, `-` outside |       _N/A_ |

__Pay attention to the connector polarity!__


Adapters of these ratings can often be salvaged out of discarded electronics like modems, toys, steering wheels...



Power Consumption
-----------------

Following table contains bench measurements of power draw on various voltages made to determine an adequate voltage range for the _iFlag_ device.

| DC Voltage | Full Draw | Idle Draw | White white? |
| ---------- | --------- | --------- | -------------|
| 7V         |     110mA |      85mA | NO (red)     |
| 8V         |     145mA |      99mA | NO (orange)  |
| 9V         |     180mA |      99mA | YES (barely) |
| 10V        |     190mA |      99mA | YES          |
| 11V        |     200mA |      99mA | YES          |
| 12V        |     205mA |      99mA | YES          |


---
© 2015
[Petr.Vostřel.cz](http://petr.vostrel.cz),
[simracer.cz](http://simracer.cz),
[4xracing.co.uk](http://4xracing.co.uk)
