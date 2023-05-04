from pythonosc.udp_client import SimpleUDPClient
from cms50dplus import CMS50Dplus, LiveDataPoint
import time

ip = "127.0.0.1"
port = 6000

currBPM = 0

# PULSEPORT = "/dev/ttyUSB0"
PULSEPORT = "/dev/cu.usbserial-0001"

client = SimpleUDPClient(ip, port)  # Create client

# client.send_message("/currBPM", 123)   # Send float message

pulseOx = CMS50Dplus(PULSEPORT)
pulseOx.connect()
for liveData in pulseOx.getLiveData():
    if liveData.pulseRate !=  currBPM:
        print(" bpm is ", liveData.pulseRate)
        currBPM = liveData.pulseRate
        client.send_message("/currBPM", currBPM)
