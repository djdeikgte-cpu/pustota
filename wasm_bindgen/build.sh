#!/bin/bash

# ============================================================================
# Скрипт сборки wasm-bindgen проекта
# ============================================================================

echo "🦀 Начинаю сборку wasm-bindgen проекта..."

# Проверка wasm-pack
if ! command -v wasm-pack &> /dev/null; then
    echo "❌ Ошибка: wasm-pack не установлен!"
    echo "Установите: cargo install wasm-pack"
    exit 1
fi

# Сборка с wasm-pack
echo "🔨 Компилирую с wasm-pack..."
wasm-pack build --target web --release

if [ $? -eq 0 ]; then
    echo "✅ Сборка успешна!"
    echo ""
    echo "📁 Файлы находятся в: pkg/"
    echo ""

    # Копируем HTML в pkg
    echo "📋 Копирую index.html в pkg/..."
    cp index.html pkg/

    echo ""
    echo "📋 Следующие шаги:"
    echo "   1. Запустите локальный сервер:"
    echo "      python3 -m http.server 8000"
    echo "   2. Откройте: http://localhost:8000/pkg/"
    echo ""

    # Опционально: wasm-opt
    if command -v wasm-opt &> /dev/null; then
        echo "🔧 Оптимизирую с wasm-opt..."
        wasm-opt -Oz pkg/wasm_bindgen_canvas_game_bg.wasm \
            -o pkg/wasm_bindgen_canvas_game_bg.wasm
        echo "✅ Оптимизация завершена!"
    else
        echo "💡 Совет: Установите wasm-opt для уменьшения размера:"
        echo "   cargo install wasm-opt"
    fi

    echo ""
    echo "✅ Готово! Запустите HTTP сервер и откройте pkg/index.html"
else
    echo "❌ Ошибка при сборке!"
    exit 1
fi
