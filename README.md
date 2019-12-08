# RealCam

RealCam is a remote viewing device who's orientation can be controlled by the user's head position.

## Servos and IMU

Currently, these are the only items utilizing the MSP432 and are written in C.

## Camera

The camera interfaces with an Arduino UNO and uses Arduino code.

## USB-SPI interface

This interface is currently being tested by sending values through a terminal provided by MikroElektronica. The SPI protocol is tested with an Arduino which behaves as a slave.
