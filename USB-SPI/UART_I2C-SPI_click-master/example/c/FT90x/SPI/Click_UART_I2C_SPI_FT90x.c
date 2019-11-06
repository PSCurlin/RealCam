/*
Example for UART_I2C_SPI Click

    Date          : okt 2018.
    Author        : Dusan Poluga

Test configuration FT90x :
    
    MCU                : FT900
    Dev. Board         : EasyFT90x v7 
    FT90x Compiler ver : v2.3.0.0

---

Description :

The application is composed of three sections :

- System Initialization - Initialize the GPIO and communication structures.
- Application Initialization - Initialize the communication interface and
                               configure the click board.  
- Application Task - Character data is sent trough the UART interface of the
                    click board. When all of the data has been sent the echo 
                    example is started.
*/

#include "Click_UART_I2C_SPI_types.h"
#include "Click_UART_I2C_SPI_config.h"

uint8_t str0[19]  = "System initialized";
uint8_t str1[22] = "+++++++++++++++++++++";
uint8_t str2[19] = "Entering echo mode";

void systemInit()
{
    mikrobus_gpioInit( _MIKROBUS1, _MIKROBUS_INT_PIN, _GPIO_INPUT );
    mikrobus_gpioInit( _MIKROBUS1, _MIKROBUS_RST_PIN, _GPIO_OUTPUT );
    mikrobus_gpioInit( _MIKROBUS1, _MIKROBUS_CS_PIN, _GPIO_OUTPUT );

    mikrobus_spiInit( _MIKROBUS1, &_UARTI2CSPI_SPI_CFG[0] );
    Delay_ms( 100 );
}

void applicationInit()
{
    uarti2cspi_spiDriverInit( (T_UARTI2CSPI_P)&_MIKROBUS1_GPIO, (T_UARTI2CSPI_P)&_MIKROBUS1_SPI );

    uarti2cspi_initAdvanced(19200,UARTI2CSPI_UART_8_BIT_DATA,UARTI2CSPI_UART_NOPARITY,UARTI2CSPI_UART_ONE_STOPBIT);
    uarti2cspi_interruptEnable(UARTI2CSPI_RXD_INT_EN|UARTI2CSPI_THR_EMPTY_INT_EN);

    Delay_ms(100);
    uarti2cspi_serialWrite(&str0[0],_LINE_PRINT);
    Delay_ms(1000);
}

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

void main()
{
    systemInit();
    applicationInit();

    while (1)
    {
        applicationTask();
    }
}
