import time
import ubinascii
import machine
import ssl
from umqtt.robust import MQTTClient

SERVER    = "778192bafdb24249954652d9ae05565d.s1.eu.hivemq.cloud"
PORT      = 8883
MQTT_USER = "esp32"
MQTT_PASS = "Aabc12345"

CLIENT_ID = ubinascii.hexlify(machine.unique_id())
SUB_TOPIC = b"esp32/led"          # C# publish vào topic này

# LED gắn vào GPIO2 (LED onboard ESP32)
led = machine.Pin(2, machine.Pin.OUT)

def on_message(topic, msg):
    command = msg.decode().strip().upper()
    print(f"[RECV] {topic.decode()} :: {command}")

    if command == "ON":
        led.value(1)
        print("LED ON")
    elif command == "OFF":
        led.value(0)
        print("LED OFF")
    else:
        print(f"Unknown command: {command}")

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
    print(f"Connected! Listening on topic: {SUB_TOPIC.decode()}")
    return client

def main():
    client = connect_mqtt()

    while True:
        try:
            client.check_msg()   # non-blocking, nhận message liên tục
            time.sleep(0.1)      # nhỏ thôi để phản hồi nhanh

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