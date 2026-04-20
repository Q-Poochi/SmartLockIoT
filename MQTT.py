import network
import time
from umqtt.simple import MQTTClient
from machine import Pin

# ===== WIFI =====
#SSID = "Redmi Note 14 Pro 5G"
#PASSWORD = "langxitrum"

SSID = "bios"
PASSWORD = "11111111"


# ===== MQTT CLOUD =====
MQTT_BROKER = "778192bafdb24249954652d9ae05565d.s1.eu.hivemq.cloud:8883"
PORT = 8883
USER = "esp32"
PASSWORD_MQTT = "Abc12345"

CLIENT_ID = "esp32_tls_demo"

TOPIC_LED = b"esp32/led"
TOPIC_TEMP = b"esp32/temp"

# ===== LED =====
led = Pin(2, Pin.OUT)

# ===== WIFI =====
wifi = network.WLAN(network.STA_IF)
wifi.active(False)   # reset
time.sleep(1)
wifi = network.WLAN(network.STA_IF)
wifi.active(True)
wifi.connect(SSID, PASSWORD)

print("Connecting WiFi...")
while not wifi.isconnected():
    time.sleep(1)

print("WiFi OK:", wifi.ifconfig())

# ===== CALLBACK =====
def sub_cb(topic, msg):
    print("Recv:", topic, msg)
    
    if topic == TOPIC_LED:
        if msg == b'1':
            led.on()
        else:
            led.off()

# ===== MQTT =====
client = MQTTClient(
    
    CLIENT_ID,
    MQTT_BROKER,
    port=8883,
    user=USER,
    password=PASSWORD_MQTT,
    ssl=True,
    ssl_params={"server_hostname": MQTT_BROKER}
)


client.set_callback(sub_cb)

print("Connecting MQTT...")
client.connect()

print("MQTT connected")

client.subscribe(TOPIC_LED)

# ===== LOOP =====
while True:
    client.check_msg()

    # fake nhiệt độ
    temp = str(25 + (time.time() % 5))
    client.publish(TOPIC_TEMP, temp)

    print("Temp:", temp)

    time.sleep(3)