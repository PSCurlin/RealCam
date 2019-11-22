#ifndef PWM_H_
#define PWM_H_

/* Configure TIMER_A0 to produce PWM waveform
  * TODO: reset R (timer counter) register
  * - TODO: select SMCLK (3MHz) in the CTL register
  * * - TODO: select reset/set output mode for T0.1 timer using CCTL[1]  */
void config_pwm_timer(void);
/*  - TODO: Start PWM signal on Pin XX at duty_cycle 100kHz,
 * *    Note: the DRV2605L PWM input frequency is XXXX
 * *  - TODO: calculate and set the amount of ticks needed in CCR
 * *  - TODO: enable/start timer (UP mode)
 * *  - TODO: Counting and then reset
 * * @param uint8_t duty_cycle: 0-100, percentage of time ON */
void start_pwm(uint8_t duty_cycle);
/* Stop Mode: clear all Mode Control bits, MC, in TAxCTL register */
void stop_pwm(void);
/* Config P2.4 to output TA0.1 waveform */
 void config_pwm_gpio(void);
#endif
