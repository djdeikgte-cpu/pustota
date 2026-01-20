# 🚀 Space Shooter - Rust WASM Game (Macroquad)

Полноценная HTML5 игра на Rust, компилируемая в WebAssembly с использованием фреймворка **Macroquad**.

## 🎮 Описание игры

**Space Shooter** - простая космическая аркада где вы управляете кораблём и уничтожаете врагов:
- **Цель**: Уничтожить 30 врагов
- **Враги**: Красные (10 очков) и Оранжевые быстрые (20 очков)
- **Бонусы**: Синие ромбы дают 50 очков
- **Управление**: WASD/Стрелки для движения, Пробел/Клик для стрельбы

---

## 📋 Требования

### Установка Rust (если ещё не установлен)

```bash
# Linux/macOS:
curl --proto '=https' --tlsv1.2 -sSf https://sh.rustup.rs | sh

# Windows:
# Скачайте с https://rustup.rs/
```

После установки перезапустите терминал и проверьте:

```bash
cargo --version
rustc --version
```

---

## 🔨 Сборка проекта

### Быстрый способ (автоматический скрипт)

```bash
# Дайте права на выполнение
chmod +x build.sh

# Запустите сборку
./build.sh
```

### Ручная сборка (пошагово)

#### Шаг 1: Добавьте WebAssembly target

```bash
rustup target add wasm32-unknown-unknown
```

#### Шаг 2: Соберите проект

```bash
cargo build --release --target wasm32-unknown-unknown
```

#### Шаг 3: Скопируйте WASM файл

```bash
cp target/wasm32-unknown-unknown/release/macroquad_space_shooter.wasm .
```

---

## 🌐 Запуск в браузере

### Вариант 1: Python (рекомендуется)

```bash
# Python 3
python3 -m http.server 8000

# Или Python 2
python -m SimpleHTTPServer 8000
```

Откройте: http://localhost:8000

### Вариант 2: basic-http-server

```bash
# Установка
cargo install basic-http-server

# Запуск
basic-http-server .
```

Откройте: http://127.0.0.1:4000

### Вариант 3: Node.js (http-server)

```bash
# Установка
npm install -g http-server

# Запуск
http-server -p 8000
```

Откройте: http://localhost:8000

---

## 📦 Структура проекта

```
macroquad/
├── Cargo.toml                    # Конфигурация Rust проекта
├── src/
│   └── main.rs                   # Основной код игры
├── assets/                       # Папка для ассетов (пока не используется)
├── index.html                    # HTML обёртка для WASM
├── build.sh                      # Скрипт сборки
├── README.md                     # Эта инструкция
└── macroquad_space_shooter.wasm  # Скомпилированный WASM (после сборки)
```

---

## 🎯 Оптимизация размера

### Текущие оптимизации в Cargo.toml

```toml
[profile.release]
opt-level = "z"     # Максимальная оптимизация по размеру
lto = true          # Link Time Optimization
codegen-units = 1   # Меньше размер
panic = "abort"     # Убираем unwinding
strip = true        # Удаляем debug символы
```

### Дополнительная оптимизация с wasm-opt

```bash
# Установка (часть binaryen toolkit)
cargo install wasm-opt

# Или через apt (Ubuntu/Debian)
sudo apt install binaryen

# Применение оптимизации
wasm-opt -Oz macroquad_space_shooter.wasm -o macroquad_space_shooter.wasm
```

Ожидаемый размер: **~200-500 KB** (после оптимизации)

---

## 🐛 Решение типичных проблем

### ❌ Ошибка: "Cannot find module"

**Проблема**: WASM файл не найден
**Решение**: Убедитесь что `macroquad_space_shooter.wasm` находится в той же папке что и `index.html`

```bash
ls -lh macroquad_space_shooter.wasm
```

### ❌ Ошибка: "CORS policy"

**Проблема**: Браузер блокирует загрузку из-за политики CORS
**Решение**: Используйте локальный HTTP сервер (не открывайте `index.html` напрямую!)

```bash
python3 -m http.server 8000
```

### ❌ Чёрный экран в браузере

**Проблема**: WASM модуль не загрузился
**Решение**:
1. Откройте DevTools (F12) → Console
2. Проверьте ошибки загрузки
3. Убедитесь что используете HTTP сервер

### ❌ Ошибка: "target not found"

**Проблема**: WebAssembly target не установлен
**Решение**:

```bash
rustup target add wasm32-unknown-unknown
```

### ❌ Медленная сборка

**Решение**: Используйте `--release` флаг (уже в скрипте), или попробуйте:

```bash
cargo build --release --target wasm32-unknown-unknown -j 4
```

---

## 🔧 Разработка и отладка

### Локальная разработка

```bash
# Быстрая пересборка (без оптимизаций)
cargo build --target wasm32-unknown-unknown

# Release сборка (с оптимизациями)
cargo build --release --target wasm32-unknown-unknown
```

### Отладка в браузере

1. Откройте DevTools (F12)
2. Вкладка Console - для логов
3. Вкладка Network - для проверки загрузки WASM
4. Вкладка Performance - для профилирования

### Логирование из Rust

Добавьте в `Cargo.toml`:

```toml
[dependencies]
console_error_panic_hook = "0.1"
```

В `main.rs`:

```rust
#[cfg(target_arch = "wasm32")]
console_error_panic_hook::set_once();
```

---

## 📱 Поддержка мобильных устройств

HTML файл уже настроен для мобильных:
- Touch events работают (тап = выстрел)
- Viewport настроен
- Масштабирование отключено для игрового опыта

---

## 🚀 Публикация

### GitHub Pages

```bash
# 1. Соберите проект
./build.sh

# 2. Создайте ветку gh-pages
git checkout -b gh-pages

# 3. Закоммитьте файлы
git add index.html macroquad_space_shooter.wasm
git commit -m "Deploy game"

# 4. Запушьте
git push origin gh-pages
```

Игра будет доступна по адресу: `https://username.github.io/repo-name/`

### Netlify / Vercel

Просто загрузите папку с `index.html` и `macroquad_space_shooter.wasm`

### Яндекс.Игры / VK Play

Упакуйте в ZIP:

```bash
zip -r game.zip index.html macroquad_space_shooter.wasm
```

---

## 📚 Полезные ссылки

- [Macroquad Docs](https://macroquad.rs/)
- [Macroquad Examples](https://github.com/not-fl3/macroquad/tree/master/examples)
- [Rust WASM Book](https://rustwasm.github.io/docs/book/)
- [WebAssembly MDN](https://developer.mozilla.org/en-US/docs/WebAssembly)

---

## 🎓 Архитектура кода

### Основные компоненты

- **Player**: Структура игрока (позиция, жизни, счёт)
- **Bullet**: Пули с object pooling
- **Enemy**: Враги двух типов (обычный/быстрый)
- **Bonus**: Бонусные предметы
- **GameState**: Состояния игры (Playing/Victory/GameOver)

### Оптимизации

- **Object Pooling**: Переиспользование объектов вместо создания новых
- **Deactivation**: Неактивные объекты не рендерятся
- **Circle Collision**: Быстрая проверка коллизий

---

## 📝 Лицензия

MIT License - свободно используйте для своих проектов

---

## 🆘 Поддержка

Если возникли проблемы:
1. Проверьте версии: `cargo --version` (должно быть >= 1.70)
2. Проверьте логи в браузере (F12 → Console)
3. Убедитесь что используете HTTP сервер

**Удачи! 🎮🚀**
