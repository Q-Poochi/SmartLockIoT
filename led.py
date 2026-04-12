from machine import Pin
import time

# Khai báo LED
led1 = Pin(22, Pin.OUT)   # LED nối GPIO 22
led2 = Pin(4, Pin.OUT)  # LED nối GPIO 23
led3 = Pin(23, Pin.OUT) 

while True:
    # Bật LED1, tắt LED2
    led1.value(1)
    led2.value(0)
    led3.value(0)
    time.sleep(1)

    # Tắt LED1, bật LED2
    led1.value(0)
    led2.value(1)
    led3.value(0)
    time.sleep(1)

    # Tat led1, led2, bat led3
    led1.value(0)
    led2.value(0)
    led3.value(1)
    time.sleep(1)
    
    #Bat ca 3 led
    led1.value(1)
    led2.value(1)
    led3.value(1)
    time.sleep(1)

    # Tắt cả 3 LED
    led1.value(0)
    led2.value(0)
    led3.value(0)
    time.sleep(1)