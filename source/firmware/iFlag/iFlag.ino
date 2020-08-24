//
// iFLAG LED Matrix Firmware
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
//   3. iFLAG host software (https://github.com/simracer-cz/iFlag)
//
// (c) 2015-2020 Petr.Vostrel.cz

#include <Colorduino.h>
#include <EEPROM.h>

// Version
byte major = 0;
byte minor = 23;

// Communication
#define DEVICE_ID      0xD2
#define PACKET_BYTE    0xFF
#define COMMAND_BYTE   0xFF
#define DRAW_COMMAND   0xA0
#define BLINK_COMMAND  0xA1
#define LUMA_COMMAND   0xA2
#define FRAME_COMMAND  0xA3
#define RESET_COMMAND  0xA9
#define PING_COMMAND   0xB0
int dataX;
int dataY;
int dataP;
int pinger;

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
byte balance[ 3 ] = { 36, 45, 63 }; // 0-63 RGB
                                    // Red is toned down to half here to limit the effects of its inevitable light
                                    // strength dominance due to physical custruction differences between all three
                                    // color chips in the matrix LED

float luma = 1.0F;                  // 0.00-1.00 Luminosity % level 

unsigned int blinker;
byte blink_speed= 0;

byte frame = 0;                     // Currently displayed frame
const int frames = 6;               // Total frames capacity of the animation
const int dots = 8 * 8;             // Total number of dots (pixels) of the display

// Frames dots buffer
byte dot[ frames ][ dots ] = {{ 0 }};

// Software reset
void ( *resetFunc ) ( void ) = 0;

void setup() 
{
    setupDevice();

    // Communications port
    Serial.begin( 9600 );
    Serial.println( "##### iFLAG v"+String(major)+"."+String(minor)+" Hello!" );

    // Color cycle the matrix to test out
    byte test[] = { 0x05, 0x06, 0x07, 0x01, 0x00 };
    for ( int i = 0; i < sizeof( test ); i++ )
    {
        testDots( test[i] );
        delay( 1000 );
    }
} 

void loop() 
{
    // Beacon
    if ( !(pinger++) )
        serialCommand( PING_COMMAND, DEVICE_ID, 0x00 );

    // Blinking
    if ( blink_speed && !(blinker+= blink_speed) )
        advanceFrame();
}

void serialEvent(){
    while ( Serial.available() && Serial.peek() != PACKET_BYTE ) Serial.read();
    if ( Serial.available() >= 8 && Serial.read() == PACKET_BYTE )
    {
        if ( Serial.peek() != COMMAND_BYTE ) // Stream
        {
            dataX = Serial.read();          // (00-07) LED X
            dataY = Serial.read();          // (00-07) LED Y

            for ( int i = 0; i < 4; i++ )
            {
                dataP = Serial.read();      // (00-FE) four color pixels

                Colorduino.SetPixel(
                    dataX + i,                           // X
                    dataY,                               // Y
                    luma * colors[ dataP ][ 0 ] / 100,   // R
                    luma * colors[ dataP ][ 1 ] / 100,   // G
                    luma * colors[ dataP ][ 2 ] / 100    // B
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
                case FRAME_COMMAND:
                    frame = command[ 2 ] - 1;
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

                case LUMA_COMMAND:
                    luma = command[ 2 ] / 100.0F;
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



// Set a given dot (pixel) of a frame to a given color
void setDot( byte px, byte color )
{
    dot[ frame ][ px ] = color;
}

// Set all dots of first frame to given color and render
void testDots( byte color )
{
    frame = 0;
    for ( int i = 0; i < dots; i++ )
        setDot( i, color );

    renderFrame();
}

// Advance to next buffer frame
void advanceFrame()
{
    frame = frame++ > frames - 2 ? 0 : frame;
    renderFrame();
}

// Device-specific proxies
void setupDevice() { setupDevice_Colorduino(); }
void renderFrame() { renderFrame_Colorduino(); }



// -------------------------------------------------------------------------


// Instruct Colorduino library to initialize the matrix
void setupDevice_Colorduino()
{
    Colorduino.Init();
    Colorduino.SetWhiteBal( balance );
}

// Instruct Colorduino library to render the current frame
void renderFrame_Colorduino()
{
    for ( int i = 0; i < dots; i++ )
        Colorduino.SetPixel(
            i % 8,                                // X
            i / 8,                                // Y
            luma * colors[ dot[frame][i] ][ 0 ],  // R
            luma * colors[ dot[frame][i] ][ 1 ],  // G
            luma * colors[ dot[frame][i] ][ 2 ]   // B
        );

    Colorduino.FlipPage();
}

