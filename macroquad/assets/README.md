# 🎨 Ассеты для игры

Эта папка предназначена для хранения игровых ресурсов (графика, звуки, музыка).

## 📁 Текущее состояние

В данный момент игра использует **процедурную графику** (рисование примитивов), поэтому внешние ассеты не требуются для базовой функциональности.

## 🎮 Визуальные элементы (созданы программно):

- **Игрок**: Зелёный треугольник
- **Враги**: Красные/Оранжевые квадраты
- **Пули**: Жёлтые круги
- **Бонусы**: Синие ромбы

## 🔊 Добавление звуков (опционально)

Если хотите добавить звуковые эффекты:

### 1. Поддерживаемые форматы:
- **OGG** (рекомендуется для веба)
- **WAV**
- **MP3**

### 2. Примеры звуков:
```
assets/
├── shoot.ogg       # Звук выстрела
├── explosion.ogg   # Уничтожение врага
├── bonus.ogg       # Подбор бонуса
├── gameover.ogg    # Проигрыш
└── victory.ogg     # Победа
```

### 3. Код для загрузки звука:

```rust
use macroquad::audio::{load_sound, play_sound, PlaySoundParams};

// В main() перед loop:
let shoot_sound = load_sound("assets/shoot.ogg").await.unwrap();

// При выстреле:
play_sound(
    &shoot_sound,
    PlaySoundParams {
        looped: false,
        volume: 0.5,
    },
);
```

## 🖼️ Добавление текстур (опционально)

### 1. Поддерживаемые форматы:
- **PNG** (с прозрачностью)
- **JPG**

### 2. Примеры текстур:
```
assets/
├── player.png      # Спрайт игрока (32x32)
├── enemy1.png      # Обычный враг (32x32)
├── enemy2.png      # Быстрый враг (32x32)
├── bullet.png      # Пуля (8x8)
├── bonus.png       # Бонус (24x24)
└── background.png  # Фон (опционально)
```

### 3. Код для загрузки текстур:

```rust
use macroquad::prelude::*;

// В main() перед loop:
let player_texture = load_texture("assets/player.png").await.unwrap();
player_texture.set_filter(FilterMode::Nearest); // Для пиксель-арта

// При отрисовке:
draw_texture(
    &player_texture,
    player.pos.x - 16.0,
    player.pos.y - 16.0,
    WHITE,
);
```

## 🎵 Фоновая музыка (опционально)

```rust
// Загрузка музыки
let music = load_sound("assets/music.ogg").await.unwrap();

// Зацикленное воспроизведение
play_sound(
    &music,
    PlaySoundParams {
        looped: true,
        volume: 0.3,
    },
);
```

## 🌐 Источники бесплатных ассетов

### Звуки:
- [freesound.org](https://freesound.org/)
- [OpenGameArt.org](https://opengameart.org/)
- [itch.io](https://itch.io/game-assets/free/tag-sound-effects)

### Графика:
- [kenney.nl](https://kenney.nl/assets) (отличные бесплатные спрайты)
- [OpenGameArt.org](https://opengameart.org/)
- [itch.io](https://itch.io/game-assets/free)

### Музыка:
- [incompetech.com](https://incompetech.com/)
- [freemusicarchive.org](https://freemusicarchive.org/)

## ⚙️ Рекомендации

1. **Размер файлов**: Держите ассеты маленькими (для быстрой загрузки WASM)
2. **Формат**: Предпочитайте OGG для звуков, PNG для графики
3. **Разрешение**: 32x32, 64x64 для спрайтов (пиксель-арт)
4. **Сжатие**: Используйте инструменты типа [TinyPNG](https://tinypng.com/)

## 📝 Примечание

Игра полностью работоспособна без внешних ассетов благодаря процедурной графике!
