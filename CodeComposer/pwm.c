#include <msp.h>
#include "pwm.h"
#include <stdio.h>

/* 28.5 Hz is 0 degree
 * 34 Hz is 45 degree
 * 46.5 Hz is 90 degree / 49.57
 * 70 Hz is 135 degree  /75?
 * 160 Hz is 180 degree
 *
 *P2.4 top motor (bottom on pcb)
 *135 is 45 up
 *81-83 is straight ahead front
 *55 is lowest 30 degrees lower than straight
 *53 is -35
 *55 is -30
 *61 is -25
 *65 is -20
 *
 *75 is -10
 *
 *P5.6 bottom motor (top on pcb)
 *28.5 is looking left
 *
 *47.8 is looking straight
 *
 *148 is looking right
 *
 *
 * bottom is P5.6
 */


#define SYSTEM_CLOCK        3000000/4  // [Hz] system_msp432p401r.c
//#define PWM_FREQUENCY       160   // [Hz] PWM frequency desired
#define CALC_PERIOD(X)      (SYSTEM_CLOCK / X) //calc # of ticks in period



void config_pwm_timer2(void){
    TA0CTL &= ~0x0030; //Sets the timer to stop mode (halts timer)
    TA0CTL |= 0b100; // CLR
    TA0CTL |= 0b1000000000; // SMCLK set 1 in bit 9
    TA0CTL &= ~0b100000000; // SMCLK set 0 in bit 8
    TA0CTL |= 0b0010000000; // Increase the number of ticks by dividing value by 4 (set bit 7 to 1)
    TA0CCTL1 |= 0x0E0; //0b 1110 0000; //set mode 7 (reset set)
}

void config_pwm_timer(void){
    TA2CTL &= ~0x0030; //Sets the timer to stop mode (halts timer)
    TA2CTL |= 0b100; // CLR
    TA2CTL |= 0b1000000000; // SMCLK set 1 in bit 9
    TA2CTL &= ~0b100000000; // SMCLK set 0 in bit 8
    TA2CTL |= 0b0010000000; // Increase the number of ticks by dividing value by 4 (set bit 7 to 1)
    TA2CCTL1 |= 0x0E0; //0b 1110 0000; //set mode 7 (reset set)
}

//* * @param uint8_t duty_cycle: 0-100, percentage of time ON */

void start_pwm2(uint8_t duty_cycle, uint8_t pwm){
    TA0CTL |= 0b010000; //Sets the timer to up mode (starts timer)
    TA0CTL &= ~0b100000;
    TA0CCR0 = CALC_PERIOD(pwm);
    TA0CCR1 =  TA0CCR0 * duty_cycle / 100;
}

void start_pwm(uint8_t duty_cycle, uint8_t pwm){
    TA2CTL |= 0b010000; //Sets the timer to up mode (starts timer)
    TA2CTL &= ~0b100000;
    TA2CCR0 = CALC_PERIOD(pwm);
    TA2CCR1 =  TA2CCR0 * duty_cycle / 100;
}



/* Config P2.4 to output TA0.1 waveform */
 void config_pwm_gpio2(void){
     P2OUT &= ~0b10000; // clear pin 4 (not sure if this is necessary)
     //we want bit #4 to be 0, 1 in SEL1 and SEL0 respectively for primary module function
     P2SEL1 &= ~0b10000;
     P2SEL0 |= 0b10000;
     P2DIR |= 0b10000; //set pin 4  as output
 }

 /* Config P5.6 to output TA2.1 waveform */
 void config_pwm_gpio(void){
     P5OUT &= ~0b1000000; // clear pin 0 (not sure if this is necessary)
     //we want bit #0 to be 0, 1 in SEL1 and SEL0 respectively for primary module function
     P5SEL1 &= ~0b1000000;
     P5SEL0 |= 0b1000000;
     P5DIR |= 0b1000000; //set as output 1
 }

 //Delay of 4s
 void delay(void){
     int i=0;
               while(i<700000)
               {
                  i++;
               }

 }

