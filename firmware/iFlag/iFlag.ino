//
// iFlag LED Matrix Firmware
// =========================
// https://github.com/simracer-cz/iFlag
//
// Hardware needed:
//   1. Arduino Uno R3 (http://www.arduino.cc/en/Main/ArduinoBoardUno)
//   2. Itead Colors Shield v1.1 (http://imall.iteadstudio.com/im120417002.html)
//   3. Itead 60mm Square 8x8 RGB LED Matrix module (http://imall.iteadstudio.com/im120601005.html)
//
// Software needed:
//   1. Arduino IDE (http://www.arduino.cc/en/Main/Software)
//   2. Colorduino Library for Arduino IDE (https://github.com/lincomatic/Colorduino)
//   3. iFlag host software (https://github.com/simracer-cz/iFlag)
//
// (c) 2015 Petr.Vostrel.cz

#include <Colorduino.h>
#include <EEPROM.h>

// Version
byte major = 0;
byte minor = 17;

// Communication
#define DEVICE_ID      0xD2
#define PACKET_BYTE    0xFF
#define COMMAND_BYTE   0xFF
#define DRAW_COMMAND   0xA0
#define BLINK_COMMAND  0xA1
#define RESET_COMMAND  0xA9
#define PING_COMMAND   0xB0
int dataX;
int dataY;
int dataP;
byte pinger;

// Color palette
byte colors[ 16 ][ 3 ]=
{
    {   0,   0,   0 },   // 0x00 | black
    { 255, 255, 255 },   // 0x01 | white
    { 255,   0,   0 },   // 0x02 | red
    {   0, 255,   0 },   // 0x03 | green
    {   0,   0, 255 },   // 0x04 | blue
    { 255, 255,   0 },   // 0x05 | yellow
    {   0, 255, 255 },   // 0x06 | teal
    { 255,   0, 255 },   // 0x07 | purple
    { 255,  33,   0 },   // 0x08 | orange
    {  55,  55,  55 },   // 0x09 | dim white
    {  55,   0,   0 },   // 0x10 | dim red
    {   0,  55,   0 },   // 0x11 | dim green
    {   0,   0,  55 },   // 0x12 | dim blue
    {  55,  55,   0 },   // 0x13 | dim yellow
    {   0,  55,  55 },   // 0x14 | dim teal
    {  55,   0,  55 },   // 0x15 | dim purple
};
byte brightness[ 3 ] = { 63, 63, 63 }; // 0-63 RGB
byte blinker;
byte blink_speed= 0;

// Software reset
void ( *resetFunc ) ( void ) = 0;

void setup() 
{
    // Setup the LED matrix
    Colorduino.Init();
    Colorduino.SetWhiteBal( brightness );

    // Power cycle the Uno couple of times for solid matrix brightness uniformity
    byte powerCycles= EEPROM.read( 0x00 );
    if ( powerCycles < 3 || powerCycles == 255 )
    {
        EEPROM.write( 0x00, powerCycles + 1 );
        resetFunc();
    }
    else
    {
        EEPROM.write( 0x00, 0 );
    }

    // Communications port
    Serial.begin( 9600 );
    Serial.println( "##### iFlags v"+String(major)+"."+String(minor)+" Hello!" );

    // Color cycle the matrix to test out
    byte test[] = { 0x05, 0x06, 0x07, 0x01, 0x00 };
    for ( int i = 0; i < sizeof( test ); i++ )
    {
        Colorduino.ColorFill(
            colors[ test[i] ][ 0 ], // R
            colors[ test[i] ][ 1 ], // G
            colors[ test[i] ][ 2 ]  // B
        );
        delay( 1000 );
    }

    Colorduino.SetPixel( 0, 0, 0xFF, 0x99, 0x00 );
} 

void loop() 
{
    // Beacon
    if ( !(pinger++) )
        serialCommand( PING_COMMAND, DEVICE_ID, 0x00 );

    // Blinking
    if ( blink_speed && !(blinker+= blink_speed) )
        Colorduino.FlipPage();

    delay( 2 );
}

void serialEvent(){
    while ( Serial.available() && Serial.peek() != PACKET_BYTE ) Serial.read();
    if ( Serial.available() >= 8 && Serial.read() == PACKET_BYTE )
    {
        if ( Serial.peek() != PACKET_BYTE ) // Stream
        {
            dataX = Serial.read();          // (00-07) LED X
            dataY = Serial.read();          // (00-07) LED Y

            for ( int i = 0; i < 4; i++ )
            {
                dataP = Serial.read();      // (00-FE) four color pixels

                Colorduino.SetPixel(
                    dataX + i,              // X
                    dataY,                  // Y
                    colors[ dataP ][ 0 ],   // R
                    colors[ dataP ][ 1 ],   // G
                    colors[ dataP ][ 2 ]    // B
                );
            }
        }
        else                        // Command
        {
            byte command[ 4 ] =
            {
                Serial.read(),      // (FF) Command trigger byte
                Serial.read(),      // (00-FE) Command id
                Serial.read(),      // (00-FE) Command value
                Serial.read()       // (00-FE) Command extra value
            };
            switch ( command[ 1 ] )
            {
                case DRAW_COMMAND:
                    Colorduino.FlipPage();
                    break;

                case BLINK_COMMAND:
                    blink_speed = command[ 2 ];
                    blinker = 0;
                    break;

                case RESET_COMMAND:
                    resetFunc();
                    break;
            }
        }
    }
}

void serialCommand( byte command, byte value, byte extra )
{
    byte payload[ 8 ] =
    {
      PACKET_BYTE,      // (FF) start byte
      COMMAND_BYTE,     // (FF) serial command trigger byte
      major,            // (00-FE) major firmware version
      minor,            // (00-FE) minor firmware version
      command,          // (00-FE) command id
      value,            // (00-FE) command value
      extra,            // (00-FE) optional extra value
      0x00
    };

    for( int i = 0; i < sizeof(payload) / sizeof(payload[ 0 ]); i++ )
        Serial.println( payload[ i ] );
}


