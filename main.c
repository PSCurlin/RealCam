#include "msp.h"
#include "pwm.h"
#include "i2c.h"
#include "gpio.h"
#include "arducam.h"

void main(void)
{
    WDT_A->CTL = WDT_A_CTL_PW | WDT_A_CTL_HOLD;     //Stop watchdog timer
    config_i2c();                                   //Configure I2C SDA at P1.6 and SCL at P1.7

      //config_pwm_timer();
      //start_pwm(7);
      //config_pwm_gpio();
}
