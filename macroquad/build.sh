#!/bin/bash

# ============================================================================
# Скрипт сборки Rust игры в WebAssembly (Macroquad)
# ============================================================================

echo "🚀 Начинаю сборку Macroquad Space Shooter для WebAssembly..."

# Проверяем наличие Rust
if ! command -v cargo &> /dev/null; then
    echo "❌ Ошибка: Cargo не установлен!"
    echo "Установите Rust: https://rustup.rs/"
    exit 1
fi

# Добавляем target для WebAssembly (если ещё не добавлен)
echo "📦 Добавляю target wasm32-unknown-unknown..."
rustup target add wasm32-unknown-unknown

# Собираем проект
echo "🔨 Компилирую проект..."
cargo build --release --target wasm32-unknown-unknown

# Проверяем успешность сборки
if [ $? -eq 0 ]; then
    echo "✅ Сборка успешна!"
    echo ""
    echo "📁 WASM файл находится здесь:"
    echo "   target/wasm32-unknown-unknown/release/macroquad_space_shooter.wasm"
    echo ""
    echo "📋 Следующие шаги:"
    echo "   1. Скопируйте WASM файл в папку с index.html:"
    echo "      cp target/wasm32-unknown-unknown/release/macroquad_space_shooter.wasm ."
    echo ""
    echo "   2. Запустите локальный сервер:"
    echo "      python3 -m http.server 8000"
    echo "      или"
    echo "      basic-http-server ."
    echo ""
    echo "   3. Откройте в браузере:"
    echo "      http://localhost:8000"
    echo ""

    # Автоматически копируем WASM файл
    echo "📋 Копирую WASM файл..."
    cp target/wasm32-unknown-unknown/release/macroquad_space_shooter.wasm .

    echo "✅ Готово! Теперь запустите локальный сервер."
else
    echo "❌ Ошибка при сборке!"
    exit 1
fi

# Опционально: оптимизация размера с wasm-opt (если установлен)
if command -v wasm-opt &> /dev/null; then
    echo "🔧 Оптимизирую размер WASM файла..."
    wasm-opt -Oz macroquad_space_shooter.wasm -o macroquad_space_shooter.wasm
    echo "✅ Оптимизация завершена!"
else
    echo "💡 Совет: Установите wasm-opt для уменьшения размера:"
    echo "   cargo install wasm-opt"
fi

echo ""
echo "🎮 Игра готова к запуску!"
