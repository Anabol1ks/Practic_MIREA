import torch
import torch.nn.functional as F
from torch_geometric.datasets import Planetoid
from torch_geometric.nn import GCNConv

# Загружаем датасет CORA
dataset = Planetoid(root='C:/Users/Grigo/Documents/GitGrisha/Practic_MIREA/semestr-6/Neyronki/GNN/Cora', name='Cora')
data = dataset[0]  # В датасете CORA весь граф хранится в одном объекте

# Определяем модель GCN
class GCN(torch.nn.Module):
    def __init__(self, input_dim, hidden_dim, output_dim):
        super(GCN, self).__init__()
        # Первый графовый сверточный слой: преобразует входные признаки в скрытое представление
        self.conv1 = GCNConv(input_dim, hidden_dim)
        # Второй графовый сверточный слой: выводит вероятности классов
        self.conv2 = GCNConv(hidden_dim, output_dim)

    def forward(self, data):
        x, edge_index = data.x, data.edge_index
        # Применяем первый слой и ReLU-активацию
        x = self.conv1(x, edge_index)
        x = F.relu(x)
        # Применяем dropout для регуляризации
        x = F.dropout(x, training=self.training)
        # Применяем второй слой
        x = self.conv2(x, edge_index)
        # Вычисляем логарифм вероятностей по классам
        return F.log_softmax(x, dim=1)

# Инициализируем модель, оптимизатор и задаём параметры обучения
model = GCN(dataset.num_node_features, hidden_dim=16, output_dim=dataset.num_classes)
optimizer = torch.optim.Adam(model.parameters(), lr=0.01, weight_decay=5e-4)

# Функция обучения
def train():
    model.train()
    optimizer.zero_grad()
    out = model(data)
    # Вычисляем потерю (loss) по узлам, принадлежащим тренировочной выборке
    loss = F.nll_loss(out[data.train_mask], data.y[data.train_mask])
    loss.backward()
    optimizer.step()
    return loss.item()

# Функция тестирования: вычисляем точность на тренировочной, валидационной и тестовой выборках
def test():
    model.eval()
    logits = model(data)
    accs = []
    for mask in [data.train_mask, data.val_mask, data.test_mask]:
        pred = logits[mask].max(1)[1]  # предсказание: индекс максимального значения
        acc = pred.eq(data.y[mask]).sum().item() / mask.sum().item()
        accs.append(acc)
    return accs

# Обучаем модель в течение 300 эпох
for epoch in range(1, 301):
    loss = train()
    train_acc, val_acc, test_acc = test()
    if epoch % 10 == 0:
        print(f'Epoch: {epoch:03d}, Loss: {loss:.4f}, '
              f'Train Acc: {train_acc:.4f}, Val Acc: {val_acc:.4f}, Test Acc: {test_acc:.4f}')
