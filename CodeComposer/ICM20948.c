/*
 * ICM20948.c
 *
 *  Created on: Dec 5, 2019
 *      Author: phaed
 */

#include <stdio.h>
#include "i2c.h"
#include "msp.h"
#include "ICM20948.h"

void config_ICM20948(void){

   int i;
   for (i=0; i<10000;i++); // Start-up time
   printf("Connecting to ICM20948...\n");
   for(i=0; i<100000; i++);
   printf("...\n");
   for(i=0; i<100; i++);
   printf("...\n");
   for(i=0; i<100; i++);

   uint8_t whoami = read_register(ICM20948,WHO_AM_I); //Reads WHO_AM_I register
   printf("Who am I? This is device 0x%x.\n",whoami);

   write_register(ICM20948,PWR_MGMT_1,0x01);
   write_register(ICM20948,LP_CONFIG,0x40);
   write_register(ICM20948,INT_PIN_CFG,0x02);
   write_register(ICM20948,ACCEL_CONFIG,0x38);
   write_register(ICM20948,GYRO_CONFIG_1,0x38);

   write_register(ICM20948,0x20,0x01);
 }

uint16_t read_acceleration_x(void){
   uint8_t axh,axl;
   uint16_t ax;
   ax=0;
   axh = read_register(ICM20948,ACCEL_XOUT_H);
   axl = read_register(ICM20948,ACCEL_XOUT_L);
  // ax = axh<<8;
   ax = axl;
  //printf("ax = %u\n",ax); //Debugging purposes
   return ax;
}

uint16_t read_acceleration_y(void){
    uint8_t ayh,ayl;
    uint16_t ay;
    ay=0;
    ayh = read_register(ICM20948,ACCEL_YOUT_H);
    ayl = read_register(ICM20948,ACCEL_YOUT_L);
  //  ay = ayh<<8;
     ay = ayl;
    //printf("ay = %u\n",ay); //Debugging purposes
    return ay;
}

uint16_t read_acceleration_z(void){
    uint8_t azh,azl;
    uint16_t az;
    az=0;
    azh = read_register(ICM20948,ACCEL_ZOUT_H);
    azl = read_register(ICM20948,ACCEL_ZOUT_L);
   // az = azh<<8;
    az = azl;
//    printf("az = %u\n",az); //Debugging purposes
    return az;
}

uint16_t read_gyroscope_x(void){
    uint8_t gxh,gxl;
    uint16_t gx;

    gxh = read_register(ICM20948,GYRO_XOUT_H);
    gxl = read_register(ICM20948,GYRO_XOUT_L);
    gx = gxh<<4;
    gx += gxl;
    //printf("gx = %u\n",gx); //Debugging purposes
    return gx;
}

uint16_t read_gyroscope_y(void){
    uint8_t gyh,gyl;
    uint16_t gy;

    gyh = read_register(ICM20948,GYRO_YOUT_H);
    gyl = read_register(ICM20948,GYRO_YOUT_L);
    gy = gyh<<4;
    gy += gyl;
    //printf("gy = %u\n",gx); //Debugging purposes
    return gy;
}

uint16_t read_gyroscope_z(void){
    uint8_t gzh,gzl;
    uint16_t gz;

    gzh = read_register(ICM20948,GYRO_ZOUT_H);
    gzl = read_register(ICM20948,GYRO_ZOUT_L);
    gz = gzh<<4;
    gz += gzl;
    //printf("gz = %u\n",gz); //Debugging purposes
    return gz;
}


