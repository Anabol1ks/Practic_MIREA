import pandas as pd
import matplotlib.pyplot as plt

data = pd.read_csv("sensor_data.csv")
voltage_counts = data["Voltage"].value_counts()
plt.pie(voltage_counts, labels=voltage_counts.index, autopct='%1.1f%%')
plt.title("Распределение показаний напряжения")
plt.show()
