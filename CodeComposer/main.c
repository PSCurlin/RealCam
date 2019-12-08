#include "msp.h"
#include "pwm.h"
//#include "i2c.h"
//#include "gpio.h"
//#include "arducam.h"

void main(void){
    WDT_A->CTL = WDT_A_CTL_PW | WDT_A_CTL_HOLD;     //Stop watchdog timer
    //config_i2c();                                   //Configure I2C SDA at P1.6 and SCL at P1.7

    //1/55 = lowfrq
    //1/160 = highfrq
    //(lowfrq-highfrq)/14 = sh
    //lowfrq-sh*(roll-1) = pwm


    double input = 1;
    double low = 1/55;
    double high = 1/160;
    double scalar = (low=high)/14;
    uint8_t pwm = 1/(low - scalar*(input-1));


      config_pwm_timer();
      config_pwm_timer2();
      //start_pwm(7,28.5);
      //start_pwm2(7,49.5);
      config_pwm_gpio();
      config_pwm_gpio2();

      //delay();//4s delay

     // while(1){
      start_pwm2(7,pwm);
          //delay();
          //start_pwm2(7, 55);
          //delay();
          //start_pwm(7,160);
          //delay();
          //start_pwm2(7,160);
          //delay();
      //}

      //start_pwm(7,34);
      /*start_pwm2(7,28.5);

      delay();

      start_pwm(7,46.5);
      start_pwm2(7,160);

      delay();

      start_pwm(7,70);
      start_pwm2(7,70);

      delay();

      start_pwm(7,160);
      start_pwm2(7,34);

*/
}
