from azure.storage.blob import BlockBlobService
from azure.iot.device import IoTHubDeviceClient
import subprocess
import datetime
import time
import threading
import RPi.GPIO as GPIO
import traceback
from constant import *
from PIL import Image
#import urllib.request
from logging import getLogger, FileHandler, StreamHandler, Formatter, DEBUG

logger = getLogger(__name__)

# Camera shooting interval
SHOOTING_INTERVAL=60
# Image resolution
IMAGE_RESOLUTION=(800,600)
# Thumbnail resolution
# THUMBNAIL_RESOLUTION=(200,150)
# Flash GPIO pin number
FLASH_GPIO_PIN = 2


def capture():
    logger.debug("Capture")
    #led on
    GPIO.output(2, False)
    
    # capture
    res = subprocess.call(['raspistill','-o','image.jpg', '-t', '1', "-w", str(IMAGE_RESOLUTION[0]), "-h", str(IMAGE_RESOLUTION[1])])
    
    if res != 0:
        #led off
        logger.warn("fail")
        GPIO.output(FLASH_GPIO_PIN, True)
        raise Exception

    logger.debug("Captured")

    # Get Storage Account
    service = BlockBlobService(account_name=STORAGE_ACCOUNT_NAME,account_key=STORAGE_ACCOUNT_KEY)

    #compress
    # img = Image.open('image.jpg', 'r')
    # img.thumbnail(THUMBNAIL_RESOLUTION)
    # img.save('preview.jpg', 'JPEG')

    # Blob File Name
    file_name = datetime. datetime.now().strftime("%Y%m%d_%H%M%S.jpg")

    # Upload Blob
    # service.create_blob_from_path(STORAGE_CONTAINER_NAME_THUMBNAIL, file_name,'preview.jpg')
    service.create_blob_from_path(STORAGE_CONTAINER_NAME, file_name,'image.jpg')
    logger.debug("Image sent")
    
    #trigger
    # params = {
    #     'token': file_name
    # }
    # req = urllib.request.Request('{}&{}'.format(TRIGGER_URL, urllib.parse.urlencode(params)))
    # with urllib.request.urlopen(req) as res:
    #     body = res.read()

    # logger.debug("Trigger riquested")
    
    #led off
    GPIO.output(2, True)

def message_listener():
    while True:
        client = IoTHubDeviceClient.create_from_connection_string(IOT_DEVICE_CONNECTION_STRING)
        while True:
            logger.debug("Wait message")
            try:
                message = client.receive_message()
                logger.debug("Message received")
                logger.debug( "    Data: <<{}>>".format(message.data) )
                logger.debug( "    Properties: {}".format(message.custom_properties))
                capture()
            except:
                logger.error('catch',exc_info=True)
                time.sleep(5)
                break

def iothub_client_sample_run():
    while True:
        try:
            message_listener_thread = threading.Thread(target=message_listener)
            message_listener_thread.daemon = True
            message_listener_thread.start()

            while True:
                # acquire datetime
                time_str = "{0:%Y%m%d_%H%M%S.%f}".format(datetime.datetime.now())
                # capture(time_str + '.jpg')
                time.sleep(SHOOTING_INTERVAL)

        except KeyboardInterrupt:
            logger.debug ( "stopped" )
            raise
        except Exception as e:
            logger.error('catch',exc_info=True)

if __name__ == '__main__':
    GPIO.setmode(GPIO.BCM)
    GPIO.setup(FLASH_GPIO_PIN, GPIO.OUT)

    sh = StreamHandler()
    fh = FileHandler('/var/log/beercamera.log')
    sh.setFormatter(Formatter("%(message)s"))
    sh.setLevel(DEBUG)
    fh.setFormatter(Formatter("%(asctime)s:%(lineno)d:%(levelname)s:%(message)s"))
    fh.setLevel(DEBUG)
    logger.addHandler(sh)
    logger.addHandler(fh)
    logger.setLevel(DEBUG)
    
    #led off
    GPIO.output(2, True)
    logger.debug ( "start" )
    try:
        iothub_client_sample_run()
    except:
        logger.error('catch',exc_info=True)
    GPIO.cleanup()
