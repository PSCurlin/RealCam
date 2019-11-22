#include <msp.h>
#include "pwm.h"


#define SYSTEM_CLOCK        3000000/4  // [Hz] system_msp432p401r.c
#define PWM_FREQUENCY       50   // [Hz] PWM frequency desired
#define CALC_PERIOD(X)      (SYSTEM_CLOCK / X) //calc # of ticks in period


void config_pwm_timer(void){
    TA0CTL &= ~0x0030; //Sets the timer to stop mode (halts timer)
    TA0CTL |= 0b100; // CLR
    TA0CTL |= 0b1000000000; // SMCLK set 1 in bit 9
    TA0CTL &= ~0b100000000; // SMCLK set 0 in bit 8
    TA0CTL |= 0b0010000000; // Increase the number of ticks by dividing value by 4 (set bit 7 to 1)
    TA0CCTL1 |= 0x0E0; //0b 1110 0000; //set mode 7 (reset set)

}

 //* * @param uint8_t duty_cycle: 0-100, percentage of time ON */
void start_pwm(uint8_t duty_cycle){
    TA0CTL |= 0b010000; //Sets the timer to up mode (starts timer)
    TA0CTL &= ~0b100000;
    TA0CCR0 = CALC_PERIOD(PWM_FREQUENCY);
    TA0CCR1 =  TA0CCR0 * duty_cycle / 100;
}

/* Stop Mode: clear all Mode Control bits, MC, in TAxCTL register */
void stop_pwm(void){
    TA0CTL &= ~0x30;//0011 0000
}

/* Config P2.4 to output TA0.1 waveform */
 void config_pwm_gpio(void){
     P2OUT &= ~0b10000; // clear pin 4 (not sure if this is necessary)
     //we want bit #4 to be 0, 1 in SEL1 and SEL0 respectively for primary module function
     P2SEL1 &= ~0b10000;
     P2SEL0 |= 0b10000;
     P2DIR |= 0b10000; //set as output 1000
 }



