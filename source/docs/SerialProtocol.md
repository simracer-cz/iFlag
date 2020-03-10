![iFLAG logo](http://simracer.cz/iracing/iFlag-logo/logo-full.svg)

Serial Protocol
===============
_as defined in [communication.cs](../iFlag/communication.cs)_


Data Stream Packets
-------------------

The image data come in 8 bytes long packets. These travel in one way only with the device always on the receiving end. Each of the packed defines X and Y coordinates relative to top left corner and color palette indexes for 4 pixels of the LED matrix.

| # | Component           | Meaning       | Description |
| - | ------------------- | ------------- | ----------- |
| 1 | Packet leading byte | `0xFF`        |
| 2 | Column              | `0x00`-`0x07` |
| 3 | Row                 | `0x00`-`0x07` |
| 4 | Pixel #1            | `0x00`-`0xFE` | Left-most of the four pixels |
| 5 | Pixel #2            | `0x00`-`0xFE` |
| 6 | Pixel #3            | `0x00`-`0xFE` |
| 7 | Pixel #4            | `0x00`-`0xFE` | Right-most of the four |
| 8 | Empty               | `0x00`        |

One matrix visual consists of 16 such packets and are followed by a command packet, which instructs the hardware to do something with the buffered data.


Command Packets
---------------

Aside of the data stream there are a few commands you can call up to at any place of the data stream. These are two-way and can be send both to and from the device.

| # | Byte                | Meaning       | Description |
| - | ------------------- | ------------- | ----------- |
| 1 | Packet leading byte | `0xFF`        |
| 2 | Command trigger     | `0xFF`        |
| 3 | Command ID          | `0xA0 - 0xA9` |
| 4 | Value               | `0x00 - 0xFE` |
| 5 | Extra value         | `0x00 - 0xFE` |
| 6 | Empty               | `0x00`        |
| 7 | Empty               | `0x00`        |
| 8 | Empty               | `0x00`        |


### Device-bound Commands

- __DRAW__ (`A0`)
The display driver operates with two buffers and switches between them. The incoming packets fill the buffer, which is not displayed until the buffers are latched with the `DRAW` command.

- __BLINK__ (`A1`)
Instructs the matrix to switch buffers timely based on the timer value given by the command's value. Value ranges from `1` (slow) to `5` (fast).

- __LUMA__ (`A2`)
Luminosity setting adjustment. The matrix will always start on 100%, but by using the `LUMA` command matrix can be instructed to use different percentage. Value ranges from `0` (no brightness) to `100` (full brightness).

- __RESET__ (`A9`)
Resets the board at once.


### Software-bound Commands

- __PING__ (`B0`)
The device continuously sends pings to the software to let it know about the _iFLAG_ presence. The `PING` command should be sent in roughly 2 Hz frequency (around 500ms interval between `PING` packets).

Ping packets are similar to regular data packets with the difference that they contain device type identifier (`D2` for _iFLAG_) and firmware version data in them for the purpose of software/firmware synchronization. This is its structure:

| # | Byte                | Meaning       | Description |
| - | ------------------- | ------------- | ----------- |
| 1 | Packet leading byte | `0xFF`        |
| 2 | Command trigger     | `0xFF`        |
| 3 | Major FW version    | `0x00 - 0x99` | 
| 4 | Minor FW version    | `0x00 - 0x99` |
| 5 | Command ID          | `0xB0`        |
| 6 | Device Type         | `0xD2`        |
| 7 | Empty               | `0x00`        |
| 8 | Empty               | `0x00`        |


Communication Speed
-------------------

Currently all serial communication happens in 9600 baudrate.

_iFLAG_ will establish connection to 9600 as well as 38400 baudrate shall the firmware already support it, but _iFLAG_ device itself doesn't use this speed yet until the multi-speed support makes it into the stable release and ensures seamless down/upgrade of the firmware.







---
© 2015-2020
[Petr.Vostřel.cz](http://petr.vostrel.cz),
[simracer.cz](http://simracer.cz)
