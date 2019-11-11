/*
 * arducam.c
 *
 *  Created on: Nov 7, 2019
 *      Author: phaed
 */
#include "msp.h"
#include "i2c"
#include "arducam.h"
#include "timerA0int.h"

#define start_bit
#define arducam_slave                    0x90

#define JPEG                             0x10


//Debugging
#define product_reg_id                   0x00
#define product_reg_val                  0x1519

#define SYSTEM_CLOCK        3000000  // [Hz] system_msp432p401r.c

void config_arducam() {
       TA0CTL &= ~0x0030; //Sets the timer to stop mode (halts timer)
       TA0CTL |= 0b100; // Clears the previous values of the timer
       TA0CTL &= ~0b100000000;
       TA0CCTL1 |= 0x0E0; //sets to mode 7 (reset set)


}
