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
    double const coeff = 0.5;


    ax = 1-coeff*ax+coeff*read_acceleration_x();
    ax /=(2048);

//    ax *= 0.015708;
    ay = 1-coeff*ay+coeff*read_acceleration_y();
    ay/= (2048);

  //  ay *= 0.015708;
    az = 1-coeff*az+coeff*read_acceleration_z();
    az /= (2048);
  /*  ax = read_acceleration_x();
    ay = read_acceleration_y();
    az = read_acceleration_z();*/
    //az *= 0.015708;

    /*    ax=read_acceleration_x();
    ay=read_acceleration_y();
    az=read_acceleration_z();*/
    int i;
    for(i=0;i<480;i++);
    roll = 180*atan2(ay,sqrt(ax*ax+az*az))/M_PI;
    roll = round(roll);

    printf("roll = %f \n",roll);

    return roll;
}

double pitch(void){
    double pitch;    double const coeff = 0.5;

    ax = 1-coeff*ax+coeff*read_acceleration_x();
    ax /=(2048);
    ay = 1-coeff*ay+coeff*read_acceleration_y();
    ay/= (2048);
    az = 1-coeff*az+coeff*read_acceleration_z();
    az /= (2048);
    int i;
    for(i=0;i<480;i++);
    pitch = 180*atan2(ax,sqrt(ay*ay+az*az))/M_PI;

    printf("pitch = %f \n ",pitch);

    return pitch;
}


uint8_t roll_conversion(double input){
    double low = 1;
    low /= 55;
    double high = 1;
    high /=160;
    double diff = low-high;
    diff /= 14;
    double pwm = 1/(low-diff*(input-1));
//    printf(" freq = %f\n",pwm);
    return pwm;
}

uint8_t pitch_conversion(double input){
    double low = 1;
    low /= 50;
    double high = 1;
    high /=100;
    double diff = low-high;
    diff /= 14;
    double pwm = 1/(low-diff*(input-1));
   // printf(" freq = %f\n",pwm);
    return pwm;
}

