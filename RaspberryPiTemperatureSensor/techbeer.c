#include <stdio.h>
#include <stdlib.h>
#include <string.h>
#include <time.h>
#include <sys/time.h>

#include <unistd.h>
#include <wiringPi.h>

#include <iothub.h>
#include <iothub_device_client.h>
#include <iothub_client_options.h>
#include <iothub_message.h>
#include <azure_c_shared_utility/threadapi.h>
#include <azure_c_shared_utility/crt_abstractions.h>
#include <azure_c_shared_utility/platform.h>
#include <azure_c_shared_utility/shared_util_options.h>
#include <azure_c_shared_utility/tickcounter.h>
#include <iothubtransportmqtt.h>

#include "azureiothubclient.h"

#define PROTOCOL_MQTT

#define GPIO17 17
#define GPIO27 27

//Peltierコントローラの状態遷移
#define  PELTIER_VOID               0       //無効
#define  PELTIER_OFF                1       //OFF状態
#define  PELTIER_COOL               2       //冷却中状態
#define  PELTIER_HEAT               3       //暖房中状態


//Peltierコントローラの差動開始温度
#define  PELTIER_STARTING_HEAT_TEMP      22       //暖房開始温度
#define  PELTIER_STARTING_COOL_TEMP      28       //冷却開始温度


//Peltierコントローラの差動停止温度
#define  PELTIER_STOPING_HEAT_TEMP      24       //暖房停止温度
#define  PELTIER_STOPING_COOL_TEMP      26       //冷却停止温度

//温度処理
void processTemperature(float ftemp);

//ペルチェ状態用変数
int gPeltier_state = PELTIER_VOID;

//ペルチェ状態セット
void set_state(int);

//ペルチェ状態ゲット
int get_state(void);


//ヒーターON
int onheating(void);

//クーラーON
int oncooling(void);

//クーラーヒーターOFF
int offpeltier(void);

//温度表示
float getTemperature( void );


//任意の番号のGPIOをONする
//int iGpio GPIOの番号
//int iMsec ONの時間
int ongpiotime(int iGpio, int iMsec);


//任意の番号のGPIOをOFFする
//int iGpio GPIOの番号
int offgpio(int iGpio);


//任意の番号のGPIOをONする
//int iGpio GPIOの番号
int ongpio(int iGpio);

// Temperature DataSet
struct TemperatureInfo {
	float temperature;
	float temperatureScale;
	int peltierStatus;
	int peltierOFF;
	int peltierHEAT;
	int peltierCOOL;
};

struct TemperatureInfo createTemperatureInfo(float temp);
char* getTemperatureMessageString(struct TemperatureInfo tempInfo);

// Azure
void doSendTemperatureData(float temperature);

int main(void){
  float fTemp = 0xffff;
  int i = 0;

  int isCanSendMessage = 0;

  set_state( PELTIER_VOID );

  printf(" START \n");

  set_state( PELTIER_OFF );

  while(1){

    sleep(3);

    fTemp = getTemperature();

    processTemperature(fTemp);

    printf("Temperature: %f \n", fTemp);
    printf("Peltier Status: %s\n", get_state() == 2 ? "HEAT" : get_state() == 3 ? "COOL" : "OFF");

    i++;
    if( i > 20 ){
        if (isCanSendMessage == 0) {
          printf("Trying to connect device to Azure IoT Hub\n");
          isCanSendMessage = initAzureConnection();
        }

        if (isCanSendMessage == 1) {
          doSendTemperatureData(fTemp);
        }
        else {
          printf("Cannot connect to Azure IoT Hub\n");
        }

        i=0;      
    }

    printf("\n\n");
  }

  endAzureConnection();

  return 1;
}


void processTemperature(float fTemp){
  switch (get_state()){
    case PELTIER_OFF:                                 //OFF状態
      if( fTemp > PELTIER_STARTING_COOL_TEMP ){       //庫内温度が冷却開始温度より高い
        oncooling();                                  //冷却開始  
        set_state( PELTIER_COOL );
      }else if( fTemp < PELTIER_STARTING_HEAT_TEMP ){ //庫内温度が暖房開始温度低い
        onheating();                                  //暖房開始
        set_state( PELTIER_HEAT );
      }
      break;
    case PELTIER_COOL:                               //冷却中状態
      if( fTemp < PELTIER_STOPING_COOL_TEMP ){       //冷却停止温度
        offpeltier();                                //ペルチェ停止
        set_state( PELTIER_OFF );
      }
      break;
    case PELTIER_HEAT:                               //暖房中状態
      if( fTemp > PELTIER_STOPING_HEAT_TEMP ){       //暖房停止温度
        offpeltier();                                //ペルチェ停止
        set_state( PELTIER_OFF );
      }
      break;
    default:
      break;
    }
}


//ペルチェ状態設定
//
//
void set_state(int state ){
   gPeltier_state = state;
}


//ペルチェ状態取得
//
//
int get_state(void){
  return(gPeltier_state);
}


//ヒーターON
//
//
int onheating(void){
  printf("Start Heating \n");
  offgpio( GPIO27 );
  ongpio( GPIO17 );
  return(0);
}


//クーラーON
//
//
int oncooling(void){
  printf("Start cooling \n");
  offgpio( GPIO17 );
  ongpio( GPIO27 );
  return(0);
}


//クーラーヒーターOFF
//
//
int offpeltier(void){
  printf("OFF \n");
  offgpio( GPIO17 );
  offgpio( GPIO27 );
  return(0);
}


//温度を表示する
//
//
float getTemperature( void ){
  int i=0;
  float fTemperature = 0xFFFF;
  char s[256];
  FILE* fp = NULL;


  fp = fopen("/PATH/TO/TEMPERATURE/SENSOR/DATA", "r");

  if( fp == NULL ){
    printf("File Open faile \n");
    return (fTemperature);
  }

//  printf("File Open Succes \n");

  for( i=0; i<256; i++){
    fread( &s[i] , 1 ,1 ,fp);
  }

//  printf("%s", s);
//  printf("File read Succes \n");

  fclose(fp);

  for( i=0;i<256 ;i++ ){
    if(( s[i]=='t' )&&( s[i+1]=='=' )){
//       printf("%c",s[i]);
//       printf("%c",s[i+1]);

       fTemperature = atoi(&s[i+2]);
       fTemperature = fTemperature/1000;
    }
  }

  return fTemperature;
}


int ongpio(int iGpio){
//  printf(">>ON_GPIO %d  \n", iGpio);

  if(wiringPiSetupGpio() == -1){
    printf("GPIO Set NG \n");    
    return 1;
  }

  pinMode(iGpio, OUTPUT);

  digitalWrite(iGpio, HIGH);

//  printf("<<ON_GPIO %d Done \n", iGpio);

}


int offgpio(int iGpio){
//  printf(">>OFF_GPIO %d  \n", iGpio);

  if(wiringPiSetupGpio() == -1){
    printf("GPIO Set NG \n");    
    return 1;
  }

  pinMode(iGpio, OUTPUT);

  digitalWrite(iGpio, LOW);

//  printf("<<OFF_GPIO %d Done \n", iGpio);

}


int ongpiotime(int iGpio, int iMsec){
  int i;

//  printf(">>ON_GPIO %d  \n", iGpio);

  if(wiringPiSetupGpio() == -1){
    printf("GPIO Set NG \n");    
    return 1;
  }

  pinMode(iGpio, OUTPUT);

  digitalWrite(iGpio, 0);
  delay(iMsec/10);
  digitalWrite(iGpio, 1);
  delay( (iMsec-(iMsec/10) ));
  digitalWrite(iGpio, 0);

//  printf("<<ON_GPIO %d Done \n", iGpio);

}


// Temperature DataSet Functions

char* getTemperatureMessageString(struct TemperatureInfo tempInfo) {
	char* tempStr = malloc(1000);

	if (tempStr != NULL) {
		sprintf(tempStr, "{ 'temperature':%.3f, 'temperatureScale':%.3f, 'peltierStatus': %d, 'peltierOFF': %d, 'peltierHEAT': %d, 'peltierCOOL': %d }",
			tempInfo.temperature, tempInfo.temperatureScale, 
			tempInfo.peltierStatus, tempInfo.peltierOFF, tempInfo.peltierHEAT, tempInfo.peltierCOOL);
	}

	return tempStr;
}

struct TemperatureInfo createTemperatureInfo(float temp) {
	struct TemperatureInfo tempInfo;

	tempInfo.temperature = temp;
	tempInfo.temperatureScale = temp / 50;
	tempInfo.peltierStatus = get_state();
	tempInfo.peltierOFF = tempInfo.peltierStatus == PELTIER_OFF ? 1 : 0;
	tempInfo.peltierHEAT = tempInfo.peltierStatus == PELTIER_HEAT ? 1 : 0;
	tempInfo.peltierCOOL = tempInfo.peltierStatus == PELTIER_COOL ? 1 : 0;

	return tempInfo;
}


// Azure DataSend
void doSendTemperatureData(float temperature){
    struct TemperatureInfo tempInfo = createTemperatureInfo(temperature);

    char* tempStr = getTemperatureMessageString(tempInfo);
    char tempMessage[256];

    strcpy_s(tempMessage, sizeof(tempMessage), tempStr);

    sendMessageToAzureIoTHub(tempMessage);
}
