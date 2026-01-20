# 🎮 Bevy WASM Example

Минимальный пример игры на **Bevy Engine** с поддержкой WebAssembly.

> ⚠️ **ВАЖНО**: Bevy WASM сборки значительно больше по размеру чем Macroquad (~10-20 МБ vs ~500 КБ) и требуют больше времени на компиляцию. Рекомендуется использовать **Macroquad** для простых веб-игр.

---

## 📋 Требования

### 1. Установите Rust и wasm-pack

```bash
# Установка Rust (если ещё нет)
curl --proto '=https' --tlsv1.2 -sSf https://sh.rustup.rs | sh

# Установка wasm-pack (обязательно для Bevy)
cargo install wasm-pack
```

### 2. Добавьте WASM target

```bash
rustup target add wasm32-unknown-unknown
```

---

## 🔨 Сборка

### Вариант 1: wasm-pack (рекомендуется)

```bash
wasm-pack build --target web --release
```

Файлы появятся в `pkg/`:
- `bevy_wasm_example_bg.wasm` - основной WASM модуль
- `bevy_wasm_example.js` - JS обёртка

### Вариант 2: Cargo + wasm-bindgen

```bash
# Сборка
cargo build --release --target wasm32-unknown-unknown

# Генерация JS биндингов
wasm-bindgen --out-dir ./pkg --target web \
    target/wasm32-unknown-unknown/release/bevy_wasm_example.wasm
```

---

## 🌐 Запуск

### 1. Скопируйте файлы

```bash
# Переместите index.html в pkg/
cp index.html pkg/

# Или скопируйте pkg/ в корень
cp -r pkg/* .
```

### 2. Запустите локальный сервер

```bash
# Python
python3 -m http.server 8000

# Или basic-http-server
cargo install basic-http-server
basic-http-server pkg/
```

### 3. Откройте браузер

http://localhost:8000

---

## 📦 Структура

```
bevy/
├── Cargo.toml              # Конфигурация с Bevy
├── src/
│   └── main.rs             # Простая игра
├── index.html              # HTML обёртка
├── pkg/                    # Скомпилированные файлы (после сборки)
│   ├── bevy_wasm_example_bg.wasm
│   ├── bevy_wasm_example.js
│   └── index.html (скопируйте сюда)
└── README.md
```

---

## 🎮 Что внутри

- **Игрок**: Зелёный квадрат (управление WASD/стрелки)
- **Враги**: 5 красных квадратов, движутся вниз
- **Коллизии**: Простая проверка столкновений

---

## ⚙️ Оптимизация размера

### 1. Используйте минимальные features Bevy

В `Cargo.toml` уже настроено:
```toml
bevy = { version = "0.12", default-features = false, features = [
    "bevy_winit",
    "bevy_render",
    "bevy_core_pipeline",
    "bevy_sprite",
] }
```

### 2. wasm-opt

```bash
# Установка
cargo install wasm-opt

# Или через apt
sudo apt install binaryen

# Применение
wasm-opt -Oz pkg/bevy_wasm_example_bg.wasm -o pkg/bevy_wasm_example_bg.wasm
```

### 3. Сжатие gzip

Настройте сервер для gzip-сжатия `.wasm` файлов (уменьшение в 3-5 раз).

---

## 🐛 Типичные проблемы

### ❌ Ошибка: "wasm-pack not found"

```bash
cargo install wasm-pack
```

### ❌ Ошибка: "canvas not found"

Убедитесь что в `main.rs` указан правильный ID canvas:
```rust
canvas: Some("#bevy-canvas".to_string())
```

### ❌ Чёрный экран

1. Проверьте консоль браузера (F12)
2. Убедитесь что используете HTTP сервер (не file://)
3. Проверьте что все файлы из pkg/ доступны

### ❌ Долгая компиляция

Bevy большой фреймворк. Первая компиляция может занять 10-15 минут. Используйте `--release` только для финальной сборки.

---

## 📊 Сравнение с Macroquad

| Параметр | Bevy | Macroquad |
|----------|------|-----------|
| Размер WASM | ~10-20 МБ | ~200-500 КБ |
| Время компиляции | 10-15 мин | 1-2 мин |
| Возможности | ECS, полный движок | Простая графика |
| Сложность | Высокая | Низкая |

**Вывод**: Для простых веб-игр используйте **Macroquad**. Bevy больше подходит для сложных проектов где вам нужна ECS архитектура.

---

## 📚 Полезные ссылки

- [Bevy WASM Examples](https://github.com/bevyengine/bevy/tree/main/examples#wasm)
- [Bevy Cheatbook - Web](https://bevy-cheatbook.github.io/platforms/wasm.html)
- [wasm-pack Docs](https://rustwasm.github.io/docs/wasm-pack/)

---

## 💡 Рекомендации

1. **Разработка**: Компилируйте нативно (`cargo run`) - быстрее
2. **Тестирование WASM**: Используйте debug сборку
3. **Продакшен**: Release сборка + wasm-opt
4. **Альтернатива**: Рассмотрите Macroquad для веб-игр

---

**Удачи! 🚀**
