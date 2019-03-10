![iFLAG logo](http://simracer.cz/iracing/iFlag-logo/logo-full.svg)

Patterns
========
_as defined in [patterns.cs](../iFlag/patterns.cs)_

A complete alphabetical list of patterns, which get conbined with color index information and timing to form a [visual signal](Signals.md).
Many of them are capable of more than they are used for and there are also few patterns, which are currently not being used at all.


Mechanics
---------

Each pattern is made out of color indexes referring to the `color` array the actual flag uses in the `flag()` call.
This allows for reuse of the patterns and while many are unique to some flags, some of the more trivial ones are being used by multiple signals at once.


Color Legend
------------

Frame Color | Index in `flag()`'s `color` Array
----------- | ---------------------------------
Red         | `0`
Green       | `1`
Blue        | `2`
Yellow      | `3`
Cyan        | `4` *
Magenta     | `5` *
Black       | `9`

\* Not used yet


Pattern Frames
--------------

| Pattern              | Frames                                | Used For |
| -------------------- | --------------------------------------| ------- |
| `CHECKERED`          | ![](patterns/checkered.gif)           | Checkered flag |
| `CORNERS`            | ![](patterns/corners.gif)             | Pits closed |
| `CROSSED`            | ![](patterns/crossed.gif)             | Disqualified, Entering Closed Pits! |
| `DIAGONAL_STRIPE`    | ![](patterns/diagonal-stripe.gif)     | Blue flag |
| `DOUBLE_WAVING`      | ![](patterns/double-waving.gif)       | (N/A) |
| `F`                  | ![](patterns/f.gif)                   | Greeting, Orientation check pattern |
| `FLASHING`           | ![](patterns/flashing.gif)            | Red flag, Full course caution (oval) |
| `FURLED`             | ![](patterns/furled.gif)              | Furled black flag |
| `HALF`               | ![](patterns/half.gif)                | Start lights: Ready! |
| `INCIDENT`           | ![](patterns/incident.gif)            | Incident notification |
| `INVERTED`           | ![](patterns/inverted.gif)            | Black flag, One lap to green |
| `IRACING_LOGO`       | ![](patterns/iracing.gif)             | (N/A) |
| `MEATBALL`           | ![](patterns/meatball.gif)            | Meat ball flag |
| `SC`                 | ![](patterns/sc.gif)                  | Full course caution (road) |
| `SIMPLE`             | ![](patterns/simple.gif)              | White flag, Start lights: Set!, All green flags |
| `STATUS`             | ![](patterns/status.gif)              | (N/A) |
| `STRIPPED`           | ![](patterns/stripped.gif)            | Debris flag |
| `WAVING`             | ![](patterns/waving.gif)              | Yellow flag |
| `WARN_L`             | ![](patterns/warn-left.gif)           | Car(s) left spotter warning |
| `WARN_R`             | ![](patterns/warn-right.gif)          | Car(s) right spotter warning |
| `WARN_LR`            | ![](patterns/warn-left-right.gif)     | Cars left abd right spotter warning |



---
© 2015-2019
[Petr.Vostřel.cz](http://petr.vostrel.cz),
[simracer.cz](http://simracer.cz)
