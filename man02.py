import machine, st7789py as st7789, time
from fonts import vga2_8x8 as font_small
from fonts import vga1_16x32 as font_big

# Khởi tạo màn hình
spi = machine.SPI(2, baudrate=2000000, polarity=0, phase=0,
                  sck=machine.Pin(14), mosi=machine.Pin(13))

dc  = machine.Pin(2,  machine.Pin.OUT)
rst = machine.Pin(4,  machine.Pin.OUT)
cs  = machine.Pin(5,  machine.Pin.OUT)

cs.value(0)
rst.value(1); time.sleep_ms(50)
rst.value(0); time.sleep_ms(50)
rst.value(1); time.sleep_ms(50)

display = st7789.ST7789(spi, 240, 240,
                        reset=rst, dc=dc, cs=cs,
                        xstart=0, ystart=0, rotation=0)

# Màu nền đen
display.fill(st7789.BLACK)
time.sleep_ms(100)

# Chữ to ở trên
display.text(font_big, "ESP32", 40, 20, st7789.RED)
display.text(font_big, "MicroPy", 10, 60, st7789.YELLOW)
display.text(font_big, "ST7789", 30, 100, st7789.GREEN)

# Đường kẻ ngang
for x in range(240):
    display.pixel(x, 140, st7789.WHITE)

# Chữ nhỏ ở dưới
display.text(font_small, "240 x 240 px", 20, 155, st7789.CYAN)
display.text(font_small, "SPI @ 2MHz",   20, 170, st7789.WHITE)
display.text(font_small, "GPIO14 SCK",   20, 185, st7789.GREEN)
display.text(font_small, "GPIO13 MOSI",  20, 200, st7789.GREEN)
display.text(font_small, "GPIO2  DC",    20, 215, st7789.YELLOW)
display.text(font_small, "GPIO5  CS",    20, 230, st7789.YELLOW)

print("Hiển thị xong!")

# Chạy chữ cuộn từ phải sang trái
time.sleep(2)
display.fill(st7789.BLACK)

msg = "Trung lo "
x = 240

while True:
    display.fill(st7789.BLACK)
    display.text(font_big, msg[0:10], x, 100, st7789.CYAN)
    x -= 4
    if x < -160:
        x = 240
    time.sleep_ms(50)