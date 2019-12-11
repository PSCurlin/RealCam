/**
 * main.c
 *
 * Pins used:
 * P2.4 - Top servo PWM
 * P5.6 - Bottom servo PWM 
 * P1.6 - SDA
 * P1.7 - SCL
 *
 */
 
#include <stdio.h>
#include "msp.h"
#include "i2c.h"
#include "ICM20948.h"
#include "AK09916.h"
#include "rotation.h"
#include "pwm.h"

void main(void)
{
	WDT_A->CTL = WDT_A_CTL_PW | WDT_A_CTL_HOLD;		// stop watchdog timer

	config_i2c() //Configures the I2C pins on the MSP
    config_pwm_timer(); //Configures the PWM for the top servo
	config_pwm_timer2(); //Configures the PWM for the bottom servo

	config_pwm_gpio(); //Configures the PWM at pin 2.4
	config_pwm_gpio2(); //Configures the PWM at pin 5.6

	config_ICM20948(); //
	
	//config_AK09916(); //Used to setup the magnetometer
	//read_magnetometer_x(); //Reads the magnetometer x-direction
    
    uint8_t pwm2,pwm; 
	double roll_to_pwm2,pitch_to_pwm;
	
	while(1){
		
	 pitch_to_pwm = pitch();
	 roll_to_pwm2 = roll();
	 
	 if (roll_to_pwm2<=15 || roll_to_pwm2>=3){ //Checks to see if the IMU roll is within range for the servo
	        pwm2 = roll_conversion(roll_to_pwm2);
	        start_pwm2(7,pwm2);
	     }
	if (pitch_to_pwm<=41 || pitch_to_pwm>=46){ //Checks to see if the IMU pitch is within range for the servo
	             pwm = pitch_conversion(pitch_to_pwm);
	             start_pwm(7,pwm);
	 }
	}
}
