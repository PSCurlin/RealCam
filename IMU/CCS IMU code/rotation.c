/*
 * rotation.c
 *
 *  Created on: Dec 7, 2019
 *      Author: phaed
 */

#include "rotation.h"
#include "ICM20948.h"
#include "msp.h"
#include <stdio.h>
#include <math.h>
double ax=0;
double ay=0;
double az=0;

double roll(void){
    double roll;
  //  double ax,ay,az;

    ax = 1-0.5*ax+0.5*read_acceleration_x();
    ax *= 0.015708;
    ay = 1-0.5*ay+0.5*read_acceleration_y();
    ay *= 0.015708;
    az = 1-0.5*az+0.5*read_acceleration_z();
    az *= 0.015708;

    /*    ax=read_acceleration_x();
    ay=read_acceleration_y();
    az=read_acceleration_z();*/
    int i;
    for(i=0;i<100;i++);
    roll = 180*atan2(ay,sqrt(ax*ax+az*az))/M_PI;
    printf("roll = %f\n",roll);



    return roll;
}


