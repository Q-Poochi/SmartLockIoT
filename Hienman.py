import machine, st7789py as st7789, time

spi = machine.SPI(2, baudrate=500000, polarity=0, phase=0,
                  sck=machine.Pin(14), mosi=machine.Pin(13))

dc  = machine.Pin(2,  machine.Pin.OUT)
rst = machine.Pin(4,  machine.Pin.OUT)
cs  = machine.Pin(5,  machine.Pin.OUT)  # ← GPIO5

cs.value(0)

rst.value(1); time.sleep_ms(50)
rst.value(0); time.sleep_ms(200)
rst.value(1); time.sleep_ms(200)

display = st7789.ST7789(spi, 240, 240,
                        reset=rst, dc=dc, cs=cs,
                        xstart=0, ystart=0, rotation=0)

display.fill(st7789.RED)
print("OK - RED")
time.sleep(2)
display.fill(st7789.GREEN)
print("OK - GREEN")