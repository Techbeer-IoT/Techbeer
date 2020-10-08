#ifndef AZUREIOTHUBCLIENT_H_
#define AZUREIOTHUBCLIENT_H_

int initAzureConnection();

void endAzureConnection();

void sendMessageToAzureIoTHub(char messageBuff[]);

#endif

