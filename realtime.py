import time
import ubinascii
from umqtt.simple import MQTTClient
import machine

MQTT_BROKER = "192.168.1.57"
CLIENT_ID = ubinascii.hexlify(machine.unique_id())
TOPIC = b"tempepature"

last_ping = time.time()
ping_interval = 60

def sub_cb(topic, msg):
    now = time.localtime()
    timestamp = f"{now[0]}/{now[1]:02d}/{now[2]:02d} {now[3]:02d}:{now[4]:02d}:{now[5]:02d}"
    print(f"[{timestamp}] Received message on {topic.decode()}: {msg.decode()}")

def reset():
    print("Resetting...")
    time.sleep(5)
    machine.reset()
    
def main():
    mqttClient = MQTTClient(CLIENT_ID, MQTT_BROKER, keepalive=60)
    mqttClient.set_callback(sub_cb)
    mqttClient.connect()
    mqttClient.subscribe(TOPIC)
    print(f"Connected to MQTT Broker :: {MQTT_BROKER}, waiting for messages...")

    global last_ping
    while True:
        mqttClient.check_msg()
        if (time.time() - last_ping) >= ping_interval:
            mqttClient.ping()
            last_ping = time.time()
            now = time.localtime()
            print(f"[{now[0]}/{now[1]:02d}/{now[2]:02d} {now[3]:02d}:{now[4]:02d}:{now[5]:02d}] Pinged MQTT Broker")
        time.sleep(1)

if __name__ == "__main__":
    try:
        main()
    except OSError as e:
        print("Error:", e)
        reset()
