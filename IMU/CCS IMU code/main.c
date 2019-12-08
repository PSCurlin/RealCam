#include <rotation.h>
#include "msp.h"
#include "i2c.h"
#include "ICM20948.h"
#include "AK09916.h"
#include "rotation.h"

/**
 * main.c
 */
void main(void)
{
	WDT_A->CTL = WDT_A_CTL_PW | WDT_A_CTL_HOLD;		// stop watchdog timer
	config_i2c();
    //config_AK09916();
	//read_magnetometer_x();
	config_ICM20948();


	while(1){

	//read_acceleration_y();
	//read_acceleration_z();
	//read_gyroscope_x();
roll();
	}


}
