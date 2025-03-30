import os
import torch
import torch.nn as nn
import torch.optim as optim
import torchvision.datasets as datasets
import torchvision.transforms as transforms
import matplotlib
matplotlib.use('Agg')  # используем неинтерактивный backend
import matplotlib.pyplot as plt
import time

if torch.cuda.is_available():
    torch.backends.cudnn.benchmark = True

# Гиперпараметры
batch_size = 128
lr = 0.0002
epochs = 1000
noise_dim = 100  # размер случайного шума
save_every = 5  # сохранять изображения каждые N эпох

# Путь для сохранения изображений
save_dir = "./semestr-6/Neyronki/GAN/generated_images"
os.makedirs(save_dir, exist_ok=True)

# Трансформации и загрузка MNIST
transform = transforms.Compose([
    transforms.Resize(28),
    transforms.ToTensor(),
    transforms.Normalize((0.5,), (0.5,))
])

dataset = datasets.MNIST(root='./semestr-6/Neyronki/GAN/data', train=True, download=True, transform=transform)
dataloader = torch.utils.data.DataLoader(
    dataset, batch_size=batch_size, shuffle=True,
    num_workers=4, pin_memory=torch.cuda.is_available()
)

# Генератор
class Generator(nn.Module):
    def __init__(self, noise_dim):
        super(Generator, self).__init__()
        self.model = nn.Sequential(
            nn.Linear(noise_dim, 128),
            nn.ReLU(inplace=True),
            nn.Linear(128, 256),
            nn.ReLU(inplace=True),
            nn.Linear(256, 28 * 28),
            nn.Tanh()
        )

    def forward(self, z):
        img = self.model(z)
        return img.view(-1, 1, 28, 28)

# Дискриминатор (без Sigmoid, т.к. используем BCEWithLogitsLoss)
class Discriminator(nn.Module):
    def __init__(self):
        super(Discriminator, self).__init__()
        self.model = nn.Sequential(
            nn.Linear(28 * 28, 256),
            nn.LeakyReLU(0.2, inplace=True),
            nn.Linear(256, 128),
            nn.LeakyReLU(0.2, inplace=True),
            nn.Linear(128, 1)
        )

    def forward(self, img):
        x = img.view(-1, 28 * 28)
        return self.model(x)

# Функция для сохранения сгенерированных изображений в указанную папку
def show_generated_images(epoch, generator, device, fixed_noise):
    generator.eval()
    with torch.no_grad():
        gen_imgs = generator(fixed_noise.to(device)).cpu().numpy()
    generator.train()

    fig, axes = plt.subplots(4, 4, figsize=(6, 6))
    for i, ax in enumerate(axes.flat):
        img = gen_imgs[i].squeeze()
        img = (img + 1) / 2.0  # денормализация из [-1, 1] в [0, 1]
        ax.imshow(img, cmap='gray')
        ax.axis("off")
    plt.tight_layout()
    
    save_path = os.path.join(save_dir, f"generated_epoch_{epoch:03d}.png")
    plt.savefig(save_path)
    plt.close(fig)

def train():
    device = torch.device("cuda" if torch.cuda.is_available() else "cpu")
    generator = Generator(noise_dim).to(device)
    discriminator = Discriminator().to(device)

    optimizer_G = optim.Adam(generator.parameters(), lr=lr, betas=(0.5, 0.999))
    optimizer_D = optim.Adam(discriminator.parameters(), lr=lr, betas=(0.5, 0.999))
    
    criterion = nn.BCEWithLogitsLoss()

    # Если используется GPU, применяем AMP для ускорения (новый синтаксис)
    scaler_G = torch.amp.GradScaler(device_type='cuda') if device.type == 'cuda' else None
    scaler_D = torch.amp.GradScaler(device_type='cuda') if device.type == 'cuda' else None

    fixed_noise = torch.randn(16, noise_dim)

    for epoch in range(1, epochs + 1):
        for batch_idx, (imgs, _) in enumerate(dataloader):
            imgs = imgs.to(device)
            current_batch = imgs.size(0)
            valid = torch.ones(current_batch, 1, device=device)
            fake = torch.zeros(current_batch, 1, device=device)

            # Обучение генератора
            optimizer_G.zero_grad()
            z = torch.randn(current_batch, noise_dim, device=device)
            if scaler_G:
                with torch.amp.autocast(device_type='cuda'):
                    gen_imgs = generator(z)
                    g_loss = criterion(discriminator(gen_imgs), valid)
                scaler_G.scale(g_loss).backward()
                scaler_G.step(optimizer_G)
                scaler_G.update()
            else:
                gen_imgs = generator(z)
                g_loss = criterion(discriminator(gen_imgs), valid)
                g_loss.backward()
                optimizer_G.step()

            # Обучение дискриминатора
            optimizer_D.zero_grad()
            if scaler_D:
                with torch.amp.autocast(device_type='cuda'):
                    real_loss = criterion(discriminator(imgs), valid)
                    fake_loss = criterion(discriminator(gen_imgs.detach()), fake)
                    d_loss = (real_loss + fake_loss) / 2
                scaler_D.scale(d_loss).backward()
                scaler_D.step(optimizer_D)
                scaler_D.update()
            else:
                real_loss = criterion(discriminator(imgs), valid)
                fake_loss = criterion(discriminator(gen_imgs.detach()), fake)
                d_loss = (real_loss + fake_loss) / 2
                d_loss.backward()
                optimizer_D.step()

            if batch_idx % 100 == 0:
                print(f"Epoch [{epoch}/{epochs}] Batch [{batch_idx}/{len(dataloader)}] "
                      f"D_loss: {d_loss.item():.4f} G_loss: {g_loss.item():.4f}")

        # Сохраняем изображения только каждые save_every эпох
        if epoch % save_every == 0:
            show_generated_images(epoch, generator, device, fixed_noise)

if __name__ == '__main__':
    train()
