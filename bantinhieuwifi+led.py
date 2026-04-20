import network, socket, time
import machine, st7789py as st7789

# ── Wifi ─────────────────────────────────────────────────
SSID     = "Redmi Note 14 Pro 5G" #mang 5g 
PASSWORD = "langxitrum"

wifi = network.WLAN(network.STA_IF)
wifi.active(True)
wifi.connect(SSID, PASSWORD)

print("Đang kết nối Wifi...")
while not wifi.isconnected():
    time.sleep(0.5)
    print(".", end="")

IP = wifi.ifconfig()[0]
print("\nKết nối thành công! IP:", IP)

# ── Màn hình ST7789 ───────────────────────────────────────
spi = machine.SPI(2, baudrate=2000000, polarity=0, phase=0,
                  sck=machine.Pin(14), mosi=machine.Pin(13))
dc  = machine.Pin(2, machine.Pin.OUT)
rst = machine.Pin(4, machine.Pin.OUT)
cs  = machine.Pin(5, machine.Pin.OUT)

cs.value(0)
rst.value(1); time.sleep_ms(50)
rst.value(0); time.sleep_ms(200)
rst.value(1); time.sleep_ms(200)

display = st7789.ST7789(spi, 240, 240,
                        reset=rst, dc=dc, cs=cs,
                        xstart=0, ystart=0, rotation=0)
display.fill(st7789.BLACK)

# ── 3 LED ─────────────────────────────────────────────────
led_do   = machine.Pin(25, machine.Pin.OUT)
led_vang = machine.Pin(26, machine.Pin.OUT)
led_xanh = machine.Pin(27, machine.Pin.OUT)

def tat_het():
    led_do.value(0)
    led_vang.value(0)
    led_xanh.value(0)

tat_het()

# ── HTML ──────────────────────────────────────────────────
COLORS = {
    "red":    (st7789.RED,                "ĐỎ",   "#e74c3c", led_do),
    "yellow": (st7789.color565(255,200,0),"VÀNG", "#f1c40f", led_vang),
    "green":  (st7789.GREEN,             "XANH", "#2ecc71", led_xanh),
    "off":    (st7789.BLACK,             "TẮT",  "#444444", None),
}

def webpage(active=""):
    buttons = ""
    for key, (_, label, hex_color, _led) in COLORS.items():
        border = "4px solid #fff" if key == active else "4px solid transparent"
        buttons += f"""
        <a href="/color?c={key}">
          <button style="background:{hex_color};border:{border};">
            {label}
          </button>
        </a>"""

    return f"""<!DOCTYPE html>
<html>
<head>
  <meta charset="UTF-8">
  <meta name="viewport" content="width=device-width,initial-scale=1">
  <title>ESP32 LED Control</title>
  <style>
    body {{
      background:#1a1a2e;
      display:flex;
      flex-direction:column;
      align-items:center;
      justify-content:center;
      min-height:100vh;
      margin:0;
      font-family:sans-serif;
      color:#fff;
    }}
    h1 {{ font-size:24px; margin-bottom:8px; }}
    p  {{ font-size:13px; color:#aaa; margin-bottom:36px; }}
    .grid {{
      display:grid;
      grid-template-columns:1fr 1fr;
      gap:20px;
    }}
    button {{
      width:130px;
      height:130px;
      border-radius:20px;
      font-size:20px;
      font-weight:bold;
      color:#fff;
      cursor:pointer;
      text-shadow:0 1px 3px rgba(0,0,0,0.4);
    }}
    a {{ text-decoration:none; }}
  </style>
</head>
<body>
  <h1>ESP32 LED + Display</h1>
  <p>Nhấn nút → LED sáng + màn hình đổi màu</p>
  <div class="grid">{buttons}</div>
</body>
</html>"""

# ── Web server ────────────────────────────────────────────
server = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
server.setsockopt(socket.SOL_SOCKET, socket.SO_REUSEADDR, 1)
server.bind(("", 80))
server.listen(5)
print(f"Mở trình duyệt: http://{IP}")

active_color = ""

while True:
    try:
        conn, addr = server.accept()
        request = conn.recv(1024).decode()

        color_key = ""
        if "/color?c=" in request:
            color_key = request.split("/color?c=")[1].split(" ")[0].strip()

        if color_key in COLORS:
            active_color = color_key
            screen_color = COLORS[color_key][0]
            led_pin      = COLORS[color_key][3]

            tat_het()
            if led_pin:
                led_pin.value(1)
            display.fill(screen_color)
            print(f"→ {color_key}")

        html = webpage(active_color)
        response = ("HTTP/1.1 200 OK\r\n"
                    "Content-Type: text/html\r\n"
                    f"Content-Length: {len(html)}\r\n"
                    "Connection: close\r\n\r\n" + html)
        conn.send(response.encode())
        conn.close()

    except Exception as e:
        print("Lỗi:", e)
        try:
            conn.close()
        except:
            pass