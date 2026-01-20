# ⚡ Быстрый старт - 3 простых шага

## 1️⃣ Установите Rust (если ещё нет)

### Linux/macOS:
```bash
curl --proto '=https' --tlsv1.2 -sSf https://sh.rustup.rs | sh
source $HOME/.cargo/env
```

### Windows:
Скачайте и установите: https://rustup.rs/

---

## 2️⃣ Соберите игру

### Linux/macOS:
```bash
chmod +x build.sh
./build.sh
```

### Windows:
```cmd
build.bat
```

### Или вручную (все ОС):
```bash
rustup target add wasm32-unknown-unknown
cargo build --release --target wasm32-unknown-unknown
cp target/wasm32-unknown-unknown/release/macroquad_space_shooter.wasm .
```

---

## 3️⃣ Запустите локальный сервер

### Вариант A: Python (проще всего)
```bash
python3 -m http.server 8000
```

### Вариант B: Cargo basic-http-server
```bash
cargo install basic-http-server
basic-http-server .
```

---

## 4️⃣ Откройте в браузере

Перейдите: **http://localhost:8000**

---

## 🎮 Управление

- **WASD** или **Стрелки** - Движение
- **Пробел** или **Клик мыши** - Выстрел
- **R** - Перезапуск (после победы/поражения)

---

## ❓ Не работает?

### Проблема: Чёрный экран
✅ **Решение**: Проверьте что файл `macroquad_space_shooter.wasm` находится в той же папке что и `index.html`

### Проблема: "Cannot load WASM"
✅ **Решение**: Используйте локальный HTTP сервер, не открывайте `index.html` напрямую!

### Проблема: "cargo not found"
✅ **Решение**:
```bash
# Перезапустите терминал и проверьте:
cargo --version

# Если не помогло, добавьте в PATH:
source $HOME/.cargo/env  # Linux/macOS
```

---

## 📖 Больше информации

Смотрите **README.md** для подробных инструкций и настройки.

**Удачи! 🚀**
