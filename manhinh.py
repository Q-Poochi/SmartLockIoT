import uos
import machine
import st7789py as st7789
from fonts import vga2_8x8 as font1
from fonts import vga1_16x32 as font2
import random, time

# ESP32 wiring theo sơ đồ
spi = machine.SPI(2, baudrate=40000000, polarity=1,
                  sck=machine.Pin(18), mosi=machine.Pin(23))

dc  = machine.Pin(2, machine.Pin.OUT)
rst = machine.Pin(4, machine.Pin.OUT)


disp_width  = 240
disp_height = 240
CENTER_X = disp_width // 2
CENTER_Y = disp_height // 2

print(uos.uname())
print(spi)

display = st7789.ST7789(spi, disp_width, disp_height,
                        reset=rst, dc=dc,
                        xstart=80, ystart=0, rotation=0)

# test màu nền
for color in [st7789.RED, st7789.GREEN, st7789.BLUE]:
    display.fill(color)
    time.sleep(1)

# test chữ
display.fill(st7789.BLACK)
display.text(font2, "Hello ESP32!", 10, 10, st7789.RED)
display.text(font2, "MicroPython", 10, 40, st7789.YELLOW)
display.text(font1, "ST7789 SPI 240x240", 10, 80, st7789.GREEN)

# pixel ngẫu nhiên
for i in range(1000):
    display.pixel(random.randint(0, disp_width-1),
                  random.randint(0, disp_height-1),
                  st7789.color565(random.getrandbits(8),
                                  random.getrandbits(8),
                                  random.getrandbits(8)))

# vẽ vòng tròn
def draw_circle(xpos0, ypos0, rad, col=st7789.color565(255, 255, 255)):
    x = rad - 1
    y = 0
    dx = 1
    dy = 1
    err = dx - (rad << 1)
    while x >= y:
        display.pixel(xpos0 + x, ypos0 + y, col)
        display.pixel(xpos0 + y, ypos0 + x, col)
        display.pixel(xpos0 - y, ypos0 + x, col)
        display.pixel(xpos0 - x, ypos0 + y, col)
        display.pixel(xpos0 - x, ypos0 - y, col)
        display.pixel(xpos0 - y, ypos0 - x, col)
        display.pixel(xpos0 + y, ypos0 - x, col)
        display.pixel(xpos0 + x, ypos0 - y, col)
        if err <= 0:
            y += 1
            err += dy
            dy += 2
        if err > 0:
            x -= 1
            dx += 2
            err += dx - (rad << 1)

draw_circle(CENTER_X, CENTER_Y, 80, st7789.BLUE)

print("- bye-")
