import torch
import torch.nn as nn
import torch.optim as optim
import torchvision
import torchvision.transforms as transforms
import matplotlib.pyplot as plt
import numpy as np

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

# 4.2.1. Реализация сверточной нейросети
class CNNClassifier(nn.Module):
    def __init__(self, num_classes=10):
        super(CNNClassifier, self).__init__()
        self.features = nn.Sequential(
            nn.Conv2d(3, 32, kernel_size=3, padding=1),  # [B,32,32,32]
            nn.ReLU(),
            nn.MaxPool2d(2),                             # [B,32,16,16]
            nn.Conv2d(32, 64, kernel_size=3, padding=1),   # [B,64,16,16]
            nn.ReLU(),
            nn.MaxPool2d(2),                             # [B,64,8,8]
            nn.Conv2d(64, 128, kernel_size=3, padding=1),  # [B,128,8,8]
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

# Функция обучения модели
def train_model(model, optimizer, criterion, train_loader, num_epochs):
    model.train()
    losses = []
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
        print(f"Epoch [{epoch+1}/{num_epochs}] Loss: {epoch_loss:.4f}")
    return losses

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

if __name__ == '__main__':
    # Инициализация моделей, оптимизаторов и функции потерь
    cnn_model = CNNClassifier(num_classes=10).to(device)
    vit_model = VisionTransformer(image_size=32, patch_size=4, in_channels=3, num_classes=10,
                                  embed_dim=128, num_layers=4, num_heads=4, dropout=0.1).to(device)

    criterion = nn.CrossEntropyLoss()
    optimizer_cnn = optim.Adam(cnn_model.parameters(), lr=learning_rate)
    optimizer_vit = optim.Adam(vit_model.parameters(), lr=learning_rate)

    print("Обучение сверточной нейросети...")
    cnn_losses = train_model(cnn_model, optimizer_cnn, criterion, train_loader, num_epochs)
    cnn_acc = evaluate_model(cnn_model, test_loader)
    print(f"Точность CNN: {cnn_acc:.2f}%")

    print("\nОбучение Vision Transformer...")
    vit_losses = train_model(vit_model, optimizer_vit, criterion, train_loader, num_epochs)
    vit_acc = evaluate_model(vit_model, test_loader)
    print(f"Точность ViT: {vit_acc:.2f}%")

    # Построение графиков обучения
    plt.figure(figsize=(10,5))
    plt.plot(range(1, num_epochs+1), cnn_losses, label='CNN Loss')
    plt.plot(range(1, num_epochs+1), vit_losses, label='ViT Loss')
    plt.xlabel("Эпоха")
    plt.ylabel("Loss")
    plt.title("График потерь при обучении")
    plt.legend()
    plt.grid(True)
    plt.savefig("loss_comparison.png")
    plt.show()
