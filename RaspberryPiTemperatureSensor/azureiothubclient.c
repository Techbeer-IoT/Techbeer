#include <stdio.h>
#include <stdlib.h>
#include <string.h>

#include "iothub.h"
#include "iothub_device_client.h"
#include "iothub_client_options.h"
#include "iothub_message.h"
#include "azure_c_shared_utility/threadapi.h"
#include "azure_c_shared_utility/crt_abstractions.h"
#include "azure_c_shared_utility/platform.h"
#include "azure_c_shared_utility/shared_util_options.h"
#include "azure_c_shared_utility/tickcounter.h"


/* --- Transport Protocol --- */
//#define PROTOCOL_HTTP
#define PROTOCOL_MQTT

#ifdef PROTOCOL_HTTP
#include "iothubtransporthttp.h"
#endif // PROTOCOL_HTTP

#ifdef PROTOCOL_MQTT
#include "iothubtransportmqtt.h"
#endif // PROTOCOL_MQTT


static const char* CONNECTION_STRING = "";

static IOTHUB_DEVICE_CLIENT_HANDLE device_handle;


static void sendConfirmCallback(IOTHUB_CLIENT_CONFIRMATION_RESULT result, void* userContextCallback);


void sendMessageToAzureIoTHub(char messageBuff[]) {
	IOTHUB_MESSAGE_HANDLE message_handle;

	message_handle = IoTHubMessage_CreateFromString(messageBuff);

	// Create Azure IoT Hub message
	(void)IoTHubMessage_SetContentTypeSystemProperty(message_handle, "application%2fjson");
	(void)IoTHubMessage_SetContentEncodingSystemProperty(message_handle, "utf-8");
	
	// Send message async to Azure IoT Hub
	IoTHubDeviceClient_SendEventAsync(device_handle, message_handle, sendConfirmCallback, NULL);

	IoTHubMessage_Destroy(message_handle);
}


int initAzureConnection() {
	IOTHUB_CLIENT_TRANSPORT_PROVIDER protocol;

#ifdef PROTOCOL_HTTP
	protocol = HTTP_Protocol;
#endif // PROTOCOL_HTTP
#ifdef PROTOCOL_MQTT
	protocol = MQTT_Protocol;
#endif // PROTOCOL_MQTT 

	IoTHub_Init();

	device_handle = IoTHubDeviceClient_CreateFromConnectionString(CONNECTION_STRING, protocol);

	int isCanStart = 1;

	if (device_handle == NULL) {
		isCanStart = 0;
	}

	return isCanStart;
}

void endAzureConnection() {
	IoTHubDeviceClient_Destroy(device_handle);

	IoTHub_Deinit();
}

 // Azure IoT Hub callback
static void sendConfirmCallback(IOTHUB_CLIENT_CONFIRMATION_RESULT result, void* userContextCallback) {
	(void*)userContextCallback;
	printf("Data Send Completed: %s\n", MU_ENUM_TO_STRING(IOTHUB_CLIENT_CONFIRMATION_RESULT, result));
}
