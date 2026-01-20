@echo off
REM ============================================================================
REM Скрипт сборки Rust игры в WebAssembly для Windows
REM ============================================================================

echo 🚀 Начинаю сборку Macroquad Space Shooter для WebAssembly...

REM Проверяем наличие Rust
where cargo >nul 2>nul
if %errorlevel% neq 0 (
    echo ❌ Ошибка: Cargo не установлен!
    echo Установите Rust: https://rustup.rs/
    pause
    exit /b 1
)

REM Добавляем target для WebAssembly
echo 📦 Добавляю target wasm32-unknown-unknown...
rustup target add wasm32-unknown-unknown

REM Собираем проект
echo 🔨 Компилирую проект...
cargo build --release --target wasm32-unknown-unknown

if %errorlevel% equ 0 (
    echo ✅ Сборка успешна!
    echo.
    echo 📁 WASM файл находится здесь:
    echo    target\wasm32-unknown-unknown\release\macroquad_space_shooter.wasm
    echo.

    REM Копируем WASM файл
    echo 📋 Копирую WASM файл...
    copy /Y target\wasm32-unknown-unknown\release\macroquad_space_shooter.wasm .

    echo.
    echo 📋 Следующие шаги:
    echo    1. Запустите локальный сервер:
    echo       python -m http.server 8000
    echo.
    echo    2. Откройте в браузере:
    echo       http://localhost:8000
    echo.
    echo ✅ Готово! Теперь запустите локальный сервер.
) else (
    echo ❌ Ошибка при сборке!
    pause
    exit /b 1
)

echo.
echo 🎮 Игра готова к запуску!
pause
