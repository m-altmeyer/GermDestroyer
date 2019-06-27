#include <Arduino.h>
#include <SPI.h>
#include <CapacitiveSensor.h>
#include <Wire.h>
#include <VL6180X.h>
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

CapacitiveSensor capSensor = CapacitiveSensor(11,A0);
VL6180X distSensor;

int capThresh = 500;
int distThresh= 180;

bool waterOn = false;
bool handThere = false;

int stat = 0;

long lastHandsWereThere=0;

void setup() {
  Serial.begin(115200);
  setupSensors();
  setupBLE();
}

void loop() {
    if(capSensor.capacitiveSensor(30) > capThresh) {
      waterOn = true;
      //Serial.println("Water is running!");
    } 
    else {
      waterOn = false;
    }
    
    if(distSensor.readRangeContinuousMillimeters() < distThresh) {
      handThere = true;
      lastHandsWereThere=millis();
      //Serial.println("Hands taken away!");
    } 
    else {
      if (millis()-lastHandsWereThere>800){
        handThere = false;
      }
    }

    int priorStat = stat;
    stat = 0;
    if(waterOn && !handThere) stat = 1;
    if(waterOn && handThere) stat = 2;
    if(!waterOn && handThere) stat = 3;

    if(stat != priorStat) { //New Status
       Serial.println(stat);
      
      // Send characters to Bluefruit
      ble.print("AT+BLEUARTTX=");
      ble.println(stat);

      // check response stastus
      if (! ble.waitForOK() ) {
        Serial.println(F("Failed to send?"));
      }   
    }
    
    ble.waitForOK();
    checkUserInput();
    delay(10);
}

void checkUserInput() {
  ble.println("AT+BLEUARTRX");
  ble.readline();
  if (strcmp(ble.buffer, "OK") == 0) {
    // no data
    return;
  }
  // Some data was found, its in the buffer
  Serial.print(F("[Recv] ")); Serial.println(ble.buffer);

  String bufferString(ble.buffer);
  bufferString.replace(" ", "");
  bufferString.toLowerCase();
  
  if(bufferString.startsWith("d:")) {
    String newThreshold = bufferString.substring(2, bufferString.length());
    distThresh = newThreshold.toInt();
    Serial.print("New distance threshold value: ");
    Serial.println(distThresh);
  } 
  else if (bufferString.startsWith("c:")) {
    String newThreshold = bufferString.substring(2, bufferString.length());
    capThresh = newThreshold.toInt();
    Serial.print("New capacitive threshold value: ");
    Serial.println(capThresh);
  }
}

void setupSensors() {
  Serial.println("Setting up sensors...");
  capSensor.set_CS_AutocaL_Millis(0xFFFFFFFF);

  Wire.begin();

  distSensor.init();
  distSensor.configureDefault();

  // Reduce range max convergence time and ALS integration
  // time to 30 ms and 50 ms, respectively, to allow 10 Hz
  // operation (as suggested by Table 6 ("Interleaved mode
  // limits (10 Hz operation)") in the datasheet).
  distSensor.writeReg(VL6180X::SYSRANGE__MAX_CONVERGENCE_TIME, 30);
  distSensor.writeReg16Bit(VL6180X::SYSALS__INTEGRATION_PERIOD, 50);

  distSensor.setTimeout(500);

   // stop continuous mode if already active
  distSensor.stopContinuous();
  // in case stopContinuous() triggered a single-shot
  // measurement, wait for it to complete
  delay(300);
  // start interleaved continuous mode with period of 100 ms
  distSensor.startInterleavedContinuous(100);

  int tempCapThresh = 500;
  int tempDistThresh = 255;
  if(tempCapThresh != 0) {
    //capThresh = tempCapThresh;
  }
  if(tempDistThresh != 0) {
    //distThresh = tempDistThresh;
  }

  Serial.print("Set capacitive threshold to: ");
  Serial.println(capThresh);
  Serial.print("Set distance threshold to: ");
  Serial.println(distThresh);

  Serial.println((int)&capThresh, HEX);
  Serial.println((int)&distThresh, HEX);
  Serial.println("Configured sensors!");
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

float average(int interval[]) {
  int sum = 0;
  for(int i = 0; i < 10; i++) {
    sum += interval[i];
  }
  return (float)sum / 10.0f;
}

float standardDeviation(int interval[], int avg) {
  int sum = 0;
  for(int i = 0; i < 10; i++) {
    sum += absolute(avg - interval[i]);
  }
  return (float)sum / 10.0f;
}

float absolute(int val) {
  if(val < 0) return -val;
  else return val;
}

void error(const __FlashStringHelper*err) {
  Serial.println(err);
  while (1);
}
