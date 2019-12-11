/*
 * AK09916.h
 * Magnetometer
 *  Created on: Dec 6, 2019
 *      Author: phaed
 */

#ifndef AK09916_H_
#define AK09916_H_

#define AK09916      0x0C

#define WHO_AM_I     0x00
#define CNTL2        0x31
#define HXH          0x11
#define HYH          0x14
#define HZH          0x16

void config_AK09916(void);
void read_magnetometer_x(void);

#endif /* AK09916_H_ */
