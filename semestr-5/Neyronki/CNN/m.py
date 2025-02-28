
import numpy as np
import pandas as pd
import matplotlib.pyplot as plt
from pathlib import Path
import opendatasets as od
import torch
import torch.nn as nn
import torch.nn.functional as F
from torch.utils.data import DataLoader
from torch.utils.data import random_split
from torchvision.utils import make_grid
from torchvision import datasets, transforms
from torchvision.datasets import ImageFolder
import pytorch_lightning as pl
from sklearn.model_selection import train_test_split
from sklearn.metrics import classification_report
from PIL import Image
import os


od.download("https://www.kaggle.com/datasets/puneet6060/intel-image-classification")

transform = transforms.Compose([
    transforms.RandomRotation(10),
    transforms.RandomHorizontalFlip(),
    transforms.Resize((224, 224)),
    transforms.CenterCrop(224),
    transforms.ToTensor(),
    transforms.Normalize(mean=[.485, .456, .406],
                         std=[.229, .224, .225])
])


class_names = sorted(os.listdir("./intel-image-classification/seg_train"))
print(class_names)

class DataModule(pl.LightningDataModule):
    def __init__(self, transform=transform, batch_size=32):
        super().__init__()
        self.root_dir = "./intel-image-classification/seg_train"  # Указываем путь к данным
        self.transform = transform
        self.batch_size = batch_size


    def setup(self, stage=None):
        dataset = ImageFolder(root=self.root_dir, transform=self.transform)
        n_data = len(dataset)
        n_train = int(0.6 * n_data)  # 60% train
        n_val = int(0.2 * n_data)    # 20% val
        n_test = n_data - n_train - n_val  # 20% test

        train_dataset, val_dataset, test_dataset = random_split(dataset, [n_train, n_val, n_test])

        self.train_dataset = DataLoader(train_dataset, batch_size=self.batch_size, shuffle=True)
        self.val_dataset = DataLoader(val_dataset, batch_size=self.batch_size)
        self.test_dataset = DataLoader(test_dataset, batch_size=self.batch_size)

    def train_dataloader(self):
        return self.train_dataset

    def val_dataloader(self):
        return self.val_dataset

    def test_dataloader(self):
        return self.test_dataset


class ConvolutionalNetwork(pl.LightningModule):

    def __init__(self):
        super().__init__()
        self.features = nn.Sequential(
            nn.Conv2d(in_channels=3, out_channels=6, kernel_size=3, stride=1),
            nn.ReLU(), #активация
            nn.MaxPool2d(kernel_size=2, stride=2), #уменьшает размер признака в 2 раза шаг 2
            nn.Conv2d(in_channels=6, out_channels=16, kernel_size=3, stride=1),
            nn.ReLU(),
            nn.MaxPool2d(kernel_size=2, stride=2),
        )
# in_channels=3: Входное изображение имеет 3 канала (RGB).
# out_channels=6: Слой извлекает 6 фильтров (карты признаков).
# kernel_size=3: Размер фильтра — 3x3.
# stride=1: Шаг фильтра — 1 пиксель

        self.classifier = nn.Sequential(
            nn.Linear(16 * 54 * 54, 120), #
            nn.ReLU(),
            nn.Linear(120, 84), #
            nn.ReLU(),
            nn.Linear(84, 20),
            nn.ReLU(),
            nn.Linear(20, len(class_names)),  # Количество классов = числу моделей
        )

    def forward(self, x):
        x = self.features(x)
        x = x.view(-1, 16 * 54 * 54)
        x = self.classifier(x)
        return F.log_softmax(x, dim=1)

    def configure_optimizers(self):
        optimizer = torch.optim.Adam(self.parameters(), lr=0.001)
        return optimizer

    def training_step(self, train_batch, batch_idx):
        X, y = train_batch
        y_hat = self(X)
        loss = F.cross_entropy(y_hat, y)
        acc = y_hat.argmax(dim=1).eq(y).float().mean()
        self.log("train_loss", loss)
        self.log("train_acc", acc)
        return loss

    def validation_step(self, val_batch, batch_idx):
        X, y = val_batch
        y_hat = self(X)
        loss = F.cross_entropy(y_hat, y)
        acc = y_hat.argmax(dim=1).eq(y).float().mean()
        self.log("val_loss", loss)
        self.log("val_acc", acc)

    def test_step(self, test_batch, batch_idx):
        X, y = test_batch
        y_hat = self(X)
        loss = F.cross_entropy(y_hat, y)
        acc = y_hat.argmax(dim=1).eq(y).float().mean()
        self.log("test_loss", loss)
        self.log("test_acc", acc)


if __name__ == "__main__":
    datamodule = DataModule()
    datamodule.setup()
    model = ConvolutionalNetwork()
    trainer = pl.Trainer(max_epochs=10)
    trainer.fit(model, datamodule)

    datamodule.setup(stage="test")
    test_loader = datamodule.test_dataloader()
    trainer.test(dataloaders=test_loader)

    for images, labels in datamodule.train_dataloader():
        break
    im = make_grid(images, nrow=16)

    plt.figure(figsize=(12, 12))
    plt.imshow(np.transpose(im.numpy(), (1, 2, 0)))

    inv_normalize = transforms.Normalize(mean=[-0.485 / 0.229, -0.456 / 0.224, -0.406 / 0.225],
                                         std=[1 / 0.229, 1 / 0.224, 1 / 0.225])
    im = inv_normalize(im)

    plt.figure(figsize=(12, 12))
    plt.imshow(np.transpose(im.numpy(), (1, 2, 0)))

    device = "cpu"
    model.eval()

    y_true = []
    y_pred = []
    with torch.no_grad():
        for test_data in datamodule.test_dataloader():
            test_images, test_labels = test_data[0].to(device), test_data[1].to(device)

            pred = model(test_images).argmax(dim=1)
            y_true.extend(test_labels.cpu().numpy())
            y_pred.extend(pred.cpu().numpy())

    print(classification_report(y_true, y_pred, target_names=class_names, digits=4))
