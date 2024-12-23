import csv
import time
import paho.mqtt.client as mqtt

# Настройки подключения к MQTT
broker_address = "192.168.1.13"
port = 1883
topics = ["/devices/wb-msw-v3_21/controls/CO2",
          "/devices/wb-ms_11/controls/Illuminance",
          "/devices/wb-adc/controls/Vin"] 
# Создание файла CSV
csv_file = "sensor_data.csv"
with open(csv_file, mode='w', newline='') as file:
    writer = csv.writer(file)
    writer.writerow(["Timestamp", "CO2", "Illuminance", "Voltage"])
# Глобальные переменные для хранения данных
data = {"CO2": None, "Illuminance": None, "Voltage": None}

# Функция обработки сообщений
def on_message(client, userdata, msg):
    topic = msg.topic
    value = float(msg.payload.decode())

    if "/controls/CO2" in topic:
        data["CO2"] = value
    elif "/controls/Illuminance" in topic:
        data["Illuminance"] = value
    elif "/controls/Voltage" in topic:
        data["Voltage"] = value

# Подключение к MQTT и подписка на топики
client = mqtt.Client()
client.on_message = on_message
client.connect(broker_address, port, 60)
for topic in topics:
    client.subscribe(topic)

client.loop_start()

# Сбор данных в течение 10 минут
start_time = time.time()
while time.time() - start_time < 600:
    if None not in data.values():
        with open(csv_file, mode='a', newline='') as file:
            writer = csv.writer(file)
            writer.writerow([time.strftime("%Y-%m-%d%H:%M:%S"), data["CO2"], data["Illuminance"], data["Voltage"]])
        time.sleep(1)  # Сохраняем данные каждую секунду

client.loop_stop()
print("Сбор данных завершен. Данные сохранены в", csv_file)
