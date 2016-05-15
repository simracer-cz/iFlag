![iFlag logo](http://simracer.cz/iracing/iFlag-logo/logo-full.svg)

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
Cyan        | `4` (N/A)
Magenta     | `5` (N/A)

\* Not used yet


Pattern Frames
--------------

| Pattern                   | Frames                                | Used For |
| ------------------------- | --------------------------------------| ------- |
| `CHECKERED_FLAG`          | ![](patterns/checkered.gif)           | Checkered flag |
| `CROSSED_FLAG`            | ![](patterns/crossed.gif)             | Disqualified |
| `DIAGONAL_STRIPE_FLAG`    | ![](patterns/diagonal-stripe.gif)     | Blue flag |
| `DOUBLE_WAVING_FLAG`      | ![](patterns/double-waving.gif)       | (N/A) |
| `F_FLAG`                  | ![](patterns/f.gif)                   | Greeting, Orientation check pattern |
| `FLASHING_FLAG`           | ![](patterns/flashing.gif)            | Red flag, Full course caution (oval) |
| `FURLED_FLAG`             | ![](patterns/furled.gif)              | Furled black flag |
| `HALF_FLAG`               | ![](patterns/half.gif)                | Start lights: Ready! |
| `INVERTED_FLAG`           | ![](patterns/inverted.gif)            | Black flag, One lap to green |
| `IRACING_LOGO_FLAG`       | ![](patterns/iracing.gif)             | (N/A) |
| `MEATBALL_FLAG`           | ![](patterns/meatball.gif)            | Meat ball flag |
| `SC_FLAG`                 | ![](patterns/sc.gif)                  | Full course caution (road) |
| `SIMPLE_FLAG`             | ![](patterns/simple.gif)              | White flag, Start lights: Set!, All green flags |
| `STATUS_FLAG`             | ![](patterns/status.gif)              | (N/A) |
| `STRIPPED_FLAG`           | ![](patterns/stripped.gif)            | Debris flag |
| `WAVING_FLAG`             | ![](patterns/waving.gif)              | Yellow flag |



---
© 2015-2016
[Petr.Vostřel.cz](http://petr.vostrel.cz),
[simracer.cz](http://simracer.cz),
[4xracing.co.uk](http://4xracing.co.uk)
