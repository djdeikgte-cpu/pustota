# 🎮 Rust WASM Game - Полный набор примеров

Полноценный проект игр на Rust с компиляцией в WebAssembly. Три готовых варианта с разными подходами!

---

## 📦 Что внутри

Проект содержит **три варианта** реализации игры на Rust для браузера:

### 1️⃣ **Macroquad** (рекомендуется) 🌟
- 🚀 Космический шутер с полным геймплеем
- ✅ Простая сборка и минимальный размер (~200-500 КБ)
- 🎮 Полноценная игра: враги, стрельба, бонусы, HUD
- 📁 Папка: `macroquad/`

### 2️⃣ **Bevy** (для сложных проектов)
- 🎯 Минимальный пример на Bevy Engine
- 🏗️ ECS архитектура, подходит для больших игр
- ⚠️ Большой размер (~10-20 МБ), долгая компиляция
- 📁 Папка: `bevy/`

### 3️⃣ **wasm-bindgen** (низкоуровневый)
- 🦀 Прямая работа с Canvas API через web-sys
- 🔧 Максимальный контроль над Web APIs
- 📚 Образовательный пример (~20-50 КБ)
- 📁 Папка: `wasm_bindgen/`

---

## ⚡ Быстрый старт (Macroquad)

### 1. Установите Rust
```bash
# Linux/macOS
curl --proto '=https' --tlsv1.2 -sSf https://sh.rustup.rs | sh

# Windows: скачайте с https://rustup.rs/
```

### 2. Соберите проект
```bash
cd macroquad
chmod +x build.sh
./build.sh
```

### 3. Запустите сервер
```bash
python3 -m http.server 8000
```

### 4. Откройте браузер
http://localhost:8000

**Готово! 🎉**

---

## 📂 Структура проекта

```
pustota/
├── macroquad/              ⭐ Основной вариант
│   ├── src/main.rs         - Полная игра (Space Shooter)
│   ├── index.html          - HTML обёртка
│   ├── Cargo.toml          - Конфигурация
│   ├── build.sh / .bat     - Скрипты сборки
│   ├── README.md           - Подробные инструкции
│   └── QUICKSTART.md       - Быстрый старт
│
├── bevy/                   🎮 Bevy Engine вариант
│   ├── src/main.rs         - Минимальный пример
│   ├── index.html          - HTML обёртка
│   ├── Cargo.toml          - Конфигурация
│   └── README.md           - Инструкции
│
├── wasm_bindgen/           🔧 Низкоуровневый вариант
│   ├── src/lib.rs          - Canvas API через web-sys
│   ├── index.html          - HTML с JS интеграцией
│   ├── Cargo.toml          - Конфигурация
│   ├── build.sh            - Скрипт сборки
│   └── README.md           - Инструкции
│
├── README.md               - Этот файл
└── notes_for_devs.md       - Сравнение подходов
```

---

## 🎯 Какой вариант выбрать?

### 🌟 Macroquad - для большинства случаев
**Используйте если:**
- ✅ Хотите быстро создать работающую игру
- ✅ Важен маленький размер файла
- ✅ Нужна простота и скорость разработки
- ✅ Достаточно 2D графики

**Плюсы:**
- Быстрая компиляция (1-2 минуты)
- Маленький размер (~200-500 КБ)
- Простой API
- Готовая поддержка WASM

**Минусы:**
- Ограничен 2D
- Нет ECS архитектуры

---

### 🎮 Bevy - для сложных проектов
**Используйте если:**
- ✅ Нужна ECS архитектура
- ✅ Планируете сложную игру с множеством систем
- ✅ Размер файла не критичен
- ✅ Есть время на компиляцию

**Плюсы:**
- Мощная ECS архитектура
- Большая экосистема плагинов
- Отличная документация
- Поддержка 2D и 3D

**Минусы:**
- Большой размер (~10-20 МБ)
- Долгая компиляция (10-15 мин)
- Сложнее для новичков

---

### 🔧 wasm-bindgen - для обучения и утилит
**Используйте если:**
- ✅ Изучаете WASM и биндинги
- ✅ Нужен максимальный контроль над Web APIs
- ✅ Интегрируете Rust в JS проект
- ✅ Пишете библиотеку, а не игру

**Плюсы:**
- Минимальный размер (~20-50 КБ)
- Прямой доступ к Web APIs
- Понимание внутренностей WASM
- Гибкость интеграции

**Минусы:**
- Много boilerplate кода
- Нет готовых игровых компонентов
- Нужно всё писать с нуля

---

## 📊 Сравнительная таблица

| Параметр | Macroquad | Bevy | wasm-bindgen |
|----------|-----------|------|--------------|
| **Размер WASM** | ~200-500 КБ | ~10-20 МБ | ~20-50 КБ |
| **Время сборки** | 1-2 мин | 10-15 мин | ~1 мин |
| **Сложность** | ⭐ Низкая | ⭐⭐⭐ Высокая | ⭐⭐ Средняя |
| **Для игр** | ✅ Да | ✅ Да | ❌ Нет |
| **ECS** | ❌ | ✅ | ❌ |
| **2D графика** | ✅ | ✅ | Вручную |
| **3D графика** | ❌ | ✅ | Вручную (WebGL) |
| **Web API доступ** | Ограничен | Ограничен | ✅ Полный |
| **Документация** | Хорошая | Отличная | Отличная |
| **Кривая обучения** | Пологая | Крутая | Средняя |

---

## 🛠️ Общие требования

### Установка Rust
```bash
# Linux/macOS
curl --proto '=https' --tlsv1.2 -sSf https://sh.rustup.rs | sh
source $HOME/.cargo/env

# Windows
# Скачайте с https://rustup.rs/
```

### Добавление WASM target
```bash
rustup target add wasm32-unknown-unknown
```

### Дополнительные инструменты

```bash
# Для wasm-bindgen варианта
cargo install wasm-pack

# Для оптимизации размера (опционально)
cargo install wasm-opt
# или
sudo apt install binaryen

# Локальный HTTP сервер
cargo install basic-http-server
```

---

## 🚀 Публикация

### GitHub Pages
```bash
# В папке с проектом (например macroquad/)
./build.sh

git checkout -b gh-pages
git add index.html *.wasm
git commit -m "Deploy game"
git push origin gh-pages
```

Игра будет доступна: `https://username.github.io/repo-name/`

### Netlify / Vercel
Просто загрузите папку с `index.html` и `.wasm` файлом

### Яндекс.Игры / VK Play
```bash
# Упакуйте в ZIP
cd macroquad
zip -r game.zip index.html macroquad_space_shooter.wasm assets/
```

---

## 📚 Документация

Каждый вариант имеет подробный README:
- `macroquad/README.md` - полная инструкция по Macroquad
- `macroquad/QUICKSTART.md` - быстрый старт
- `bevy/README.md` - инструкции по Bevy
- `wasm_bindgen/README.md` - руководство по wasm-bindgen
- `notes_for_devs.md` - детальное сравнение подходов

---

## 🐛 Типичные проблемы

### ❌ Чёрный экран в браузере
✅ Используйте HTTP сервер (не `file://`)
✅ Проверьте консоль браузера (F12)
✅ Убедитесь что `.wasm` файл рядом с `index.html`

### ❌ "cargo not found"
```bash
# Перезапустите терминал и проверьте
cargo --version

# Если не помогло
source $HOME/.cargo/env  # Linux/macOS
```

### ❌ CORS errors
✅ Запустите локальный сервер:
```bash
python3 -m http.server 8000
```

### ❌ Долгая компиляция
✅ Первая сборка долгая (скачивание зависимостей)
✅ Bevy компилируется особенно долго (10-15 мин)
✅ Используйте `cargo build` без `--release` для тестов

---

## 🎓 Полезные ссылки

### Macroquad
- [Официальная документация](https://macroquad.rs/)
- [Примеры кода](https://github.com/not-fl3/macroquad/tree/master/examples)
- [WASM деплой гайд](https://macroquad.rs/tutorials/wasm/)

### Bevy
- [Официальный сайт](https://bevyengine.org/)
- [Bevy Cheatbook](https://bevy-cheatbook.github.io/)
- [WASM примеры](https://github.com/bevyengine/bevy/tree/main/examples#wasm)

### wasm-bindgen
- [Rust WASM Book](https://rustwasm.github.io/book/)
- [wasm-bindgen Guide](https://rustwasm.github.io/wasm-bindgen/)
- [web-sys API](https://rustwasm.github.io/wasm-bindgen/api/web_sys/)

### Общее
- [MDN WebAssembly](https://developer.mozilla.org/en-US/docs/WebAssembly)
- [Awesome Rust WASM](https://github.com/rustwasm/awesome-rust-and-webassembly)

---

## 💡 Рекомендации

1. **Начните с Macroquad** - самый простой путь к результату
2. **Используйте `--release`** только для финальной сборки
3. **Оптимизируйте с wasm-opt** перед публикацией
4. **Тестируйте в разных браузерах** (Chrome, Firefox, Safari)
5. **Включите gzip** на сервере для уменьшения размера

---

## 🤝 Вклад

Нашли баг или хотите улучшить проект? Создайте Issue или Pull Request!

---

## 📄 Лицензия

MIT License - свободно используйте для своих проектов

---

## 🎉 Готово!

Выберите вариант, следуйте инструкциям в соответствующей папке и создавайте игры на Rust для браузера!

**Удачи в разработке! 🦀🚀**
