cmake_minimum_required(VERSION 2.8.11)

if (CMAKE_C_COMPILER_ID STREQUAL "GNU")
  set (CMAKE_C_FLAGS "--std=c99 -g ${CMAKE_C_FLAGS}")
endif()

include_directories("/usr/local/include/azureiot"
                    "/usr/local/include/azureiot/inc/")

set(SOURCE 
    techbeer.c
    azureiothubclient.c
    azureiothubclient.h
    parson.c 
    parson.h)

add_executable(TechBeerApp ${SOURCE})
target_link_libraries(TechBeerApp wiringPi
                          serializer
                          iothub_client
                          iothub_client_mqtt_transport
                          umqtt
                          aziotsharedutil
                          ssl
                          crypto
                          curl
                          pthread
                          m
                          ssl
                          crypto)
