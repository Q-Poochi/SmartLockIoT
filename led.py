from machine import Pin
import time

# Khai báo LED
led1 = Pin(4, Pin.OUT)   # LED nối GPIO 22
led2 = Pin(26, Pin.OUT)  # LED nối GPIO 23
led3 = Pin(27, Pin.OUT) 

while True:
    # Bật LED1, tắt LED2
    led1.value(1)
   
    time.sleep(1)

