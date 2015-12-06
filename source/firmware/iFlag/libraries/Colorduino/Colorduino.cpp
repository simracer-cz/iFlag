/*
  Colorduino - Colorduino Library for Arduino
  Copyright (c) 2011-2012 Sam C. Lin <lincomatic@hotmail.com>
  based on C code by zzy@iteadstudio
    Copyright (c) 2010 zzy@IteadStudio.  All right reserved.

  This library is free software; you can redistribute it and/or
  modify it under the terms of the GNU Lesser General Public
  License as published by the Free Software Foundation; either
  version 2.1 of the License, or (at your option) any later version.

  This library is distributed in the hope that it will be useful,
  but WITHOUT ANY WARRANTY; without even the implied warranty of
  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
  Lesser General Public License for more details.

  You should have received a copy of the GNU Lesser General Public
  License along with this library; if not, write to the Free Software
  Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
*/
#include "Colorduino.h"

void ColorduinoObject::LED_Delay(unsigned char i)
{
  unsigned int y;
  y = i * 10;
  while(y--);
}

// compensate for relative intensity differences in R/G/B brightness
// array of 6-bit base values for RGB (0~63)
// wbval[0]=red
// wbval[1]=green
// wbval[2]=blue
void ColorduinoObject::SetWhiteBal(unsigned char wbval[3])
{
  LED_LAT_CLR;
  LED_SLB_CLR;
  for(unsigned char k=0;k<ColorduinoScreenHeight;k++)
    for(unsigned char i = 3;i > 0 ;i--)
    {
      unsigned char temp = wbval[i-1]<<2;
      for(unsigned char j = 0;j<6;j++)
      {
        if(temp &0x80)
          LED_SDA_SET;
        else
          LED_SDA_CLR;
        
        temp =temp << 1;
        LED_SCL_CLR;
        LED_SCL_SET;
    }
  }
  LED_SLB_SET;
}

/********************************************************
Name: ColorFill
Function: Fill the frame with a color
Parameter:R: the value of RED.   Range:RED 0~255
          G: the value of GREEN. Range:RED 0~255
          B: the value of BLUE.  Range:RED 0~255
********************************************************/
void ColorduinoObject::ColorFill(unsigned char R,unsigned char G,unsigned char B)
{
  PixelRGB *p = GetPixel(0,0);
  for (unsigned char y=0;y<ColorduinoScreenWidth;y++) {
    for(unsigned char x=0;x<ColorduinoScreenHeight;x++) {
      p->r = R;
      p->g = G;
      p->b = B;
      p++;
    }
  }
  
  FlipPage();
}




// global instance
ColorduinoObject Colorduino;

#if defined (__AVR_ATmega32U4__)
ISR(TIMER4_OVF_vect)          //Timer4  Service 
{  
  //ISR fires every 256-TCNT4 ticks
  //so if TCNT4 = 100, ISR fires every 156 ticks
  //prescaler = 128 so ISR fires every 16MHz / 128 = 125KHz
  //125KHz / 156 = 801.282Hz / 8 rows = 100.16Hz refresh rate
  //if TCNT4 = 61, ISR fires every 256 - 61 = 195 ticks
  //125KHz / 195 = 641.026Hz / 8 rows = 80.128Hz refresh rate
  //TCNT4 = 100;
  TCNT4 = 61;
  close_all_lines;  
  Colorduino.run();
  Colorduino.open_line(Colorduino.line);
  if (++Colorduino.line > 7) Colorduino.line = 0;
}
#else
ISR(TIMER2_OVF_vect)          //Timer2  Service 
{ 
 // ISR fires every 256-TCNT2 ticks
 // so if TCNT2 = 100, ISR fires every 156 ticks
 // prescaler = 128 so ISR fires every 16MHz / 128 = 125KHz
 // 125KHz / 156 = 801.282Hz / 8 rows = 100.16Hz refresh rate
 // if TCNT2 = 61, ISR fires every 256 - 61 = 195 ticks
 // 125KHz / 195 = 641.026Hz / 8 rows = 80.128Hz refresh rate
  //  TCNT2 = 100;
  TCNT2 = 61;
  close_all_lines;  
  Colorduino.run();
  Colorduino.open_line(Colorduino.line);
  if (++Colorduino.line > 7) Colorduino.line = 0;
}
#endif

/****************************************************
the LED Hardware operate functions zone
****************************************************/

/***************************************************
the LED datas operate functions zone
***************************************************/

void ColorduinoObject::run()
{
  LED_SLB_SET;
  LED_LAT_CLR;
  PixelRGB *pixel = GetDrawPixel(0,line);
  for(unsigned char x=0;x<ColorduinoScreenWidth;x++)
  {
    unsigned char temp = pixel->b;
    unsigned char p;
    for(p=0;p<ColorduinoBitsPerColor;p++) {
      if(temp & 0x80)
	LED_SDA_SET;
      else
	LED_SDA_CLR;
      temp <<= 1;  
      LED_SCL_CLR;
      LED_SCL_SET;
    }
    temp = pixel->g;
    for(p=0;p<ColorduinoBitsPerColor;p++) {
      if(temp & 0x80)
	LED_SDA_SET;
      else
	LED_SDA_CLR;
      temp <<= 1;  
      LED_SCL_CLR;
      LED_SCL_SET;
    }
    temp = pixel->r;
    for(p=0;p<ColorduinoBitsPerColor;p++) {
      if(temp & 0x80)
	LED_SDA_SET;
      else
	LED_SDA_CLR;
      temp <<= 1;  
      LED_SCL_CLR;
      LED_SCL_SET;
    }
    pixel++;
  }
  LED_LAT_SET;
  LED_LAT_CLR;
}

