#include <stdio.h>
#include "msp.h"
#include "i2c.h"
#include "ICM20948.h"
#include "AK09916.h"
#include "rotation.h"
#include "pwm.h"

/**
 * main.c
 */
void main(void)
{
	WDT_A->CTL = WDT_A_CTL_PW | WDT_A_CTL_HOLD;		// stop watchdog timer

	config_i2c();
    	config_pwm_timer_a0(); //configures Timer_A0
	config_pwm_timer_a2(); //configures Timer_A2

	config_pwm_gpio_p2_4(); //Configures P2.4 using Timer_A0 for top servo
	config_pwm_gpio_p5_6(); //Configures P5.6 using Timer_A2 for bottom servo

	//config_AK09916();
	//read_magnetometer_x();

    config_ICM20948();
    uint8_t pwm;
	double roll_to_pwm;
	while(1){
	    roll_to_pwm = roll();
	 if (roll_to_pwm<=15 || roll_to_pwm>=1){
	        pwm = roll_conversion(roll_to_pwm);
	        start_pwm_p2_4(pwm);
	 }
	}
}
