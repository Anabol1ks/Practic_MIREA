import json
import xml.etree.ElementTree as ET
import time
import paho.mqtt.client as mqtt


# Глобальные переменные для хранения данных датчиков
data = {
    "co2": None,
    "noise": None,
    "light": None,
    "temperature": None,
    "timestamp": None,
    "suitcase_number": "13"  # Замените на последние две цифры вашего IP
}
# Функция обратного вызова при получении сообщения
def on_message(client, userdata, msg):
    topic = msg.topic
    value = float(msg.payload.decode())

    if "/controls/CO2" in topic:
        data["co2"] = value
    elif "/controls/Sound Level" in topic:
        data["noise"] = value
    elif "/controls/Temperature" in topic:
        data["temperature"] = value
    elif "/controls/Illuminance" in topic:
        data["light"] = value
# Настройка MQTT-клиента
client = mqtt.Client()
client.on_message = on_message

# Подписка на топики
client.connect("192.168.1.13", 1883, 60)  # Замените на адрес вашего MQTT брокера
client.subscribe("/devices/wb-msw-v3_21/controls/CO2")
client.subscribe("/devices/wb-msw-v3_21/controls/Sound Level")
client.subscribe("/devices/wb-msw-v3_21/controls/Temperature")
client.subscribe("/devices/wb-ms_11/controls/Illuminance")

client.loop_start()

# Главный цикл для упаковки данных каждые 5 секунд
while True:
    data["timestamp"] = time.strftime("%Y-%m-%d %H:%M:%S")

    # Упаковка данных в JSON
    with open("data.json", "w") as json_file:
        json.dump(data, json_file)

    # Упаковка данных в XML
    root = ET.Element("Data")
    for key, value in data.items():
        element = ET.SubElement(root, key)
        element.text = str(value)
    tree = ET.ElementTree(root)
    tree.write("data.xml")

    print("Данные успешно сохранены в файлы JSON и XML.")
    time.sleep(5)
