import time
import ubinascii
import machine
import ssl
import random
from umqtt.robust import MQTTClient

SERVER    = "778192bafdb24249954652d9ae05565d.s1.eu.hivemq.cloud"
PORT      = 8883
MQTT_USER = "test1"
MQTT_PASS = "Aabc12345"   # password mới

CLIENT_ID = ubinascii.hexlify(machine.unique_id())
PUB_TOPIC = b"esp32/temperature"
SUB_TOPIC = b"esp32/command"

def on_message(topic, msg):
    print(f"[RECV] Topic: {topic.decode()} | Msg: {msg.decode()}")

def reset():
    print("Resetting in 5s...")
    time.sleep(5)
    machine.reset()

def connect_mqtt():
    ssl_ctx = ssl.SSLContext(ssl.PROTOCOL_TLS_CLIENT)
    ssl_ctx.verify_mode = ssl.CERT_NONE

    client = MQTTClient(
        client_id = CLIENT_ID,
        server    = SERVER,
        port      = PORT,
        user      = MQTT_USER,
        password  = MQTT_PASS,
        keepalive = 60,
        ssl       = ssl_ctx
    )
    client.set_callback(on_message)
    client.connect()
    client.subscribe(SUB_TOPIC)
    print(f"Connected to HiveMQ :: {SERVER}")
    return client

def main():
    client = connect_mqtt()

    while True:
        try:
            client.check_msg()

            temp = random.randint(20, 50)
            client.publish(PUB_TOPIC, str(temp).encode())
            print(f"[SEND] Temperature: {temp}°C")

            time.sleep(3)

        except OSError as e:
            print(f"Connection lost: {e}")
            time.sleep(3)
            try:
                client = connect_mqtt()
            except:
                reset()

if __name__ == "__main__":
    try:
        main()
    except Exception as e:
        print(f"Fatal error: {e}")
        reset()