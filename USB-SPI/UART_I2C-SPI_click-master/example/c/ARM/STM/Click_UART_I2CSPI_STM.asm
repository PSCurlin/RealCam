_systemInit:
;Click_UART_I2CSPI_STM.c,30 :: 		void systemInit()
SUB	SP, SP, #4
STR	LR, [SP, #0]
;Click_UART_I2CSPI_STM.c,32 :: 		mikrobus_gpioInit( _MIKROBUS1, _MIKROBUS_INT_PIN, _GPIO_INPUT );
MOVS	R2, #1
MOVS	R1, #7
MOVS	R0, #0
BL	_mikrobus_gpioInit+0
;Click_UART_I2CSPI_STM.c,33 :: 		mikrobus_gpioInit( _MIKROBUS1, _MIKROBUS_RST_PIN, _GPIO_OUTPUT );
MOVS	R2, #0
MOVS	R1, #1
MOVS	R0, #0
BL	_mikrobus_gpioInit+0
;Click_UART_I2CSPI_STM.c,34 :: 		mikrobus_gpioInit( _MIKROBUS1, _MIKROBUS_CS_PIN, _GPIO_OUTPUT );
MOVS	R2, #0
MOVS	R1, #2
MOVS	R0, #0
BL	_mikrobus_gpioInit+0
;Click_UART_I2CSPI_STM.c,36 :: 		mikrobus_spiInit( _MIKROBUS1, &_UARTI2CSPI_SPI_CFG[0] );
MOVW	R0, #lo_addr(__UARTI2CSPI_SPI_CFG+0)
MOVT	R0, #hi_addr(__UARTI2CSPI_SPI_CFG+0)
MOV	R1, R0
MOVS	R0, #0
BL	_mikrobus_spiInit+0
;Click_UART_I2CSPI_STM.c,39 :: 		mikrobus_logInit( _LOG_USBUART_A, 115200 );
MOV	R1, #115200
MOVS	R0, #32
BL	_mikrobus_logInit+0
;Click_UART_I2CSPI_STM.c,40 :: 		Delay_ms( 100 );
MOVW	R7, #20351
MOVT	R7, #18
NOP
NOP
L_systemInit0:
SUBS	R7, R7, #1
BNE	L_systemInit0
NOP
NOP
NOP
;Click_UART_I2CSPI_STM.c,41 :: 		}
L_end_systemInit:
LDR	LR, [SP, #0]
ADD	SP, SP, #4
BX	LR
; end of _systemInit
_applicationInit:
;Click_UART_I2CSPI_STM.c,43 :: 		void applicationInit()
SUB	SP, SP, #4
STR	LR, [SP, #0]
;Click_UART_I2CSPI_STM.c,45 :: 		uarti2cspi_spiDriverInit( (T_UARTI2CSPI_P)&_MIKROBUS1_GPIO, (T_UARTI2CSPI_P)&_MIKROBUS1_SPI );
MOVW	R1, #lo_addr(__MIKROBUS1_SPI+0)
MOVT	R1, #hi_addr(__MIKROBUS1_SPI+0)
MOVW	R0, #lo_addr(__MIKROBUS1_GPIO+0)
MOVT	R0, #hi_addr(__MIKROBUS1_GPIO+0)
BL	_uarti2cspi_spiDriverInit+0
;Click_UART_I2CSPI_STM.c,47 :: 		}
L_end_applicationInit:
LDR	LR, [SP, #0]
ADD	SP, SP, #4
BX	LR
; end of _applicationInit
_applicationTask:
;Click_UART_I2CSPI_STM.c,49 :: 		void applicationTask()
;Click_UART_I2CSPI_STM.c,54 :: 		}
L_end_applicationTask:
BX	LR
; end of _applicationTask
_main:
;Click_UART_I2CSPI_STM.c,56 :: 		void main()
;Click_UART_I2CSPI_STM.c,58 :: 		systemInit();
BL	_systemInit+0
;Click_UART_I2CSPI_STM.c,59 :: 		applicationInit();
BL	_applicationInit+0
;Click_UART_I2CSPI_STM.c,61 :: 		while (1)
L_main2:
;Click_UART_I2CSPI_STM.c,63 :: 		applicationTask();
BL	_applicationTask+0
;Click_UART_I2CSPI_STM.c,64 :: 		}
IT	AL
BAL	L_main2
;Click_UART_I2CSPI_STM.c,65 :: 		}
L_end_main:
L__main_end_loop:
B	L__main_end_loop
; end of _main
