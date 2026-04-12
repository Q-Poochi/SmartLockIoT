from machine import Pin
import time

# Khai báo 3 LED - pin tránh conflict với màn hình (4,23)
led1 = Pin(5, Pin.OUT)    # LED1 GPIO5
led2 = Pin(16, Pin.OUT)   # LED2 GPIO16
led3 = Pin(17, Pin.OUT)   # LED3 GPIO17

print("OLED LED Control - 3 LEDs sequencing")

while True:
    # 1. Chỉ LED1 ON
    led1.value(1)
    led2.value(0)
    led3.value(0)
    print("LED1 ON")
    time.sleep(1)

    # 2. Chỉ LED2 ON
    led1.value(0)
    led2.value(1)
    led3.value(0)
    print("LED2 ON")
    time.sleep(1)

    # 3. Chỉ LED3 ON
    led1.value(0)
    led2.value(0)
    led3.value(1)
    print("LED3 ON")
    time.sleep(1)
    
    # 4. Tất cả ON
    led1.value(1)
    led2.value(1)
    led3.value(1)
    print("All LEDs ON")
    time.sleep(1)

    # 5. Tất cả OFF
    led1.value(0)
    led2.value(0)
    led3.value(0)
    print("All LEDs OFF")
    time.sleep(1)

