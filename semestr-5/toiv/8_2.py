import pandas as pd
import matplotlib.pyplot as plt

data = pd.read_csv("sensor_data.csv")
plt.hist(data["CO2"], bins=10, edgecolor='black')
plt.title("Гистограмма частоты показаний CO2")
plt.xlabel("CO2 (ppm)")
plt.ylabel("Частота")
plt.show()
