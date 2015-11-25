Serial Protocol
===============


_=== WORK IN PROGRESS ===_


Data Stream
-----------

The image data come in packets. Each of the packed defines X and Y coordinates and color palette indexes for 4 pixels of the LED matrix.

| Component | Meaning | Description |
| --- | ------- | ----------- |
| Packet leading byte | `0xFF`    |
| Column              | `0x00`-`0x07` |
| Row                 | `0x00`-`0x07` |
| Pixel #1            | `0x00`-`0xFE` |
| Pixel #2            | `0x00`-`0xFE` |
| Pixel #3            | `0x00`-`0xFE` |
| Pixel #4            | `0x00`-`0xFE` |

One matrix visual consists of 16 such packets and are followed by a command packet, which instructs the hardware to do something with the buffered data.

| Bit | Meaning | Description |
| --- | ------- | ----------- |
| Packet leading byte | `0xFF`    |
| Command trigger     | `0xFF`    |
| Command ID          | `0xA0 - 0xA9` |
| Value               | `0x00 - 0xFE` |
| Extra value         | `0x00 - 0xFE` |


The display driver operates with two buffers and switches between them. The incoming packets fill the buffer, which is not displayed and in order to switch the buffers, a REDRAW command needs to be issued


---
© 2015
[Petr.Vostřel.cz](http://petr.vostrel.cz),
[simracer.cz](http://simracer.cz),
[4xracing.co.uk](http://4xracing.co.uk)
