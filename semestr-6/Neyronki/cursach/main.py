import os
import torch
import torch.nn as nn
import torch.optim as optim
import torchvision
import torchvision.transforms as transforms
import matplotlib.pyplot as plt
import numpy as np

# Создаём папку для логов, если её нет
logs_dir = "logs"
os.makedirs(logs_dir, exist_ok=True)

# Проверка наличия GPU
device = torch.device("cuda" if torch.cuda.is_available() else "cpu")

# Параметры обучения
num_epochs = 20
batch_size = 128
learning_rate = 1e-3

# Подготовка датасета CIFAR-10 с аугментацией
transform_train = transforms.Compose([
    transforms.RandomCrop(32, padding=4),
    transforms.RandomHorizontalFlip(),
    transforms.ToTensor(),
    transforms.Normalize((0.4914, 0.4822, 0.4465),
                         (0.2023, 0.1994, 0.2010)),
])
transform_test = transforms.Compose([
    transforms.ToTensor(),
    transforms.Normalize((0.4914, 0.4822, 0.4465),
                         (0.2023, 0.1994, 0.2010)),
])

train_dataset = torchvision.datasets.CIFAR10(
    root='C:/Users/Grigo/Documents/GitGrisha/Practic_MIREA/semestr-6/Neyronki/cursach',
    train=True, download=True, transform=transform_train)
test_dataset = torchvision.datasets.CIFAR10(
    root='C:/Users/Grigo/Documents/GitGrisha/Practic_MIREA/semestr-6/Neyronki/cursach',
    train=False, download=True, transform=transform_test)

train_loader = torch.utils.data.DataLoader(
    train_dataset, batch_size=batch_size, shuffle=True, num_workers=2)
test_loader = torch.utils.data.DataLoader(
    test_dataset, batch_size=batch_size, shuffle=False, num_workers=2)

# 4.2.1. Реализация сверточной нейросети (CNN)
class CNNClassifier(nn.Module):
    def __init__(self, num_classes=10):
        super(CNNClassifier, self).__init__()
        self.features = nn.Sequential(
            nn.Conv2d(3, 32, kernel_size=3, padding=1),  # [B,32,32,32]
            nn.ReLU(),
            nn.MaxPool2d(2),                             # [B,32,16,16]
            nn.Conv2d(32, 64, kernel_size=3, padding=1),  # [B,64,16,16]
            nn.ReLU(),
            nn.MaxPool2d(2),                             # [B,64,8,8]
            nn.Conv2d(64, 128, kernel_size=3, padding=1), # [B,128,8,8]
            nn.ReLU(),
            nn.MaxPool2d(2)                              # [B,128,4,4]
        )
        self.classifier = nn.Sequential(
            nn.Flatten(),
            nn.Linear(128 * 4 * 4, 256),
            nn.ReLU(),
            nn.Linear(256, num_classes)
        )
    def forward(self, x):
        x = self.features(x)
        x = self.classifier(x)
        return x

# 4.2.2. Реализация трансформерной модели (Vision Transformer)
class VisionTransformer(nn.Module):
    def __init__(self, image_size=32, patch_size=4, in_channels=3, num_classes=10,
                 embed_dim=128, num_layers=4, num_heads=4, dropout=0.1):
        super(VisionTransformer, self).__init__()
        assert image_size % patch_size == 0, "Размер изображения должен делиться на размер патча"
        self.num_patches = (image_size // patch_size) ** 2
        self.patch_dim = in_channels * patch_size * patch_size

        # Patch embedding
        self.patch_embed = nn.Linear(self.patch_dim, embed_dim)

        # Классификационный токен
        self.cls_token = nn.Parameter(torch.zeros(1, 1, embed_dim))
        # Позиционные эмбеддинги
        self.pos_embed = nn.Parameter(torch.zeros(1, self.num_patches + 1, embed_dim))
        self.dropout = nn.Dropout(dropout)

        # Transformer Encoder
        encoder_layer = nn.TransformerEncoderLayer(d_model=embed_dim, nhead=num_heads, dropout=dropout)
        self.transformer_encoder = nn.TransformerEncoder(encoder_layer, num_layers=num_layers)

        # Классификационная голова
        self.mlp_head = nn.Sequential(
            nn.LayerNorm(embed_dim),
            nn.Linear(embed_dim, num_classes)
        )
    
    def forward(self, x):
        B, C, H, W = x.shape
        patch_size = int(np.sqrt(self.patch_dim / C))
        # Разбивка изображения на патчи
        x = x.unfold(2, patch_size, patch_size).unfold(3, patch_size, patch_size)
        x = x.contiguous().view(B, C, -1, patch_size, patch_size)  # [B, C, num_patches, patch_size, patch_size]
        x = x.permute(0, 2, 1, 3, 4).contiguous().view(B, self.num_patches, -1)  # [B, num_patches, patch_dim]

        # Применяем линейное преобразование к каждому патчу
        x = self.patch_embed(x)  # [B, num_patches, embed_dim]

        # Добавляем классификационный токен
        cls_tokens = self.cls_token.expand(B, -1, -1)  # [B, 1, embed_dim]
        x = torch.cat((cls_tokens, x), dim=1)  # [B, num_patches+1, embed_dim]

        # Добавляем позиционные эмбеддинги и применяем dropout
        x = x + self.pos_embed
        x = self.dropout(x)

        # Трансформер ожидает вход с размерностью [seq_len, B, embed_dim]
        x = x.transpose(0, 1)
        x = self.transformer_encoder(x)
        x = x.transpose(0, 1)  # [B, num_patches+1, embed_dim]

        # Используем эмбеддинг классификационного токена для классификации
        x = x[:, 0]
        x = self.mlp_head(x)
        return x

# Функция обучения модели с сбором статистики (loss и accuracy за эпоху)
def train_model(model, optimizer, criterion, train_loader, test_loader, num_epochs):
    model.train()
    losses = []
    acc_list = []
    for epoch in range(num_epochs):
        running_loss = 0.0
        for images, labels in train_loader:
            images, labels = images.to(device), labels.to(device)
            optimizer.zero_grad()
            outputs = model(images)
            loss = criterion(outputs, labels)
            loss.backward()
            optimizer.step()
            running_loss += loss.item() * images.size(0)
        epoch_loss = running_loss / len(train_loader.dataset)
        losses.append(epoch_loss)
        # Вычисляем точность на тестовой выборке после каждой эпохи
        acc = evaluate_model(model, test_loader)
        acc_list.append(acc)
        print(f"Epoch [{epoch+1}/{num_epochs}] Loss: {epoch_loss:.4f} Test Acc: {acc:.2f}%")
    return losses, acc_list

# Функция тестирования модели
def evaluate_model(model, test_loader):
    model.eval()
    correct = 0
    total = 0
    with torch.no_grad():
        for images, labels in test_loader:
            images, labels = images.to(device), labels.to(device)
            outputs = model(images)
            _, predicted = torch.max(outputs.data, 1)
            total += labels.size(0)
            correct += (predicted == labels).sum().item()
    acc = 100 * correct / total
    return acc

# Функция для получения примеров предсказаний (правильных и ошибочных)
def get_prediction_examples(model, test_loader, num_examples=1):
    model.eval()
    correct_examples = []
    incorrect_examples = []
    with torch.no_grad():
        for images, labels in test_loader:
            images, labels = images.to(device), labels.to(device)
            outputs = model(images)
            _, predicted = torch.max(outputs.data, 1)
            for i in range(images.size(0)):
                if predicted[i] == labels[i] and len(correct_examples) < num_examples:
                    correct_examples.append((images[i].cpu(), labels[i].cpu(), predicted[i].cpu()))
                elif predicted[i] != labels[i] and len(incorrect_examples) < num_examples:
                    incorrect_examples.append((images[i].cpu(), labels[i].cpu(), predicted[i].cpu()))
                if len(correct_examples) >= num_examples and len(incorrect_examples) >= num_examples:
                    break
            if len(correct_examples) >= num_examples and len(incorrect_examples) >= num_examples:
                break
    return correct_examples, incorrect_examples

# Функция для денормализации изображений CIFAR-10
def unnormalize(img, mean=(0.4914, 0.4822, 0.4465), std=(0.2023, 0.1994, 0.2010)):
    for t, m, s in zip(img, mean, std):
        t.mul_(s).add_(m)
    return img

# Функция для сохранения примера предсказания с подписью
def save_prediction_example(example, filename, model_name):
    # example: (image, true_label, predicted_label)
    img, true_label, pred_label = example
    img = unnormalize(img)
    npimg = img.numpy().transpose(1, 2, 0)
    plt.figure()
    plt.imshow(np.clip(npimg, 0, 1))
    plt.title(f"{model_name} | True: {true_label.item()} Pred: {pred_label.item()}")
    plt.axis("off")
    plt.savefig(os.path.join(logs_dir, filename))
    plt.close()

if __name__ == '__main__':
    # Инициализация моделей, оптимизаторов и функции потерь
    cnn_model = CNNClassifier(num_classes=10).to(device)
    vit_model = VisionTransformer(image_size=32, patch_size=4, in_channels=3, num_classes=10,
                                  embed_dim=128, num_layers=4, num_heads=4, dropout=0.1).to(device)
    
    criterion = nn.CrossEntropyLoss()
    optimizer_cnn = optim.Adam(cnn_model.parameters(), lr=learning_rate)
    optimizer_vit = optim.Adam(vit_model.parameters(), lr=learning_rate)

    print("Обучение сверточной нейросети...")
    cnn_losses, cnn_acc_list = train_model(cnn_model, optimizer_cnn, criterion, train_loader, test_loader, num_epochs)
    final_cnn_acc = evaluate_model(cnn_model, test_loader)
    print(f"Точность CNN: {final_cnn_acc:.2f}%")

    print("\nОбучение Vision Transformer...")
    vit_losses, vit_acc_list = train_model(vit_model, optimizer_vit, criterion, train_loader, test_loader, num_epochs)
    final_vit_acc = evaluate_model(vit_model, test_loader)
    print(f"Точность ViT: {final_vit_acc:.2f}%")

    # Сохранение графиков обучения
    # Рисунок 4.1 – График потерь сверточной нейросети
    plt.figure()
    plt.plot(range(1, num_epochs+1), cnn_losses, marker='o')
    plt.xlabel("Эпоха")
    plt.ylabel("Loss")
    plt.title("Рисунок 4.1 – Потери CNN")
    plt.grid(True)
    plt.savefig(os.path.join(logs_dir, "CNN_loss.png"))
    plt.close()

    # Рисунок 4.2 – График точности сверточной нейросети
    plt.figure()
    plt.plot(range(1, num_epochs+1), cnn_acc_list, marker='o', color='green')
    plt.xlabel("Эпоха")
    plt.ylabel("Accuracy (%)")
    plt.title("Рисунок 4.2 – Точность CNN")
    plt.grid(True)
    plt.savefig(os.path.join(logs_dir, "CNN_accuracy.png"))
    plt.close()

    # Рисунок 4.3 – График потерь трансформерной модели
    plt.figure()
    plt.plot(range(1, num_epochs+1), vit_losses, marker='o', color='red')
    plt.xlabel("Эпоха")
    plt.ylabel("Loss")
    plt.title("Рисунок 4.3 – Потери ViT")
    plt.grid(True)
    plt.savefig(os.path.join(logs_dir, "ViT_loss.png"))
    plt.close()

    # Рисунок 4.4 – График точности трансформерной модели
    plt.figure()
    plt.plot(range(1, num_epochs+1), vit_acc_list, marker='o', color='purple')
    plt.xlabel("Эпоха")
    plt.ylabel("Accuracy (%)")
    plt.title("Рисунок 4.4 – Точность ViT")
    plt.grid(True)
    plt.savefig(os.path.join(logs_dir, "ViT_accuracy.png"))
    plt.close()

    # Получаем примеры предсказаний для CNN
    cnn_correct, cnn_incorrect = get_prediction_examples(cnn_model, test_loader, num_examples=1)
    if cnn_correct:
        save_prediction_example(cnn_correct[0], "CNN_correct.png", "CNN")
    if cnn_incorrect:
        save_prediction_example(cnn_incorrect[0], "CNN_incorrect.png", "CNN")
    
    # Получаем примеры предсказаний для ViT
    vit_correct, vit_incorrect = get_prediction_examples(vit_model, test_loader, num_examples=1)
    if vit_correct:
        save_prediction_example(vit_correct[0], "ViT_correct.png", "ViT")
    if vit_incorrect:
        save_prediction_example(vit_incorrect[0], "ViT_incorrect.png", "ViT")
