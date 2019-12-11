
#ifndef PWM_H_

#define PWM_H_



/* Configure TIMER_A0 */
void config_pwm_timer(void);

/* Configure TIMER_A2 */
void config_pwm_timer2(void);


 
/* Calculates number of ticks required for duty cycle and frequency of Timer_A0*/
void start_pwm_p2_4(uint8_t pwm);

/* Calculates number of ticks required for duty cycle and frequency of Timer_A2*/
void start_pwm_p5_6(uint8_t pwm);


/* Config P2.4 to output TA0.1 waveform */
 void config_pwm_gpio_p2_4(void);

/* Config P5.6 to output TA2.1 waveform */
 void config_pwm_gpio_p5_6(void);

#endif
