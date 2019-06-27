#include <Arduino.h>
#include <SPI.h>
#include "Adafruit_BLE.h"
#include "Adafruit_BluefruitLE_SPI.h"
#include "Adafruit_BluefruitLE_UART.h"
#include "BluefruitConfig.h"
#if SOFTWARE_SERIAL_AVAILABLE
  #include <SoftwareSerial.h>
#endif
/*------------------------------------------------*/

#define MINIMUM_FIRMWARE_VERSION    "0.6.6"
#define MODE_LED_BEHAVIOUR          "MODE"

Adafruit_BluefruitLE_SPI ble(BLUEFRUIT_SPI_CS, BLUEFRUIT_SPI_IRQ, BLUEFRUIT_SPI_RST);


int stat = 0;
bool up = true;

int delayTime = 1000;

void setup() {
  Serial.begin(115200);
  setupBLE();
}

void loop() {
  if(stat == 0) {
    up = true;
    stat = 1;
    delayTime = 1000;  
  } else if (stat == 1) {
    if(up) {
      stat = 2;
      delayTime = 5000;
    }
    else {
      stat = 0;
      delayTime = 1000;
    }
  } else if (stat == 2) {
    up = false;
    stat = 1;
    delayTime = 1000;
  }
  
  // Send characters to Bluefruit
  ble.print("AT+BLEUARTTX=");
  ble.println(stat);

  Serial.println(stat);    
  
  // check response stastus
  if (! ble.waitForOK() ) {
     Serial.println(F("Failed to send?"));
  }   
  
  delay(delayTime);
}

void setupBLE() {
    /* Initialise the module */
  Serial.print(F("Initialising the Bluefruit LE module: "));

  if ( !ble.begin(VERBOSE_MODE) ){
    error(F("Couldn't find Bluefruit, make sure it's in CoMmanD mode & check wiring?"));
  }
  Serial.println( F("OK!") );

  /* Disable command echo from Bluefruit */
  ble.echo(false);

  Serial.println("Requesting Bluefruit info:");
  /* Print Bluefruit information */
  ble.info();

  Serial.println(F("Please use Adafruit Bluefruit LE app to connect in UART mode"));
  Serial.println();

  ble.verbose(false);  // debug info is a little annoying after this point!

  /* Wait for connection */
  Serial.println("Connecting...");
  while (! ble.isConnected()) {
      delay(500);
  }
  Serial.println("Connected!");

  // LED Activity command is only supported from 0.6.6
  if ( ble.isVersionAtLeast(MINIMUM_FIRMWARE_VERSION) ){
    // Change Mode LED Activity
    Serial.println(F("******************************"));
    Serial.println(F("Change LED activity to " MODE_LED_BEHAVIOUR));
    ble.sendCommandCheckOK("AT+HWModeLED=" MODE_LED_BEHAVIOUR);
    Serial.println(F("******************************"));
  }
}

void error(const __FlashStringHelper*err) {
  Serial.println(err);
  while (1);
}
