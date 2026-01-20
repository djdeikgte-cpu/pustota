# 🦀 Rust WASM с wasm-bindgen и web-sys

Минимальный пример **ручной интеграции** Rust с Web APIs через `wasm-bindgen` и `web-sys`.

> 💡 Этот вариант показывает как работать с Canvas 2D API, DOM и событиями напрямую из Rust без игровых движков.

---

## 🎯 Что внутри

- Прямая работа с **Canvas 2D API** через `web-sys`
- Обработка **клавиатуры** (KeyboardEvent)
- Игровой цикл через **requestAnimationFrame**
- Минимальная игра: движение игрока + счётчик

---

## 📋 Требования

### 1. Установите wasm-pack

```bash
# Рекомендуемый способ (официальный инструмент)
cargo install wasm-pack

# Или через curl (Linux/macOS)
curl https://rustwasm.github.io/wasm-pack/installer/init.sh -sSf | sh
```

### 2. Rust toolchain

```bash
# Если ещё нет Rust
curl --proto '=https' --tlsv1.2 -sSf https://sh.rustup.rs | sh

# Добавьте WASM target
rustup target add wasm32-unknown-unknown
```

---

## 🔨 Сборка

### Вариант 1: wasm-pack (рекомендуется)

```bash
wasm-pack build --target web --release
```

Результат в папке `pkg/`:
```
pkg/
├── wasm_bindgen_canvas_game_bg.wasm    # WASM модуль
├── wasm_bindgen_canvas_game.js         # JS обёртка
├── wasm_bindgen_canvas_game.d.ts       # TypeScript типы
└── package.json                         # NPM метаданные
```

### Вариант 2: Cargo + wasm-bindgen CLI

```bash
# Установка wasm-bindgen-cli
cargo install wasm-bindgen-cli

# Сборка
cargo build --release --target wasm32-unknown-unknown

# Генерация биндингов
wasm-bindgen --out-dir ./pkg --target web \
    target/wasm32-unknown-unknown/release/wasm_bindgen_canvas_game.wasm
```

---

## 🌐 Запуск

### 1. Скопируйте index.html в pkg/

```bash
cp index.html pkg/
```

### 2. Запустите HTTP сервер

```bash
# Python
python3 -m http.server 8000
# Откройте: http://localhost:8000/pkg/

# Или basic-http-server
cargo install basic-http-server
basic-http-server pkg/
# Откройте: http://127.0.0.1:4000
```

---

## 📦 Структура проекта

```
wasm_bindgen/
├── Cargo.toml              # Конфигурация (cdylib)
├── src/
│   └── lib.rs              # Rust код с Canvas API
├── index.html              # HTML + JS загрузчик
├── pkg/                    # Скомпилированные файлы (после сборки)
│   ├── *.wasm
│   ├── *.js
│   └── index.html (скопируйте)
└── README.md
```

---

## 🎮 Архитектура

### Rust сторона (`lib.rs`)

```rust
#[wasm_bindgen]
pub struct Game {
    canvas: HtmlCanvasElement,
    context: CanvasRenderingContext2d,
    // ... игровое состояние
}

#[wasm_bindgen]
impl Game {
    #[wasm_bindgen(constructor)]
    pub fn new(canvas_id: &str) -> Result<Game, JsValue> {
        // Создание игры
    }

    pub fn update(&mut self) { /* Логика */ }
    pub fn render(&self) { /* Отрисовка */ }
}
```

### JavaScript сторона (`index.html`)

```javascript
import init, { Game } from './pkg/wasm_bindgen_canvas_game.js';

// Загрузка WASM
await init();

// Создание игры
const game = new Game('game-canvas');

// Игровой цикл
function gameLoop() {
    game.update();
    game.render();
    requestAnimationFrame(gameLoop);
}
```

---

## 🔧 Оптимизация

### 1. Cargo.toml уже оптимизирован

```toml
[profile.release]
opt-level = "z"     # Размер важнее скорости
lto = true          # Link Time Optimization
codegen-units = 1   # Уменьшение размера
panic = "abort"     # Меньше кода для паники
strip = true        # Удаляем символы
```

### 2. wasm-opt (дополнительная оптимизация)

```bash
# Установка
cargo install wasm-opt
# или
sudo apt install binaryen

# Применение
wasm-opt -Oz pkg/wasm_bindgen_canvas_game_bg.wasm \
    -o pkg/wasm_bindgen_canvas_game_bg.wasm
```

### 3. Gzip compression на сервере

Настройте веб-сервер для сжатия `.wasm` файлов (уменьшение в 3-5 раз).

**Ожидаемый размер**: ~20-50 КБ (после оптимизации)

---

## 🐛 Решение проблем

### ❌ "wasm-pack not found"

```bash
cargo install wasm-pack
```

### ❌ "Cannot find module 'pkg/...'"

Убедитесь что:
1. Запустили `wasm-pack build --target web`
2. Скопировали `index.html` в `pkg/`
3. Открываете через HTTP сервер (не file://)

### ❌ CORS ошибки

Используйте локальный HTTP сервер, не открывайте файл напрямую.

### ❌ "Imports from `web_sys` not found"

Проверьте `Cargo.toml` - нужные features добавлены в `web-sys`:
```toml
web-sys = { version = "0.3", features = [
    "HtmlCanvasElement",
    "CanvasRenderingContext2d",
    # ...
] }
```

---

## 📚 Расширение проекта

### Добавление новых Web APIs

1. Найдите нужный API в [web-sys docs](https://rustwasm.github.io/wasm-bindgen/api/web_sys/)
2. Добавьте feature в `Cargo.toml`:

```toml
web-sys = { version = "0.3", features = [
    "HtmlCanvasElement",
    "AudioContext",      # Для звука
    "WebGlRenderingContext", # Для WebGL
    # ...
] }
```

3. Используйте в Rust:

```rust
use web_sys::AudioContext;

let audio = AudioContext::new()?;
```

### Примеры расширений

- **Звук**: `AudioContext`, `AudioBuffer`
- **WebGL**: `WebGlRenderingContext`
- **Локальное хранилище**: `Storage`, `Window::local_storage`
- **Сеть**: `XmlHttpRequest`, `fetch` через `web_sys`

---

## 🎓 Полезные материалы

### Документация
- [wasm-bindgen Book](https://rustwasm.github.io/wasm-bindgen/)
- [web-sys API Docs](https://rustwasm.github.io/wasm-bindgen/api/web_sys/)
- [Rust WASM Book](https://rustwasm.github.io/book/)

### Примеры
- [Official wasm-bindgen examples](https://github.com/rustwasm/wasm-bindgen/tree/main/examples)
- [Rust WASM examples](https://github.com/rustwasm/rust-webpack-template)

### Инструменты
- [wasm-pack](https://rustwasm.github.io/wasm-pack/) - сборщик WASM проектов
- [wasm-bindgen](https://github.com/rustwasm/wasm-bindgen) - биндинги для Web APIs

---

## 💡 Когда использовать этот подход

### ✅ Подходит для:
- Максимальный контроль над Web APIs
- Интеграция Rust в существующий JS проект
- Обучение внутренностям WASM
- Небольшие утилиты и библиотеки

### ❌ Не подходит для:
- Сложных игр (используйте Macroquad/Bevy)
- Быстрой разработки (больше boilerplate кода)
- Проектов требующих много готовых компонентов

---

## 📊 Сравнение подходов

| Параметр | wasm-bindgen | Macroquad | Bevy |
|----------|--------------|-----------|------|
| Размер | ~20-50 КБ | ~200-500 КБ | ~10-20 МБ |
| Контроль | Максимальный | Средний | Высокий |
| Сложность | Высокая | Низкая | Средняя |
| Для игр | ❌ | ✅ | ✅ |
| Для утилит | ✅ | ❌ | ❌ |

---

**Удачи в изучении Rust WASM! 🦀🚀**
