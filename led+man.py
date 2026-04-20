import machine, st7789py as st7789, time

# SPI + màn hình
spi = machine.SPI(2, baudrate=2000000, polarity=0, phase=0,
                  sck=machine.Pin(14), mosi=machine.Pin(13))

dc  = machine.Pin(2,  machine.Pin.OUT)
rst = machine.Pin(4,  machine.Pin.OUT)
cs  = machine.Pin(5,  machine.Pin.OUT)

cs.value(0)
rst.value(1); time.sleep_ms(50)
rst.value(0); time.sleep_ms(200)
rst.value(1); time.sleep_ms(200)

display = st7789.ST7789(spi, 240, 240,
                        reset=rst, dc=dc, cs=cs,
                        xstart=0, ystart=0, rotation=0)

# 3 LED
led_do   = machine.Pin(26, machine.Pin.OUT)
led_vang = machine.Pin(27, machine.Pin.OUT)
led_xanh = machine.Pin(25, machine.Pin.OUT)

# Tắt hết khi khởi động
led_do.value(0)
led_vang.value(0)
led_xanh.value(0)
display.fill(st7789.BLACK)

print("Bắt đầu vòng lặp...")

while True:
    # Bật LED đỏ
    led_do.value(1)
    led_vang.value(0)
    led_xanh.value(0)
    display.fill(st7789.RED)
    print("ĐỎ")
    time.sleep(1)

    # Bật LED vàng
    led_do.value(0)
    led_vang.value(1)
    led_xanh.value(0)
    display.fill(st7789.color565(255, 200, 0))  # vàng
    print("VÀNG")
    time.sleep(1)

    # Bật LED xanh
    led_do.value(0)
    led_vang.value(0)
    led_xanh.value(1)
    display.fill(st7789.GREEN)
    print("XANH")
    time.sleep(1)