import network, time

wifi = network.WLAN(network.STA_IF)
wifi.active(False)
time.sleep(1)
wifi.active(True)
time.sleep(1)

print("MAC:", wifi.config('mac'))
print("Wifi sẵn sàng!")