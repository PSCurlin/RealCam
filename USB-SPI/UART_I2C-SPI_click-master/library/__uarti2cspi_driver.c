/*
    __uarti2cspi_driver.c

-----------------------------------------------------------------------------

  This file is part of mikroSDK.

  Copyright (c) 2017, MikroElektonika - http://www.mikroe.com

  All rights reserved.

----------------------------------------------------------------------------- */

#include "__uarti2cspi_driver.h"
#include "__uarti2cspi_hal.c"

/* ------------------------------------------------------------------- MACROS */



/* ---------------------------------------------------------------- VARIABLES */

#ifdef   __UARTI2CSPI_DRV_I2C__
static uint8_t _slaveAddress;
#endif

const uint8_t _UARTI2CSPI_COMMUNICATION_I2C = 0x01;
const uint8_t _UARTI2CSPI_COMMUNICATION_SPI = 0x02;
static uint8_t _communication;

float UARTI2CSPI_OSCILATOR_FREQ             = 1843200.0;
float UARTI2CSPI_PRESCALER_DEF              = 1.0;
const uint8_t UARTI2CSPI_ADDR               = 0x48;
const uint8_t UARTI2CSPI_RHR                = 0x00<<3;
const uint8_t UARTI2CSPI_THR                = 0x00<<3;
const uint8_t UARTI2CSPI_IER                = 0x01<<3;
const uint8_t UARTI2CSPI_FCR                = 0x02<<3;
const uint8_t UARTI2CSPI_IIR                = 0x02<<3;
const uint8_t UARTI2CSPI_LCR                = 0x03<<3;
const uint8_t UARTI2CSPI_MCR                = 0x04<<3;
const uint8_t UARTI2CSPI_LSR                = 0x05<<3;
const uint8_t UARTI2CSPI_MSR                = 0x06<<3;
const uint8_t UARTI2CSPI_SPR                = 0x07<<3;
const uint8_t UARTI2CSPI_TCR                = 0x06<<3;
const uint8_t UARTI2CSPI_TLR                = 0x07<<3;
const uint8_t UARTI2CSPI_TXLVL              = 0x08<<3;
const uint8_t UARTI2CSPI_RXLVL              = 0x09<<3;
const uint8_t UARTI2CSPI_EFCR               = 0x0F<<3;

/* Special register set LCR[7] = 1 */
const uint8_t UARTI2CSPI_DLL   = 0x00<<3;
const uint8_t UARTI2CSPI_DLH   = 0x01<<3;
/* Enhanced register set LCR   = 0xBF.*/
const uint8_t UARTI2CSPI_EFR   = 0x02<<3;
const uint8_t UARTI2CSPI_XON1  = 0x04<<3;
const uint8_t UARTI2CSPI_XON2  = 0x05<<3;
const uint8_t UARTI2CSPI_XOFF1 = 0x06<<3;
const uint8_t UARTI2CSPI_XOFF2 = 0x07<<3;

/* Bitfields Init advanced */
const uint8_t UARTI2CSPI_UART_5_BIT_DATA   = 0x00;
const uint8_t UARTI2CSPI_UART_6_BIT_DATA   = 0x01;
const uint8_t UARTI2CSPI_UART_7_BIT_DATA   = 0x02;
const uint8_t UARTI2CSPI_UART_8_BIT_DATA   = 0x03;
const uint8_t UARTI2CSPI_UART_NOPARITY     = 0x00;
const uint8_t UARTI2CSPI_UART_EVENPARITY   = 0x18;
const uint8_t UARTI2CSPI_UART_ODDPARITY    = 0x08;
const uint8_t UARTI2CSPI_UART_PARITY_ONE   = 0x38;
const uint8_t UARTI2CSPI_UART_PARITY_ZERO  = 0x28;
const uint8_t UARTI2CSPI_UART_ONE_STOPBIT  = 0x00;
const uint8_t UARTI2CSPI_UART_TWO_STOPBITS = 0x04;

/* Interrupt bits */
const uint8_t UARTI2CSPI_CTS_INT_EN                   = 0x80;
const uint8_t UARTI2CSPI_RTS_INT_EN                   = 0x40;
const uint8_t UARTI2CSPI_XOFF_INT_EN                  = 0x20;
const uint8_t UARTI2CSPI_SLEEP_INT_EN                 = 0x10;
const uint8_t UARTI2CSPI_MODEM_STATUS_INT_EN          = 0x08;
const uint8_t UARTI2CSPI_RECEIVE_LINE_STATUS_INT_EN   = 0x04;
const uint8_t UARTI2CSPI_THR_EMPTY_INT_EN             = 0x02;
const uint8_t UARTI2CSPI_RXD_INT_EN                   = 0x01;

const uint8_t _LINE_PRINT = 0x03;
const uint8_t _TEXT_PRINT = 0x01;
const uint8_t _CHAR_PRINT = 0x04;




/* -------------------------------------------- PRIVATE FUNCTION DECLARATIONS */
static uint16_t _calcBaudRate(uint16_t baud_rate);


/* --------------------------------------------- PRIVATE FUNCTION DEFINITIONS */
static uint16_t _calcBaudRate(uint16_t baud_rate)
{
    uint16_t calc;
    float tmp;
    tmp =  UARTI2CSPI_OSCILATOR_FREQ / UARTI2CSPI_PRESCALER_DEF;
    calc = (uint16_t)( tmp / ( (float)baud_rate * 16.0 ) );
    return calc;
}

/* --------------------------------------------------------- PUBLIC FUNCTIONS */

#ifdef   __UARTI2CSPI_DRV_SPI__

void uarti2cspi_spiDriverInit(T_UARTI2CSPI_P gpioObj, T_UARTI2CSPI_P spiObj)
{
    Delay_1ms();
    _communication = _UARTI2CSPI_COMMUNICATION_SPI;
    hal_spiMap( (T_HAL_P)spiObj );
    hal_gpioMap( (T_HAL_P)gpioObj );

    // ... power ON
    // ... configure CHIP
    hal_gpio_rstSet(1);
    hal_gpio_csSet(1);
    
}

#endif
#ifdef   __UARTI2CSPI_DRV_I2C__

void uarti2cspi_i2cDriverInit(T_UARTI2CSPI_P gpioObj, T_UARTI2CSPI_P i2cObj, uint8_t slave)
{
    Delay_1ms();
    _slaveAddress = slave;
    _communication = _UARTI2CSPI_COMMUNICATION_I2C;
    hal_i2cMap( (T_HAL_P)i2cObj );
    hal_gpioMap( (T_HAL_P)gpioObj );

    // ... power ON
    // ... configure CHIP
    hal_gpio_rstSet(1);
    hal_gpio_csSet(1);
}

#endif

/* ----------------------------------------------------------- IMPLEMENTATION */
void uarti2cspi_csSet( uint8_t val )
{
     hal_gpio_csSet(val);
}

void uarti2cspi_rstSet( uint8_t val )
{
     hal_gpio_rstSet(val);
}

uint8_t uarti2cspi_intSet()
{
   return  hal_gpio_intGet();
}

void uarti2cspi_writeReg( uint8_t reg, uint8_t _data )
{
    uint8_t writeReg[ 2 ];

    writeReg[ 0 ] = reg;
    writeReg[ 1 ] = _data;

    if( _communication == _UARTI2CSPI_COMMUNICATION_I2C )
    {
        hal_i2cStart();
        hal_i2cWrite( _slaveAddress, &writeReg[0], 2, END_MODE_STOP );
    }
    else
    {
        hal_gpio_csSet( 0 );
        hal_spiTransfer( &writeReg[0] ,&writeReg[0], 2 );
        hal_gpio_csSet( 1 );
    }
}

uint8_t uarti2cspi_readReg( uint8_t reg )
{
    uint8_t writeReg[ 2 ];
    writeReg[0] = reg;

    if( _communication == _UARTI2CSPI_COMMUNICATION_I2C )
    {
        hal_i2cStart();
        hal_i2cWrite( _slaveAddress, &writeReg[0], 1, END_MODE_RESTART );
        hal_i2cRead( _slaveAddress, &writeReg[0], 1, END_MODE_STOP );

        return writeReg[ 0 ];
    }
    else
    {
        writeReg[0] |= 0x80;
        hal_gpio_csSet(0);
        hal_spiTransfer( &writeReg[0], &writeReg[0], 2 );
        hal_gpio_csSet( 1 );
        return writeReg[ 1 ];
    }
}

void uarti2cspi_flowControl(uint8_t cfg)
{
     uint8_t flowTemp;
     uint8_t lcrTemp;
     //TODO: DEFAULT DISABLE !
     // LCR = 0xBF Enable enhanced registers
     // RTS, CTS EN
     // if( flow cfg = 1) EFR = 0xC0
     // LCR = 0x03 // 8 data bits, 1 stop bit, no parity
     
     lcrTemp = uarti2cspi_readReg( UARTI2CSPI_LCR );                              //preserv LCR register state
     uarti2cspi_writeReg( UARTI2CSPI_LCR, 0xBF );                               //enable enhanced feature register
     flowTemp = uarti2cspi_readReg( UARTI2CSPI_EFR );                             //read the current state of the enhanced feature register
     
     if( cfg == 1 )
     {
        flowTemp |= 0xC0;                                                       //set the flow controll fields
     }
     else
     {
        flowTemp &= ~0xC0;                                                      //clear the flow controll fields
     }
     uarti2cspi_writeReg( UARTI2CSPI_EFR, flowTemp );
     uarti2cspi_writeReg( UARTI2CSPI_LCR,lcrTemp   );
}

void uarti2cspi_interruptEnable( uint8_t vect )
{
     uint8_t regTemp;
     uint8_t flowTmp;
     
     regTemp=uarti2cspi_readReg( UARTI2CSPI_LCR );     //preserv LCR state
     
     if( (vect & 0x80) || (vect & 0x40) || (vect & 0x20) || (vect & 0x10) )
     {
        /*
           Enhanced functionality bit must be set before CTS, RTS, XOFF and
           SLEEP mode interrupt enable bits are modified.
         */
        uarti2cspi_writeReg( UARTI2CSPI_LCR, 0xBF );
        flowTmp = uarti2cspi_readReg( UARTI2CSPI_EFR );
        uarti2cspi_writeReg( UARTI2CSPI_EFR, flowTmp | 0x10 );
     }
     else
     {
        uarti2cspi_writeReg( UARTI2CSPI_LCR, 0xBF );
        flowTmp = uarti2cspi_readReg( UARTI2CSPI_EFR );
        flowTmp &= ~0x10;
        uarti2cspi_writeReg( UARTI2CSPI_EFR, flowTmp );
     }
     
     uarti2cspi_writeReg( UARTI2CSPI_LCR, regTemp );
     uarti2cspi_writeReg( UARTI2CSPI_IER, vect    );

}

void uarti2cspi_init( uint16_t baud_rate )
{
    uint8_t DLL_val;
    uint8_t DLH_val;
    uint16_t BRR_Reg_Speed;

    if(DLL_val);
    if(DLH_val);
    if(BRR_Reg_Speed);
    
    hal_gpio_rstSet(0);
    Delay_100ms();
    hal_gpio_rstSet(1);
    Delay_100ms();
    
    BRR_Reg_Speed = _calcBaudRate(baud_rate);
    
    DLH_val = (uint8_t)( ( BRR_Reg_Speed >> 8 ) & 0x00FF );
    DLL_val = (uint8_t)( BRR_Reg_Speed & 0x00FF );

    //LCR = 0x80; enable acces to DLL and DLH registers
    //DLL = DLL_val  // Write to low register
    //DLH = DLH_val  // Write to HI register
    /* Config NO Parity, one stop bit */
    // LCR = 0x03 // 8 data bits, 1 stop bit, no parity
    uarti2cspi_writeReg( UARTI2CSPI_LCR, 0x80    );
    uarti2cspi_writeReg( UARTI2CSPI_DLL, DLL_val ); //DLL
    uarti2cspi_writeReg( UARTI2CSPI_DLH, DLH_val ); //DLH
    uarti2cspi_writeReg( UARTI2CSPI_LCR, 0xBF    );
    uarti2cspi_writeReg( UARTI2CSPI_EFR, 0       ); //EFR
    uarti2cspi_writeReg( UARTI2CSPI_LCR, 0x03    );
    uarti2cspi_writeReg( UARTI2CSPI_FCR, 0x00    );

    /*RX Interrupt enable*/
    // IER = 0x04;
    uarti2cspi_writeReg( UARTI2CSPI_IER, 0x03 );
    
}

void uarti2cspi_initAdvanced( uint16_t baud_rate, uint8_t data_bits, uint8_t parity_mode, uint8_t stop_bits)
{
    uint8_t DLL_val;
    uint8_t DLH_val;
    uint16_t BRR_Reg_Speed;

    if(DLL_val);
    if(DLH_val);
    if(BRR_Reg_Speed);

    hal_gpio_rstSet(0);
    Delay_100ms();
    hal_gpio_rstSet(1);
    Delay_100ms();

    BRR_Reg_Speed = _calcBaudRate( baud_rate );

    DLH_val = (uint8_t)( ( BRR_Reg_Speed >> 8 ) & 0x00FF );
    DLL_val = (uint8_t)( BRR_Reg_Speed & 0x00FF );
    uarti2cspi_writeReg( UARTI2CSPI_LCR, 0x80    );                             //Enable Special register latch
    uarti2cspi_writeReg( UARTI2CSPI_DLL, DLL_val );                             //DLL
    uarti2cspi_writeReg( UARTI2CSPI_DLH, DLH_val );                             //DLH
    uarti2cspi_writeReg( UARTI2CSPI_LCR, 0xBF    );
    uarti2cspi_writeReg( UARTI2CSPI_EFR, 0       );                             //NO FLOW CONTROLL
    uarti2cspi_writeReg( UARTI2CSPI_LCR, data_bits | parity_mode | stop_bits ); //write UART configuration
    
    uarti2cspi_writeReg( UARTI2CSPI_IER, 0x03 );                                //Enable RX,TX interrupt
}
void uartFlush(void)
{
      uarti2cspi_uartRead();
}
void uarti2cspi_uartWrite1( uint8_t wByte )
{
      uarti2cspi_writeReg( UARTI2CSPI_IER, 0x02 );
      uarti2cspi_writeReg(UARTI2CSPI_THR,wByte);
      while(hal_gpio_intGet());
      uarti2cspi_writeReg( UARTI2CSPI_IER, 0x01 );
}

void uarti2cspi_uartWrite( uint8_t wByte )
{
      uarti2cspi_writeReg(UARTI2CSPI_THR,wByte);
      while(hal_gpio_intGet());
      uarti2cspi_writeReg( UARTI2CSPI_IER, 0x01 );
}

void uarti2cspi_uartText( uint8_t *wText )
{

     while(*wText)
     {
         uarti2cspi_uartWrite1( *wText++ );
     }
}

void uarti2cspi_serialWrite(uint8_t *strDat, uint8_t printMode)
{
    if(printMode & _TEXT_PRINT )
    {
        uarti2cspi_uartText( strDat );

        if(printMode & _LINE_PRINT)
        {
            uarti2cspi_uartText("\r\n");
        }
    }
    if(printMode == _CHAR_PRINT)
    {
         uarti2cspi_uartWrite1( *strDat );
    }
}

uint8_t uarti2cspi_uartDataready()
{
   uint8_t rDat;
   
   if( hal_gpio_intGet() )
   {
     rDat = uarti2cspi_readReg( UARTI2CSPI_LSR );
     return ( rDat & 1 );
   }
   return 0;
}

uint8_t uarti2cspi_uartRead()
{
    uint8_t RHR_V;
    RHR_V = uarti2cspi_readReg( UARTI2CSPI_RHR );
    return  RHR_V;
}

/* -------------------------------------------------------------------------- */
/*
  __uarti2cspi_driver.c

  Copyright (c) 2017, MikroElektonika - http://www.mikroe.com

  All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions are met:

1. Redistributions of source code must retain the above copyright
   notice, this list of conditions and the following disclaimer.

2. Redistributions in binary form must reproduce the above copyright
   notice, this list of conditions and the following disclaimer in the
   documentation and/or other materials provided with the distribution.

3. All advertising materials mentioning features or use of this software
   must display the following acknowledgement:
   This product includes software developed by the MikroElektonika.

4. Neither the name of the MikroElektonika nor the
   names of its contributors may be used to endorse or promote products
   derived from this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY MIKROELEKTRONIKA ''AS IS'' AND ANY
EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
DISCLAIMED. IN NO EVENT SHALL MIKROELEKTRONIKA BE LIABLE FOR ANY
DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
(INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
(INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

----------------------------------------------------------------------------- */
