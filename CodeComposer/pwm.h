
#ifndef PWM_H_

#define PWM_H_



/* Configure TIMER_A0 to produce PWM waveform*/


void config_pwm_timerA0(void);
void config_pwm_timerA2(void);



/* @param uint8_t duty_cycle: 0-100, percentage of time ON */

void start_pwm2_4(uint8_t pwm);

void start_pwm5_6(uint8_t pwm);

/* Stop Mode: clear all Mode Control bits, MC, in TAxCTL register */

void stop_pwm(void);

/* Config P2.4 to output TA0.1 waveform */

 void config_pwm_gpio2_4(void);

 void config_pwm_gpio5_6(void);
 // for second servo
 void delay(void);

#endif
