import json
import xml.etree.ElementTree as ET

# Функция для чтения данных из JSON файла
def read_from_json(file_path):
    try:
        with open(file_path, 'r') as json_file:
            data = json.load(json_file)
            print("Данные из JSON файла:")
            for key, value in data.items():
                print(f"{key}: {value}")
    except Exception as e:
        print(f"Ошибка при чтении JSON файла: {e}")

# Функция для чтения данных из XML файла
def read_from_xml(file_path):
    try:
        tree = ET.parse(file_path)
        root = tree.getroot()
        print("\nДанные из XML файла:")
        for child in root:
            print(f"{child.tag}: {child.text}")
    except Exception as e:
        print(f"Ошибка при чтении XML файла: {e}")

# Пути к файлам JSON и XML
json_file_path = "data.json"
xml_file_path = "data.xml"

# Вызов функций для чтения данных
read_from_json(json_file_path)
read_from_xml(xml_file_path)
