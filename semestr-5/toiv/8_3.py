import pandas as pd
import matplotlib.pyplot as plt

data = pd.read_csv("sensor_data.csv")
plt.plot(data["Timestamp"], data["Illuminance"], marker='o')
plt.title("Освещенность по времени (3 минуты)")
plt.xlabel("Время")
plt.ylabel("Освещенность (лк)")
plt.xticks(data["Timestamp"][::10], rotation=45)  # Отображение каждой 10-й метки
plt.tight_layout()
plt.show()
