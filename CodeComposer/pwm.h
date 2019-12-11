
#ifndef PWM_H_

#define PWM_H_



/* Configure TIMER_A0 to produce PWM waveform*/


void config_pwm_timer(void);
void config_pwm_timer2(void);



/* @param uint8_t duty_cycle: 0-100, percentage of time ON */

void start_pwm(uint8_t duty_cycle, uint8_t pwm);

void start_pwm2(uint8_t duty_cycle, uint8_t pwm);

/* Stop Mode: clear all Mode Control bits, MC, in TAxCTL register */

void stop_pwm(void);

/* Config P2.4 to output TA0.1 waveform */

 void config_pwm_gpio(void);

 void config_pwm_gpio2(void);
 // for second servo
 void delay(void);

#endif
