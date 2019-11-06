![MikroE](http://www.mikroe.com/img/designs/beta/logo_small.png)

---

# UART_I2C_SPI Click

- **CIC Prefix**  : UARTI2CSPI
- **Author**      : Dusan Poluga
- **Verison**     : 1.0.0
- **Date**        : okt 2018.

---


### Software Support

We provide a library for the UART_I2C_SPI Click on our [LibStock](https://libstock.mikroe.com/projects/view/2608/uart-i2c-spi-click) 
page, as well as a demo application (example), developed using MikroElektronika 
[compilers](http://shop.mikroe.com/compilers). The demo can run on all the main 
MikroElektronika [development boards](http://shop.mikroe.com/development-boards).

**Library Description**

Library contains basinc functions for working with the click board.

Key functions :

- ```void uarti2cspi_init(uint16_t baud);``` - Function for initializing the click board.
- ```uint8_t uarti2cspi_uartRead();``` - Function for reading a character.
- ```void uarti2cspi_uartWrite(uint8_t wByte);``` - Fucntion for writing a character.

**Examples Description**

Description :

The application is composed of three sections :

- System Initialization - Initialize the GPIO and communication structures.
- Application Initialization - Initialize the communication interface and
                               configure the click board.  
- Application Task - Character data is sent trough the UART interface of the
                    click board. When all of the data has been sent the echo 
                    example is started.

```.c
void applicationTask()
{
    uint16_t counter_var;
    char text[5];


    for(counter_var=0;counter_var<10;counter_var++)
    {
        WordToStr(counter_var,&text[0]);
        Ltrim(&text[0]);
        uarti2cspi_serialWrite(&text[0],_LINE_PRINT);
    }

    uarti2cspi_serialWrite(&str1[0],_LINE_PRINT);
    uarti2cspi_serialWrite(&str2[0],_LINE_PRINT);
    uarti2cspi_uartWrite(0x00);

    while(1)
    {
        uarti2cspi_uartWrite(uarti2cspi_uartRead());
    }
}
```

The full application code, and ready to use projects can be found on our 
[LibStock](https://libstock.mikroe.com/projects/view/2608/uart-i2c-spi-click) page.

Other mikroE Libraries used in the example:

- Conversions library
- C_String library
- SPI library
- I2C library
- UART library

**Additional notes and informations**

Depending on the development board you are using, you may need 
[USB UART click](http://shop.mikroe.com/usb-uart-click), 
[USB UART 2 Click](http://shop.mikroe.com/usb-uart-2-click) or 
[RS232 Click](http://shop.mikroe.com/rs232-click) to connect to your PC, for 
development systems with no UART to USB interface available on the board. The 
terminal available in all Mikroelektronika 
[compilers](http://shop.mikroe.com/compilers), or any other terminal application 
of your choice, can be used to read the message.

---
---
