import machine, st7789py as st7789
from fonts import vga2_8x8 as font_small
from fonts import vga1_16x32 as font_big
import random, time

# Khởi tạo màn hình
spi = machine.SPI(2, baudrate=2000000, polarity=0, phase=0,
                  sck=machine.Pin(14), mosi=machine.Pin(13))

dc  = machine.Pin(2,  machine.Pin.OUT)
rst = machine.Pin(4,  machine.Pin.OUT)
cs  = machine.Pin(5,  machine.Pin.OUT)

cs.value(0)
rst.value(1); time.sleep_ms(50)
rst.value(0); time.sleep_ms(200)
rst.value(1); time.sleep_ms(200)

disp_width  = 240
disp_height = 240
CENTER_X = disp_width  // 2
CENTER_Y = disp_height // 2

display = st7789.ST7789(spi, disp_width, disp_height,
                        reset=rst, dc=dc, cs=cs,
                        xstart=0, ystart=0, rotation=0)

# ── 1. Test màu nền ──────────────────────────────────────
print("Test màu nền...")
for color in [st7789.RED, st7789.GREEN, st7789.BLUE]:
    display.fill(color)
    time.sleep(1)

# ── 2. Chữ to + chữ nhỏ ─────────────────────────────────
print("Hiển thị chữ...")
display.fill(st7789.BLACK)
display.text(font_big,   "ESP32",   40,  20, st7789.RED)
display.text(font_big,   "MicroPy", 10,  60, st7789.YELLOW)
display.text(font_big,   "ST7789",  30, 100, st7789.GREEN)

for x in range(240):
    display.pixel(x, 140, st7789.WHITE)

display.text(font_small, "240 x 240 px",  20, 155, st7789.CYAN)
display.text(font_small, "SPI @ 2MHz",    20, 170, st7789.WHITE)
display.text(font_small, "GPIO14 SCK",    20, 185, st7789.GREEN)
display.text(font_small, "GPIO13 MOSI",   20, 200, st7789.GREEN)
display.text(font_small, "GPIO2  DC",     20, 215, st7789.YELLOW)
display.text(font_small, "GPIO5  CS",     20, 230, st7789.YELLOW)
time.sleep(2)

# ── 3. Pixel ngẫu nhiên ──────────────────────────────────
print("Pixel ngẫu nhiên...")
display.fill(st7789.BLACK)
for i in range(1000):
    display.pixel(random.randint(0, disp_width  - 1),
                  random.randint(0, disp_height - 1),
                  st7789.color565(random.getrandbits(8),
                                  random.getrandbits(8),
                                  random.getrandbits(8)))
time.sleep(2)

# ── 4. Vẽ vòng tròn ──────────────────────────────────────
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

print("Vẽ vòng tròn...")
display.fill(st7789.BLACK)
draw_circle(CENTER_X, CENTER_Y, 80,  st7789.BLUE)
draw_circle(CENTER_X, CENTER_Y, 60,  st7789.RED)
draw_circle(CENTER_X, CENTER_Y, 40,  st7789.GREEN)
draw_circle(CENTER_X, CENTER_Y, 20,  st7789.YELLOW)
time.sleep(2)

