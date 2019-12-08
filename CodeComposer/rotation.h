/*
 * rotation.h
 *
 *  Created on: Dec 7, 2019
 *      Author: phaed
 */

#ifndef ROTATION_H_
#define ROTATION_H_

#include <stdio.h>
#include "i2c.h"
#include "msp.h"
#include "ICM20948.h"

double roll(void);
double pitch(void);
double yaw(void);


uint8_t roll_conversion(double input);

#endif /* ROTATION_H_ */
