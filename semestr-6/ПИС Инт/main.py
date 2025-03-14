import random
import pandas as pd
from datetime import datetime, timedelta
from mlxtend.preprocessing import TransactionEncoder
from mlxtend.frequent_patterns import apriori, association_rules

# Список возможных товаров
items = ['Хлеб', 'Молоко', 'Яйца', 'Масло', 'Пиво', 'Сыр', 'Фрукты', 'Подгузники', 'Сок', 'Колбаса']

# Функция для генерации случайной даты в заданном интервале
def random_date(start, end):
    delta = end - start
    random_days = random.randrange(delta.days)
    return start + timedelta(days=random_days)

# Задаём период для транзакций (например, 2024 год)
start_date = datetime(2024, 1, 1)
end_date = datetime(2024, 12, 31)

# Генерация 20000 транзакций
data = []
for i in range(1, 20000):
    date = random_date(start_date, end_date)
    # Случайное количество товаров (от 1 до 5)
    num_items = random.randint(1, 5)
    # Случайный поднабор товаров
    transaction_items = random.sample(items, num_items)
    
    data.append({
        'TransactionID': i,
        'Date': date.strftime("%Y-%m-%d"),
        'Products': transaction_items
    })

# Создаём DataFrame с оригинальными данными
df_original = pd.DataFrame(data)
print("Первые 10 строк датасета:")
print(df_original.head(10))

# Извлекаем список транзакций для анализа
transactions = df_original['Products'].tolist()

# Преобразуем транзакции в формат one-hot (булеву матрицу)
te = TransactionEncoder()
te_array = te.fit(transactions).transform(transactions)
df_encoded = pd.DataFrame(te_array, columns=te.columns_)

# Для получения правил в условиях случайного распределения снижаем порог минимальной поддержки до 5%
frequent_itemsets = apriori(df_encoded, min_support=0.05, use_colnames=True)
print("\nЧастые наборы элементов:")
print(frequent_itemsets)

# Генерация ассоциативных правил с минимальной уверенностью 30%
rules = association_rules(frequent_itemsets, metric="confidence", min_threshold=0.3)
print("\nНайденные ассоциативные правила с метриками: Доверие  Лифт Рычаг   Убеждённость")
print(rules[['antecedents', 'consequents', 'support', 'confidence', 'lift', 'leverage', 'conviction']])
