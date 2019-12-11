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

// Sets up the acceleration and the gyroscope of the IMU
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

   write_register(ICM20948,PWR_MGMT_1,0x01); // Clock select
   write_register(ICM20948,LP_CONFIG,0x40); //Resets LP_CONFIG
   write_register(ICM20948,INT_PIN_CFG,0x02); //Bypasses the master functionality
   write_register(ICM20948,ACCEL_CONFIG,0x38); // Enable DLPF, 16g
   write_register(ICM20948,GYRO_CONFIG_1,0x38); // Enable DLPF, 2000 dps
 }
// Reads acceleration values in the X axis
uint16_t read_acceleration_x(void){
   uint8_t axh,axl;
   uint16_t ax;
   ax=0;
   axh = read_register(ICM20948,ACCEL_XOUT_H); // Reads the high bit of the x-acceleration register
   axl = read_register(ICM20948,ACCEL_XOUT_L);// Reads the low bit of the x-acceleration register
  // ax = axh<<8; //Used to add the low bit and the high bit into one 16-bit integer
  // ax = ax+axl;
   ax = axl; //Only want to use the low bit for now
  //printf("ax = %u\n",ax); //Debugging purposes
   return ax; //Returns the raw acceleration in the x-axis
}

// Reads acceleration values in the y axis
uint16_t read_acceleration_y(void){
    uint8_t ayh,ayl;
    uint16_t ay;
    ay=0;
    ayh = read_register(ICM20948,ACCEL_YOUT_H); // Reads the high bit of the y-acceleration register
    ayl = read_register(ICM20948,ACCEL_YOUT_L); // Reads the low bit of the y-acceleration register
    // ay = ayh<<8 //Used to add the low bit and the high bit into one 16-bit integer
	// ay = ay+ayh;
    ay = ayl; //Only want to use the low bit for now
    //printf("ay = %u\n",ay); //Debugging purposes
    return ay; //Returns the raw acceleration in the y-axis
}

// Reads acceleration values in the Z axis
uint16_t read_acceleration_z(void){
    uint8_t azh,azl;
    uint16_t az;
    az=0;
    azh = read_register(ICM20948,ACCEL_ZOUT_H); // Reads the high bit of the z-acceleration register
    azl = read_register(ICM20948,ACCEL_ZOUT_L); // Reads the low bit of the z-acceleration register
    // az = azh<<8;  //Used to add the low bit and the high bit into one 16-bit integer
    // ax = az + azl;
    az = azl; //Only want to use the low bit for now
    // printf("az = %u\n",az); //Debugging purposes
    return az;	//Returns the raw acceleration in the z-axis
}

// Reads gyroscope values in the X axis
uint16_t read_gyroscope_x(void){
    uint8_t gxh,gxl;
    uint16_t gx;
	gx = 0;
    gxh = read_register(ICM20948,GYRO_XOUT_H); // Reads the high bit of the x-gyroscope register
    gxl = read_register(ICM20948,GYRO_XOUT_L); // Reads the low bit of the x-gyroscope register
    // gx = gxh<<4;	//Used to add the low bit and the high bit into one 16-bit integer
    gx += gxl;
    // printf("gx = %u\n",gx); //Debugging purposes
    return gx; //Returns the raw gyroscope in the x-axis
}

// Reads gyroscope values in the Y axis
uint16_t read_gyroscope_y(void){
    uint8_t gyh,gyl;
    uint16_t gy;
	gy = 0;
    gyh = read_register(ICM20948,GYRO_YOUT_H);	// Reads the high bit of the y-gyroscope register
    gyl = read_register(ICM20948,GYRO_YOUT_L);	// Reads the low bit of the y-gyroscope register
    // gy = gyh<<4; //Used to add the low bit and the high bit into one 16-bit integer
    gy += gyl;
    //printf("gy = %u\n",gx); //Debugging purposes
    return gy; //Returns the raw gyroscope in the y-axis
}

// Reads gyroscope values in the Z axis
uint16_t read_gyroscope_z(void){
    uint8_t gzh,gzl;
    uint16_t gz;
	gz = 0;
    gzh = read_register(ICM20948,GYRO_ZOUT_H); // Reads the high bit of the z-gyroscope register
    gzl = read_register(ICM20948,GYRO_ZOUT_L); // Reads the low bit of the z-gyroscope register
    // gz = gzh<<4; //Used to add the low bit and the high bit into one 16-bit integer
    gz += gzl;
    //printf("gz = %u\n",gz); //Debugging purposes
    return gz; //Returns the raw gyroscope in the z-axis
}


