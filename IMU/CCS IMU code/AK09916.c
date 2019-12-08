/*
 * AK09916.c
 *
 *  Created on: Dec 6, 2019
 *      Author: phaed
 */

#include <stdio.h>
#include "i2c.h"
#include "msp.h"
#include "AK09916.h"

void config_AK09916(void){
    int i;

    uint8_t whoami = read_register(AK09916,WHO_AM_I); //Reads WHO_AM_I register
    printf("Who am I? This is device 0x%x.\n",whoami);
    for(i=0; i<1000; i++);
    uint8_t mode = read_register(AK09916,CNTL2);
    printf("Mode on boot: 0x%x.\n",mode);
    for(i=0; i<1000; i++);
    write_register(AK09916,CNTL2,00010);
    mode = read_register(AK09916,CNTL2);
    printf("New mode boot: 0x%x.\n",mode);
}

void read_magnetometer_x(void){
    uint8_t hxh;
      while(1){
          hxh = read_register(AK09916,HXH);
          printf("magnetometer_x = %u\n",hxh);
      }
}

