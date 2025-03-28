import math
import torch
import torch.nn as nn
import torch.optim as optim
from torch.utils.data import Dataset, DataLoader
import random

# Специальные токены и их индексы
PAD_TOKEN = "<pad>"
SOS_TOKEN = "<sos>"
EOS_TOKEN = "<eos>"
UNK_TOKEN = "<unk>"

PAD_IDX = 0
SOS_IDX = 1
EOS_IDX = 2
UNK_IDX = 3

# Функция для построения словаря из предложений
def build_vocab(sentences, specials=[PAD_TOKEN, SOS_TOKEN, EOS_TOKEN, UNK_TOKEN]):
    vocab = {token: idx for idx, token in enumerate(specials)}
    for sentence in sentences:
        for token in sentence.strip().split():
            if token not in vocab:
                vocab[token] = len(vocab)
    return vocab

# Инвертирование словаря (индекс -> токен)
def invert_vocab(vocab):
    return {idx: token for token, idx in vocab.items()}

# Токенизация с добавлением специальных токенов
def tokenize(sentence, vocab):
    tokens = sentence.strip().split()
    tokens = [SOS_TOKEN] + tokens + [EOS_TOKEN]
    return [vocab.get(token, UNK_IDX) for token in tokens]

# Датасет для машинного перевода: русские предложения -> английские переводы
class TranslationDataset(Dataset):
    def __init__(self, src_sentences, tgt_sentences, src_vocab, tgt_vocab):
        self.src_sentences = src_sentences
        self.tgt_sentences = tgt_sentences
        self.src_vocab = src_vocab
        self.tgt_vocab = tgt_vocab

    def __len__(self):
        return len(self.src_sentences)

    def __getitem__(self, idx):
        src_indices = tokenize(self.src_sentences[idx], self.src_vocab)
        tgt_indices = tokenize(self.tgt_sentences[idx], self.tgt_vocab)
        return torch.tensor(src_indices, dtype=torch.long), torch.tensor(tgt_indices, dtype=torch.long)

# Функция формирования батча с паддингом (batch_first=True)
def collate_fn(batch):
    src_batch, tgt_batch = zip(*batch)
    src_batch = nn.utils.rnn.pad_sequence(src_batch, batch_first=True, padding_value=PAD_IDX)
    tgt_batch = nn.utils.rnn.pad_sequence(tgt_batch, batch_first=True, padding_value=PAD_IDX)
    return src_batch, tgt_batch

# Позиционное кодирование (синусоидальное)
class PositionalEncoding(nn.Module):
    def __init__(self, d_model, dropout=0.1, max_len=5000):
        super(PositionalEncoding, self).__init__()
        self.dropout = nn.Dropout(p=dropout)

        pe = torch.zeros(max_len, d_model)  # (max_len, d_model)
        position = torch.arange(0, max_len, dtype=torch.float).unsqueeze(1)  # (max_len, 1)
        div_term = torch.exp(torch.arange(0, d_model, 2).float() * (-math.log(10000.0) / d_model))
        pe[:, 0::2] = torch.sin(position * div_term)  # четные индексы
        pe[:, 1::2] = torch.cos(position * div_term)  # нечетные
        pe = pe.unsqueeze(0)  # (1, max_len, d_model)
        self.register_buffer('pe', pe)

    def forward(self, x):
        # x: (batch_size, seq_len, d_model)
        x = x + self.pe[:, :x.size(1), :]
        return self.dropout(x)

# Масштабированное скалярное произведение (scaled dot-product attention)
def scaled_dot_product_attention(Q, K, V, mask=None):
    d_k = Q.size(-1)
    scores = torch.matmul(Q, K.transpose(-2, -1)) / math.sqrt(d_k)
    if mask is not None:
        scores = scores.masked_fill(mask == 0, -1e9)
    attn = torch.softmax(scores, dim=-1)
    output = torch.matmul(attn, V)
    return output, attn

# Многопоточное внимание (Multi-Head Attention)
class MultiHeadAttention(nn.Module):
    def __init__(self, d_model, num_heads, dropout=0.1):
        super(MultiHeadAttention, self).__init__()
        assert d_model % num_heads == 0, "d_model должно делиться на число голов"
        self.num_heads = num_heads
        self.d_k = d_model // num_heads
        self.d_model = d_model

        self.linear_q = nn.Linear(d_model, d_model)
        self.linear_k = nn.Linear(d_model, d_model)
        self.linear_v = nn.Linear(d_model, d_model)
        self.linear_out = nn.Linear(d_model, d_model)
        self.dropout = nn.Dropout(dropout)

    def forward(self, query, key, value, mask=None):
        # query, key, value: (batch_size, seq_len, d_model)
        batch_size = query.size(0)
        Q = self.linear_q(query)
        K = self.linear_k(key)
        V = self.linear_v(value)
        # Разбиваем на головы: (batch_size, num_heads, seq_len, d_k)
        Q = Q.view(batch_size, -1, self.num_heads, self.d_k).transpose(1, 2)
        K = K.view(batch_size, -1, self.num_heads, self.d_k).transpose(1, 2)
        V = V.view(batch_size, -1, self.num_heads, self.d_k).transpose(1, 2)

        if mask is not None:
            if mask.dim() == 2:
                mask = mask.unsqueeze(0)
            mask = mask.unsqueeze(1)

        attn_output, _ = scaled_dot_product_attention(Q, K, V, mask)
        # Конкатенация голов: (batch_size, seq_len, d_model)
        attn_output = attn_output.transpose(1, 2).contiguous().view(batch_size, -1, self.d_model)
        output = self.linear_out(attn_output)
        return output

# Полносвязная сеть (Feed Forward)
class PositionwiseFeedForward(nn.Module):
    def __init__(self, d_model, d_ff, dropout=0.1):
        super(PositionwiseFeedForward, self).__init__()
        self.linear1 = nn.Linear(d_model, d_ff)
        self.relu = nn.ReLU()
        self.dropout = nn.Dropout(dropout)
        self.linear2 = nn.Linear(d_ff, d_model)

    def forward(self, x):
        return self.linear2(self.dropout(self.relu(self.linear1(x))))

# Слой энкодера Transformer
class TransformerEncoderLayer(nn.Module):
    def __init__(self, d_model, num_heads, d_ff, dropout=0.1):
        super(TransformerEncoderLayer, self).__init__()
        self.self_attn = MultiHeadAttention(d_model, num_heads, dropout)
        self.ff = PositionwiseFeedForward(d_model, d_ff, dropout)
        self.norm1 = nn.LayerNorm(d_model)
        self.norm2 = nn.LayerNorm(d_model)
        self.dropout = nn.Dropout(dropout)

    def forward(self, x, mask=None):
        attn_output = self.self_attn(x, x, x, mask)
        x = self.norm1(x + self.dropout(attn_output))
        ff_output = self.ff(x)
        x = self.norm2(x + self.dropout(ff_output))
        return x

# Слой декодера Transformer
class TransformerDecoderLayer(nn.Module):
    def __init__(self, d_model, num_heads, d_ff, dropout=0.1):
        super(TransformerDecoderLayer, self).__init__()
        self.self_attn = MultiHeadAttention(d_model, num_heads, dropout)
        self.enc_dec_attn = MultiHeadAttention(d_model, num_heads, dropout)
        self.ff = PositionwiseFeedForward(d_model, d_ff, dropout)
        self.norm1 = nn.LayerNorm(d_model)
        self.norm2 = nn.LayerNorm(d_model)
        self.norm3 = nn.LayerNorm(d_model)
        self.dropout = nn.Dropout(dropout)

    def forward(self, x, encoder_output, tgt_mask=None, memory_mask=None):
        self_attn_output = self.self_attn(x, x, x, tgt_mask)
        x = self.norm1(x + self.dropout(self_attn_output))
        enc_dec_attn_output = self.enc_dec_attn(x, encoder_output, encoder_output, memory_mask)
        x = self.norm2(x + self.dropout(enc_dec_attn_output))
        ff_output = self.ff(x)
        x = self.norm3(x + self.dropout(ff_output))
        return x

# Стек слоёв энкодера
class TransformerEncoder(nn.Module):
    def __init__(self, num_layers, d_model, num_heads, d_ff, dropout=0.1):
        super(TransformerEncoder, self).__init__()
        self.layers = nn.ModuleList([
            TransformerEncoderLayer(d_model, num_heads, d_ff, dropout)
            for _ in range(num_layers)
        ])

    def forward(self, x, mask=None):
        for layer in self.layers:
            x = layer(x, mask)
        return x

# Стек слоёв декодера
class TransformerDecoder(nn.Module):
    def __init__(self, num_layers, d_model, num_heads, d_ff, dropout=0.1):
        super(TransformerDecoder, self).__init__()
        self.layers = nn.ModuleList([
            TransformerDecoderLayer(d_model, num_heads, d_ff, dropout)
            for _ in range(num_layers)
        ])

    def forward(self, x, encoder_output, tgt_mask=None, memory_mask=None):
        for layer in self.layers:
            x = layer(x, encoder_output, tgt_mask, memory_mask)
        return x

# Полная модель Transformer для машинного перевода
class TransformerModel(nn.Module):
    def __init__(self, src_vocab_size, tgt_vocab_size, d_model=128, num_layers=3,
                 num_heads=8, d_ff=512, dropout=0.1):
        super(TransformerModel, self).__init__()
        self.d_model = d_model
        self.src_embedding = nn.Embedding(src_vocab_size, d_model)
        self.tgt_embedding = nn.Embedding(tgt_vocab_size, d_model)
        self.positional_encoding = PositionalEncoding(d_model, dropout)
        self.encoder = TransformerEncoder(num_layers, d_model, num_heads, d_ff, dropout)
        self.decoder = TransformerDecoder(num_layers, d_model, num_heads, d_ff, dropout)
        self.fc_out = nn.Linear(d_model, tgt_vocab_size)

    def forward(self, src, tgt, src_mask=None, tgt_mask=None, memory_mask=None):
        # src, tgt: (batch_size, seq_len)
        src_emb = self.positional_encoding(self.src_embedding(src) * math.sqrt(self.d_model))
        tgt_emb = self.positional_encoding(self.tgt_embedding(tgt) * math.sqrt(self.d_model))
        encoder_output = self.encoder(src_emb, src_mask)
        decoder_output = self.decoder(tgt_emb, encoder_output, tgt_mask, memory_mask)
        output = self.fc_out(decoder_output)  # (batch_size, seq_len, tgt_vocab_size)
        return output

# Генерация маски для decoder (маска будущих позиций)
def generate_square_subsequent_mask(sz):
    mask = torch.triu(torch.ones(sz, sz), diagonal=1).bool()
    return ~mask  # True = разрешено

# Функция перевода (greedy decoding)
def translate(model, src_sentence, src_vocab, tgt_inv_vocab, device, max_len=50):
    model.eval()
    tokens = src_sentence.strip().split()
    tokens = [SOS_TOKEN] + tokens + [EOS_TOKEN]
    src_indexes = [src_vocab.get(token, UNK_IDX) for token in tokens]
    src_tensor = torch.tensor(src_indexes, dtype=torch.long).unsqueeze(0).to(device)
    
    src_emb = model.positional_encoding(model.src_embedding(src_tensor) * math.sqrt(model.d_model))
    encoder_output = model.encoder(src_emb)
    
    tgt_indexes = [SOS_IDX]
    for i in range(max_len):
        tgt_tensor = torch.tensor(tgt_indexes, dtype=torch.long).unsqueeze(0).to(device)
        tgt_mask = generate_square_subsequent_mask(tgt_tensor.size(1)).to(device)
        tgt_emb = model.positional_encoding(model.tgt_embedding(tgt_tensor) * math.sqrt(model.d_model))
        decoder_output = model.decoder(tgt_emb, encoder_output, tgt_mask)
        out = model.fc_out(decoder_output)
        next_token = out[0, -1, :].argmax().item()
        tgt_indexes.append(next_token)
        if next_token == EOS_IDX:
            break
    translated_tokens = [tgt_inv_vocab.get(idx, "<unk>") for idx in tgt_indexes]
    return " ".join(translated_tokens[1:-1])

# Главная функция: подготовка данных, обучение и тестовый перевод
def main():
    device = torch.device('cuda' if torch.cuda.is_available() else 'cpu')

    # Оригинальные 10 пар: русские предложения и их английские переводы
    base_src = [
        "привет мир",
        "как дела",
        "я в порядке",
        "приятно познакомиться",
        "доброе утро",
        "спасибо большое",
        "до свидания",
        "как тебя зовут",
        "хорошего дня",
        "извините"
    ]
    base_tgt = [
        "hello world",
        "how are you",
        "i am fine",
        "nice to meet you",
        "good morning",
        "thank you very much",
        "goodbye",
        "what is your name",
        "have a nice day",
        "sorry"
    ]
    
    # Для увеличения датасета до 100 пар, повторим базовые примеры 10 раз и перемешаем
    src_sentences = base_src * 10
    tgt_sentences = base_tgt * 10
    combined = list(zip(src_sentences, tgt_sentences))
    random.shuffle(combined)
    src_sentences, tgt_sentences = zip(*combined)
    src_sentences = list(src_sentences)
    tgt_sentences = list(tgt_sentences)
    
    # Построение словарей для русского (src) и английского (tgt)
    src_vocab = build_vocab(src_sentences)
    tgt_vocab = build_vocab(tgt_sentences)
    src_inv_vocab = invert_vocab(src_vocab)
    tgt_inv_vocab = invert_vocab(tgt_vocab)
    print("Размер словаря (русский):", len(src_vocab))
    print("Размер словаря (английский):", len(tgt_vocab))
    print("Количество примеров в датасете:", len(src_sentences))

    dataset = TranslationDataset(src_sentences, tgt_sentences, src_vocab, tgt_vocab)
    dataloader = DataLoader(dataset, batch_size=2, shuffle=True, collate_fn=collate_fn)

    # Параметры модели
    src_vocab_size = len(src_vocab)
    tgt_vocab_size = len(tgt_vocab)
    d_model = 128
    num_layers = 3
    num_heads = 8
    d_ff = 512
    dropout = 0.1

    model = TransformerModel(src_vocab_size, tgt_vocab_size, d_model, num_layers,
                             num_heads, d_ff, dropout).to(device)

    optimizer = optim.Adam(model.parameters(), lr=0.001)
    criterion = nn.CrossEntropyLoss(ignore_index=PAD_IDX)

    num_epochs = 20
    model.train()
    for epoch in range(1, num_epochs + 1):
        epoch_loss = 0
        for src_batch, tgt_batch in dataloader:
            src_batch = src_batch.to(device)
            tgt_batch = tgt_batch.to(device)
            optimizer.zero_grad()
            
            # Teacher forcing: вход декодера – все токены кроме последнего,
            # целевая последовательность – все токены начиная со второго.
            tgt_input = tgt_batch[:, :-1]
            tgt_out = tgt_batch[:, 1:]
            tgt_mask = generate_square_subsequent_mask(tgt_input.size(1)).to(device)

            output = model(src_batch, tgt_input, src_mask=None, tgt_mask=tgt_mask)
            loss = criterion(output.view(-1, tgt_vocab_size), tgt_out.contiguous().view(-1))
            loss.backward()
            optimizer.step()
            epoch_loss += loss.item()
        print(f"Epoch {epoch}/{num_epochs} Loss: {epoch_loss/len(dataloader):.4f}")

    # Режим перевода: пользователь вводит текст на русском, выводится перевод на английский
    model.eval()
    print("\nОбучение завершено. Введите предложение на русском для перевода на английский (или 'exit' для выхода).")
    while True:
        input_sentence = input("Введите предложение: ")
        if input_sentence.lower() == "exit":
            break
        translation = translate(model, input_sentence, src_vocab, tgt_inv_vocab, device)
        print("Перевод:", translation)

if __name__ == '__main__':
    main()
