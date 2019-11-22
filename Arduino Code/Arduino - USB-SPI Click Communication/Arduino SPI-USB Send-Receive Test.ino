#include<SPI.h> 
#define LED LED_BUILTIN  //Onboard LED
#define bufsize 8 //only allow 8 bits at a time

/*
 * Arduino UNO pin mapping:
 * 
 * MISO/SDO - pin 11
 * MOSI/SDI - pin 12
 * SCK - pin 13
 * SS/CS - pin 10
 * 
 * USB-SPI Click pin mapping:
 * 
 * MISO - SDO
 * MOSI - SDI
 * SCK - SCK
 * SS/CS - GPIO 2
 * 
 * This code is used to test the integration of the USB-SPI click with an Arduino. Bits are sent through the USB-SPI terminal on the computer
 * which is configured as the SPI master. The Arduino then receives the bits, indicated instantaneously by the built in LED's blink. 
 * The bits are then sent back to the computer through the USB-SPI click. The bits that were sent then appear in the Rx part of the 
 * terminal, showing reception of the data and sending.
 * 
 */

/*
 * To do:
 * - Setup buffer
 * - Solve weird problem w/ logic analyzer (something to do with chip select?)
 */
volatile boolean rcvd; //Boolean statement for if data was received
volatile byte slave_rcvd,Slavesend; 
int val;

int spi(unsigned int data);

void setup()

{
  Serial.begin(115200);
  pinMode(LED,OUTPUT);                    //Setting up of the onboard LED
  pinMode(MISO,OUTPUT);                   //Sets MISO as OUTPUT to send out to master

  SPCR |= _BV(SPE);                       //Turn on SPI in Slave Mode
  rcvd = false;                           //Set up initial condition of no data received
  SPI.setDataMode(SPI_MODE0);             //Sets up SPI mode to 0, trigger on posedge of clock
  SPI.attachInterrupt();                  //Global interupt made ON set for SPI commnucation
  
}

void loop(){ 
  
  if(rcvd)  {                           //If data was received
  digitalWrite(LED,HIGH);               //Turns LED on when receiving the data, instantaneous blink
  Serial.println("Tx LED OFF");   
  SPI.transfer(spi(val), bufsize); 
  delay(1);
  rcvd = false; //Reset back down to false
  }
  
  if(!rcvd) {
  digitalWrite(LED,LOW);               //Turns LED off
  Serial.println("Tx LED ON");         //Ready to transfer light
  SPI.transfer(val, sizeof(val)/sizeof(int));                          
  SPDR = Slavesend; 
  }
}

int spi(unsigned int data)
{
SPDR = data;                          // Begin transmission
while(!(SPSR & (1<<SPIF))){           // Wait to receive all bits
  digitalWrite(LED,HIGH); 
  } 
data = SPDR;
return(data);                     
} 
