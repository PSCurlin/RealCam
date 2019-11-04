/*
 * pwm.h
 *
 *  Created on: Nov 4, 2019
 *      Author: Nanu
 */

#ifndef PWM_H_
#define PWM_H_


void config_pwm_timer(void);
void start_pwm(uint8_t duty_cycle);
void stop_pwm(void);
 void config_pwm_gpio(void);

#endif /* PWM_H_ */
