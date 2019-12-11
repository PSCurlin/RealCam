#include <msp.h>
#include "pwm.h"
#include <stdio.h>


#define SYSTEM_CLOCK        3000000/4  // [Hz] system_msp432p401r.c
#define CALC_PERIOD(X)      (SYSTEM_CLOCK / X) //calc # of ticks in period


/* Configures TimerA0 to SMCLK and divides clock down to increase number of ticks*/
void config_pwm_timer_a0(void){
    TA0CTL &= ~0x0030; //Sets the timer to stop mode (halts timer)
    TA0CTL |= 0b100; // CLR
    TA0CTL |= 0b1000000000; // SMCLK set 1 in bit 9
    TA0CTL &= ~0b100000000; // SMCLK set 0 in bit 8
    TA0CTL |= 0b0010000000; // Increase the number of ticks by dividing value by 4 (set bit 7 to 1)
    TA0CCTL1 |= 0x0E0; //set mode to mode 7 (reset/set)
}

/* Configures TimerA2 to SMCLK and divides clock down to increase number of ticks*/
void config_pwm_timer_a2(void){
    TA2CTL &= ~0x0030; //Sets the timer to stop mode (halts timer)
    TA2CTL |= 0b100; // CLR
    TA2CTL |= 0b1000000000; // SMCLK set 1 in bit 9
    TA2CTL &= ~0b100000000; // SMCLK set 0 in bit 8
    TA2CTL |= 0b0010000000; // Increase the number of ticks by dividing value by 4 (set bit 7 to 1)
    TA2CCTL1 |= 0x0E0; //set mode to mode 7 (reset/set)
}

/* Starts timerA0 and calculates amount of ticks required for 
counter to count up to for each duty cycle and frequency*/
void start_pwm_p2_4(uint8_t pwm){
    TA0CTL |= 0b010000; //Sets the timer to up mode (starts timer)
    TA0CTL &= ~0b100000;
    TA0CCR0 = CALC_PERIOD(pwm);
    TA0CCR1 =  TA0CCR0 * 7 / 100;
}

/* Starts timerA2 and calculates amount of ticks required for 
counter to count up to for each duty cycle and frequency*/
void start_pwm_p5_6(uint8_t pwm){
    TA2CTL |= 0b010000; //Sets the timer to up mode (starts timer)
    TA2CTL &= ~0b100000;
    TA2CCR0 = CALC_PERIOD(pwm);
    TA2CCR1 =  TA2CCR0 * 7 / 100;
}



/* Configures P2.4 to output TA0.1 waveform (pwm for top motor) */
 void config_pwm_gpio_p2_4(void){
     P2OUT &= ~0b10000; // clear pin 4 (not sure if this is necessary)
     //we want bit #5 to be 0, 1 in SEL1 and SEL0 respectively for primary module function
     P2SEL1 &= ~0b10000;
     P2SEL0 |= 0b10000;
     P2DIR |= 0b10000; //set pin 4 direction as output
 }

 /* Config P5.6 to output TA2.1 waveform (pwm for bottom motor) */
 void config_pwm_gpio_p5_6(void){
     P5OUT &= ~0b1000000; // clear pin 6
     //we want bit #7 to be 0, 1 in SEL1 and SEL0 respectively for primary module function
     P5SEL1 &= ~0b1000000;
     P5SEL0 |= 0b1000000;
     P5DIR |= 0b1000000; //set pin 6 direction as output
 }

