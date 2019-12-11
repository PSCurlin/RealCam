/*
 * AK09916.c
 *
 *  Created on: Dec 6, 2019
 *      Author: phaed
 */
/*this function allows use of magnetometer and uses I2C communication protocol*/
#include <stdio.h> //include libraries
#include "i2c.h"
#include "msp.h"
#include "AK09916.h"

void config_AK09916(void){
    int i;

    uint8_t whoami = read_register(AK09916,WHO_AM_I); /*Reads WHO_AM_I register to indicate to user which device is being accessed.
The value for ICM-20948 is 0xEA.*/
    printf("Who am I? This is device 0x%x.\n",whoami); //allows you to ensure device is communicating
    for(i=0; i<1000; i++);
    uint8_t mode = read_register(AK09916,CNTL2);//AK09916 is the address for the magnetometer using the CNTL2 register to enable read/write
    printf("Mode on boot: 0x%x.\n",mode); 
    for(i=0; i<1000; i++);
    write_register(AK09916,CNTL2,00010); //we use address “00010”: Continuous measurement mode 1 
    mode = read_register(AK09916,CNTL2); //now the register has been written we can read the value for CNTL2
    printf("New mode boot: 0x%x.\n",mode); //display value
}

void read_magnetometer_x(void){
    uint8_t hxh;   //HXH will collect an 8 bit value X-axis data
      while(1){
          hxh = read_register(AK09916,HXH);
          printf("magnetometer_x = %u\n",hxh);
      }
}

