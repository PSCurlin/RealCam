#line 1 "D:/Clicks_git/U/UART_I2CSPI_Click/SW/example/c/ARM/STM/Click_UART_I2CSPI_STM.c"
#line 1 "d:/clicks_git/u/uart_i2cspi_click/sw/example/c/arm/stm/click_uart_i2cspi_types.h"
#line 1 "c:/users/public/documents/mikroelektronika/mikroc pro for arm/include/stdint.h"





typedef signed char int8_t;
typedef signed int int16_t;
typedef signed long int int32_t;
typedef signed long long int64_t;


typedef unsigned char uint8_t;
typedef unsigned int uint16_t;
typedef unsigned long int uint32_t;
typedef unsigned long long uint64_t;


typedef signed char int_least8_t;
typedef signed int int_least16_t;
typedef signed long int int_least32_t;
typedef signed long long int_least64_t;


typedef unsigned char uint_least8_t;
typedef unsigned int uint_least16_t;
typedef unsigned long int uint_least32_t;
typedef unsigned long long uint_least64_t;



typedef signed long int int_fast8_t;
typedef signed long int int_fast16_t;
typedef signed long int int_fast32_t;
typedef signed long long int_fast64_t;


typedef unsigned long int uint_fast8_t;
typedef unsigned long int uint_fast16_t;
typedef unsigned long int uint_fast32_t;
typedef unsigned long long uint_fast64_t;


typedef signed long int intptr_t;
typedef unsigned long int uintptr_t;


typedef signed long long intmax_t;
typedef unsigned long long uintmax_t;
#line 1 "d:/clicks_git/u/uart_i2cspi_click/sw/example/c/arm/stm/click_uart_i2cspi_config.h"
#line 1 "d:/clicks_git/u/uart_i2cspi_click/sw/example/c/arm/stm/click_uart_i2cspi_types.h"
#line 4 "d:/clicks_git/u/uart_i2cspi_click/sw/example/c/arm/stm/click_uart_i2cspi_config.h"
const uint32_t _UARTI2CSPI_SPI_CFG[ 2 ] =
{
 _SPI_FPCLK_DIV256,
 _SPI_FIRST_CLK_EDGE_TRANSITION |
 _SPI_CLK_IDLE_LOW |
 _SPI_MASTER |
 _SPI_MSB_FIRST |
 _SPI_8_BIT |
 _SPI_SSM_ENABLE |
 _SPI_SS_DISABLE |
 _SPI_SSI_1
};



const uint32_t _UARTI2CSPI_I2C_CFG[ 1 ] =
{
 100000
};
#line 1 "d:/clicks_git/u/uart_i2cspi_click/sw/library/__uarti2cspi_driver.h"
#line 1 "c:/users/public/documents/mikroelektronika/mikroc pro for arm/include/stdint.h"
#line 72 "d:/clicks_git/u/uart_i2cspi_click/sw/library/__uarti2cspi_driver.h"
void uarti2cspi_spiDriverInit( const uint8_t*  gpioObj,  const uint8_t*  spiObj);


void uarti2cspi_i2cDriverInit( const uint8_t*  gpioObj,  const uint8_t*  i2cObj, uint8_t slave);
#line 82 "d:/clicks_git/u/uart_i2cspi_click/sw/library/__uarti2cspi_driver.h"
void uarti2cspi_gpioDriverInit( const uint8_t*  gpioObj);
#line 30 "D:/Clicks_git/U/UART_I2CSPI_Click/SW/example/c/ARM/STM/Click_UART_I2CSPI_STM.c"
void systemInit()
{
 mikrobus_gpioInit( _MIKROBUS1, _MIKROBUS_INT_PIN, _GPIO_INPUT );
 mikrobus_gpioInit( _MIKROBUS1, _MIKROBUS_RST_PIN, _GPIO_OUTPUT );
 mikrobus_gpioInit( _MIKROBUS1, _MIKROBUS_CS_PIN, _GPIO_OUTPUT );

 mikrobus_spiInit( _MIKROBUS1, &_UARTI2CSPI_SPI_CFG[0] );


 mikrobus_logInit( _LOG_USBUART_A, 115200 );
 Delay_ms( 100 );
}

void applicationInit()
{
 uarti2cspi_spiDriverInit( ( const uint8_t* )&_MIKROBUS1_GPIO, ( const uint8_t* )&_MIKROBUS1_SPI );

}

void applicationTask()
{



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
