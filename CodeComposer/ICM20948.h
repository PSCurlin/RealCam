/*
 * ICM-20948.h
 *
 *  Created on: Dec 5, 2019
 *      Author: phaed
 */

#ifndef ICM20948_H_
#define ICM20948_H_

#include <stdio.h>
#include "i2c.h"
#include "msp.h"
#include "ICM20948.h"

#define PWR_MGMT_1 0x06
#define LP_CONFIG 0x05
#define INT_PIN_CFG 0x0F
#define ACCEL_CONFIG 0x14
#define GYRO_CONFIG_1 0x01

#define ACCEL_XOUT_H 0x2D
#define ACCEL_XOUT_L 0x2E

#define ACCEL_YOUT_H 0x2F
#define ACCEL_YOUT_L 0x2E

#define ACCEL_ZOUT_H 0x31
#define ACCEL_ZOUT_L 0x30


#define GYRO_XOUT_H 0x33
#define GYRO_XOUT_L 0x32

#define GYRO_YOUT_H 0x35
#define GYRO_YOUT_L 0x34

#define GYRO_ZOUT_H 0x37
#define GYRO_ZOUT_L 0x36

#define ICM20948 0x69                //Pin AD0 is logic high
#define WHO_AM_I 0x00

void config_ICM20948(void);

uint16_t read_acceleration_x(void);
uint16_t read_acceleration_y(void);
uint16_t read_acceleration_z(void);

uint16_t read_gyroscope_x(void);
uint16_t read_gyroscope_y(void);
uint16_t read_gyroscope_z(void);

#endif /* ICM20948_H_ */
