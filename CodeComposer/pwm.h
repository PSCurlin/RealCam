
#ifndef PWM_H_

#define PWM_H_



/* Configure TIMER_A0 */
void config_pwm_timer(void);

/* Configure TIMER_A2 */
void config_pwm_timer2(void);


 
/* Calculates number of ticks required for duty cycle and frequency of Timer_A0*/
void start_pwm(uint8_t duty_cycle, uint8_t pwm);

/* Calculates number of ticks required for duty cycle and frequency of Timer_A2*/
void start_pwm2(uint8_t duty_cycle, uint8_t pwm);


/* Config P2.4 to output TA0.1 waveform */
 void config_pwm_gpio(void);

/* Config P5.6 to output TA2.1 waveform */
 void config_pwm_gpio2(void);
 
/* Function to delay 4s */
 void delay(void);

#endif
