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
    config_pwm_timer();
	config_pwm_timer2();

	config_pwm_gpio2();
	config_pwm_gpio();

	//config_AK09916();
	//read_magnetometer_x();

    config_ICM20948();
    uint8_t pwm;
	double roll_to_pwm;
	while(1){
	    roll_to_pwm = roll();
	 if (roll_to_pwm<=15 || roll_to_pwm>=1){
	        pwm = roll_conversion(roll_to_pwm);
	        start_pwm2(7,pwm);
	 }
	}
}
