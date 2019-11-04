/*
 * pwm.c
 *
 *  Created on: Nov 4, 2019
 *      Author: Nanu
 */
#include <msp.h>
#include "pwm.h"

#define SYSTEM_CLOCK        3000000  // [Hz] system_msp432p401r.c
#define PWM_FREQUENCY       100000   // [Hz] PWM frequency desired
#define CALC_PERIOD(X)      (SYSTEM_CLOCK / X) //calc # of ticks in period


void config_pwm_timer(void){
    TA0CTL |= 0b100; // Clears the previous values of the timer
    TA0CTL |= 0b1000000000; // Setting timer to SMCLK (set 1 in bit 9) and then set 0 in bit 8
    TA0CTL &= ~0b100000000;
    TA0CCTL1 |= 0x0E0; //sets to mode 7 (reset set)

}

 //* * @param uint8_t duty_cycle: 0-100, percentage of time ON */
void start_pwm(uint8_t duty_cycle){
    TA0CCR0 = CALC_PERIOD(PWM_FREQUENCY);   //Sets CCR0 to the # of ticks in period
    TA0CCR1 =  TA0CCR0 * duty_cycle / 100;  //Sets CCR1 to the duty cycle wanted
    TA0CTL |= 0b010000; //Sets timer to up mode
    TA0CTL &= ~0b100000;

}

/* Stop Mode: clear all Mode Control bits, MC, in TAxCTL register */
void stop_pwm(void){
    TA0CTL &= ~0x30; //Sets timer to stop mode (halts timer)
}

/* Config P2.4 to output TA0.1 waveform */
 void config_pwm_gpio(void){
     P2OUT &= ~0b10000; // Clears previous values of the pin
     P2SEL1 &= ~0b10000;//Sets Port 2 to primary module function
     P2SEL0 |= 0b10000;
     P2DIR |= 0b10000; //Sets pin as output
 }
