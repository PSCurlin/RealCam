/*
    __uarti2cspi_driver.h

-----------------------------------------------------------------------------

  This file is part of mikroSDK.
  
  Copyright (c) 2017, MikroElektonika - http://www.mikroe.com

  All rights reserved.

----------------------------------------------------------------------------- */

/**
@file   __uarti2cspi_driver.h
@brief    UART_I2C_SPI Driver
@mainpage UART_I2C_SPI Click
@{

@image html libstock_fb_view.jpg

@}

@defgroup   UARTI2CSPI
@brief      UART_I2C_SPI Click Driver
@{

| Global Library Prefix | **UARTI2CSPI** |
|:---------------------:|:-----------------:|
| Version               | **1.0.0**    |
| Date                  | **okt 2018.**      |
| Developer             | **Dusan Poluga**     |

*/
/* -------------------------------------------------------------------------- */

#include "stdint.h"

#ifndef _UARTI2CSPI_H_
#define _UARTI2CSPI_H_

/** 
 * @macro T_UARTI2CSPI_P
 * @brief Driver Abstract type 
 */
#define T_UARTI2CSPI_P    const uint8_t*

/** @defgroup UARTI2CSPI_COMPILE Compilation Config */              /** @{ */

   #define   __UARTI2CSPI_DRV_SPI__                            /**<     @macro __UARTI2CSPI_DRV_SPI__  @brief SPI driver selector */
   #define   __UARTI2CSPI_DRV_I2C__                            /**<     @macro __UARTI2CSPI_DRV_I2C__  @brief I2C driver selector */                                          
// #define   __UARTI2CSPI_DRV_UART__                           /**<     @macro __UARTI2CSPI_DRV_UART__ @brief UART driver selector */ 

                                                                       /** @} */
/** @defgroup UARTI2CSPI_VAR Variables */                           /** @{ */
extern const uint8_t UARTI2CSPI_ADDR ;
extern const uint8_t UARTI2CSPI_RHR  ;
extern const uint8_t UARTI2CSPI_THR  ;
extern const uint8_t UARTI2CSPI_IER  ;
extern const uint8_t UARTI2CSPI_FCR  ;
extern const uint8_t UARTI2CSPI_IIR  ;
extern const uint8_t UARTI2CSPI_LCR  ;
extern const uint8_t UARTI2CSPI_MCR  ;
extern const uint8_t UARTI2CSPI_LSR  ;
extern const uint8_t UARTI2CSPI_MSR  ;
extern const uint8_t UARTI2CSPI_SPR  ;
extern const uint8_t UARTI2CSPI_TCR  ;
extern const uint8_t UARTI2CSPI_TLR  ;
extern const uint8_t UARTI2CSPI_TXLVL;
extern const uint8_t UARTI2CSPI_RXLVL;
extern const uint8_t UARTI2CSPI_EFCR ;

/* Special register set LCR[7] = 1 */
extern const uint8_t UARTI2CSPI_DLL  ;
extern const uint8_t UARTI2CSPI_DLH  ;
/* Enhanced register set LCR   = 0xBF.*/
extern const uint8_t UARTI2CSPI_EFR  ;
extern const uint8_t UARTI2CSPI_XON1 ;
extern const uint8_t UARTI2CSPI_XON2 ;
extern const uint8_t UARTI2CSPI_XOFF1;
extern const uint8_t UARTI2CSPI_XOFF2;

extern const uint8_t UARTI2CSPI_UART_5_BIT_DATA   ;
extern const uint8_t UARTI2CSPI_UART_6_BIT_DATA   ;
extern const uint8_t UARTI2CSPI_UART_7_BIT_DATA   ;
extern const uint8_t UARTI2CSPI_UART_8_BIT_DATA   ;
extern const uint8_t UARTI2CSPI_UART_NOPARITY     ;
extern const uint8_t UARTI2CSPI_UART_EVENPARITY   ;
extern const uint8_t UARTI2CSPI_UART_ODDPARITY    ;
extern const uint8_t UARTI2CSPI_UART_PARITY_ONE   ;
extern const uint8_t UARTI2CSPI_UART_PARITY_ZERO  ;
extern const uint8_t UARTI2CSPI_UART_ONE_STOPBIT  ;
extern const uint8_t UARTI2CSPI_UART_TWO_STOPBITS ;

extern const uint8_t UARTI2CSPI_CTS_INT_EN                 ;
extern const uint8_t UARTI2CSPI_RTS_INT_EN                 ;
extern const uint8_t UARTI2CSPI_XOFF_INT_EN                ;
extern const uint8_t UARTI2CSPI_SLEEP_INT_EN               ;
extern const uint8_t UARTI2CSPI_MODEM_STATUS_INT_EN        ;
extern const uint8_t UARTI2CSPI_RECEIVE_LINE_STATUS_INT_EN ;
extern const uint8_t UARTI2CSPI_THR_EMPTY_INT_EN           ;
extern const uint8_t UARTI2CSPI_RXD_INT_EN                 ;

extern const uint8_t _LINE_PRINT;
extern const uint8_t _TEXT_PRINT;
extern const uint8_t _CHAR_PRINT;


                                                                       /** @} */
/** @defgroup UARTI2CSPI_TYPES Types */                             /** @{ */



                                                                       /** @} */
#ifdef __cplusplus
extern "C"{
#endif

/** @defgroup UARTI2CSPI_INIT Driver Initialization */              /** @{ */

#ifdef   __UARTI2CSPI_DRV_SPI__
void uarti2cspi_spiDriverInit(T_UARTI2CSPI_P gpioObj, T_UARTI2CSPI_P spiObj);
#endif
#ifdef   __UARTI2CSPI_DRV_I2C__
void uarti2cspi_i2cDriverInit(T_UARTI2CSPI_P gpioObj, T_UARTI2CSPI_P i2cObj, uint8_t slave);
#endif

                                                                       /** @} */
/** @defgroup UARTI2CSPI_FUNC Driver Functions */                   /** @{ */

/**
   Set the state of the chip select pin.
*/
void uarti2cspi_csSet(uint8_t val);

/**
   Set the state of the reset pin.
*/
void uarti2cspi_rstSet(uint8_t val);

/**
   Get the state of the interrupt pin.
*/
uint8_t uarti2cspi_intSet();

/**
   Generic register write function.
*/
void uarti2cspi_writeReg(uint8_t reg, uint8_t _data);

/**
   Generic register read function.
*/
uint8_t uarti2cspi_readReg(uint8_t reg);

/**
   Default configuration. Maximum baud rate that the click board can send data
   is 56000.
   The default UART configuration is 8N1.
*/
void uarti2cspi_init(uint16_t baud);

/**
   Advanced configuration.
*/
void uarti2cspi_initAdvanced(uint16_t baud_rate, uint8_t data_bits, uint8_t parity_mode, uint8_t stop_bits);

/**
   Enable disable flow control.
*/
void uarti2cspi_flowControl(uint8_t cfg);

/**
   Enable interrupts by passing the predefined constants.
   The function will handle all nessecery preconditions.
*/
void uarti2cspi_interruptEnable(uint8_t vect);

/**
   Check if the data has been received.
*/
uint8_t uarti2cspi_uartDataready();

/**
   Read the serial port data from the click board.
*/
uint8_t uarti2cspi_uartRead();

/**
   Write a byte to the serial port.
*/
void uarti2cspi_uartWrite(uint8_t wByte);

/**
   Write a string to the serial port.
*/
void uarti2cspi_uartText(uint8_t *wText);

/**
 *
 * Function for printing data to the serial port.
 *
 * Function parameters:
 *
 * - *strDat - String data
 *
 * - printMode - Modes for handing string data.
 *     _LINE_PRINT - Print "\r\n" to the end of the string.
 *     _TEXT_PRINT - Print string data without any additions.
 *     _CHAR_PRINT - Print a single character.
 *
 */
void uarti2cspi_serialWrite(uint8_t *strDat, uint8_t printMode);


                                                                       /** @} */
#ifdef __cplusplus
} // extern "C"
#endif
#endif

/**
    @example Click_UART_I2C_SPI_STM.c
    @example Click_UART_I2C_SPI_TIVA.c
    @example Click_UART_I2C_SPI_CEC.c
    @example Click_UART_I2C_SPI_KINETIS.c
    @example Click_UART_I2C_SPI_MSP.c
    @example Click_UART_I2C_SPI_PIC.c
    @example Click_UART_I2C_SPI_PIC32.c
    @example Click_UART_I2C_SPI_DSPIC.c
    @example Click_UART_I2C_SPI_AVR.c
    @example Click_UART_I2C_SPI_FT90x.c
    @example Click_UART_I2C_SPI_STM.mbas
    @example Click_UART_I2C_SPI_TIVA.mbas
    @example Click_UART_I2C_SPI_CEC.mbas
    @example Click_UART_I2C_SPI_KINETIS.mbas
    @example Click_UART_I2C_SPI_MSP.mbas
    @example Click_UART_I2C_SPI_PIC.mbas
    @example Click_UART_I2C_SPI_PIC32.mbas
    @example Click_UART_I2C_SPI_DSPIC.mbas
    @example Click_UART_I2C_SPI_AVR.mbas
    @example Click_UART_I2C_SPI_FT90x.mbas
    @example Click_UART_I2C_SPI_STM.mpas
    @example Click_UART_I2C_SPI_TIVA.mpas
    @example Click_UART_I2C_SPI_CEC.mpas
    @example Click_UART_I2C_SPI_KINETIS.mpas
    @example Click_UART_I2C_SPI_MSP.mpas
    @example Click_UART_I2C_SPI_PIC.mpas
    @example Click_UART_I2C_SPI_PIC32.mpas
    @example Click_UART_I2C_SPI_DSPIC.mpas
    @example Click_UART_I2C_SPI_AVR.mpas
    @example Click_UART_I2C_SPI_FT90x.mpas
*/                                                                     /** @} */
/* -------------------------------------------------------------------------- */
/*
  __uarti2cspi_driver.h

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
