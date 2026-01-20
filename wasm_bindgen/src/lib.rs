use wasm_bindgen::prelude::*;
use wasm_bindgen::JsCast;
use web_sys::{CanvasRenderingContext2d, HtmlCanvasElement, KeyboardEvent};

// ============================================================================
// КОНСТАНТЫ
// ============================================================================

const CANVAS_WIDTH: f64 = 800.0;
const CANVAS_HEIGHT: f64 = 600.0;
const PLAYER_SIZE: f64 = 30.0;
const PLAYER_SPEED: f64 = 5.0;

// ============================================================================
// СТРУКТУРА ИГРЫ
// ============================================================================

#[wasm_bindgen]
pub struct Game {
    canvas: HtmlCanvasElement,
    context: CanvasRenderingContext2d,
    player_x: f64,
    player_y: f64,
    keys_pressed: Vec<String>,
    score: u32,
}

// ============================================================================
// ПУБЛИЧНЫЕ МЕТОДЫ (вызываются из JavaScript)
// ============================================================================

#[wasm_bindgen]
impl Game {
    /// Создание новой игры
    #[wasm_bindgen(constructor)]
    pub fn new(canvas_id: &str) -> Result<Game, JsValue> {
        // Получаем доступ к DOM
        let window = web_sys::window().expect("no global `window` exists");
        let document = window.document().expect("should have a document on window");

        // Получаем canvas элемент
        let canvas = document
            .get_element_by_id(canvas_id)
            .expect("canvas not found")
            .dyn_into::<HtmlCanvasElement>()?;

        // Настраиваем размеры canvas
        canvas.set_width(CANVAS_WIDTH as u32);
        canvas.set_height(CANVAS_HEIGHT as u32);

        // Получаем 2D контекст
        let context = canvas
            .get_context("2d")?
            .expect("failed to get 2d context")
            .dyn_into::<CanvasRenderingContext2d>()?;

        // Логируем в консоль
        web_sys::console::log_1(&"Game initialized!".into());

        Ok(Game {
            canvas,
            context,
            player_x: CANVAS_WIDTH / 2.0,
            player_y: CANVAS_HEIGHT - 100.0,
            keys_pressed: Vec::new(),
            score: 0,
        })
    }

    /// Обработка нажатия клавиши
    pub fn key_down(&mut self, key: String) {
        if !self.keys_pressed.contains(&key) {
            self.keys_pressed.push(key);
        }
    }

    /// Обработка отпускания клавиши
    pub fn key_up(&mut self, key: String) {
        self.keys_pressed.retain(|k| k != &key);
    }

    /// Обновление игровой логики
    pub fn update(&mut self) {
        // Движение игрока
        for key in &self.keys_pressed {
            match key.as_str() {
                "ArrowLeft" | "a" | "A" => {
                    self.player_x = (self.player_x - PLAYER_SPEED).max(PLAYER_SIZE / 2.0);
                }
                "ArrowRight" | "d" | "D" => {
                    self.player_x = (self.player_x + PLAYER_SPEED)
                        .min(CANVAS_WIDTH - PLAYER_SIZE / 2.0);
                }
                "ArrowUp" | "w" | "W" => {
                    self.player_y = (self.player_y - PLAYER_SPEED).max(PLAYER_SIZE / 2.0);
                }
                "ArrowDown" | "s" | "S" => {
                    self.player_y = (self.player_y + PLAYER_SPEED)
                        .min(CANVAS_HEIGHT - PLAYER_SIZE / 2.0);
                }
                "Space" | " " => {
                    self.score += 1;
                }
                _ => {}
            }
        }
    }

    /// Отрисовка
    pub fn render(&self) -> Result<(), JsValue> {
        // Очистка экрана
        self.context.set_fill_style(&"#0a0a1e".into());
        self.context
            .fill_rect(0.0, 0.0, CANVAS_WIDTH, CANVAS_HEIGHT);

        // Рисуем игрока (зелёный треугольник)
        self.context.set_fill_style(&"#00ff00".into());
        self.context.begin_path();
        self.context
            .move_to(self.player_x, self.player_y - PLAYER_SIZE / 2.0);
        self.context.line_to(
            self.player_x - PLAYER_SIZE / 2.0,
            self.player_y + PLAYER_SIZE / 2.0,
        );
        self.context.line_to(
            self.player_x + PLAYER_SIZE / 2.0,
            self.player_y + PLAYER_SIZE / 2.0,
        );
        self.context.close_path();
        self.context.fill();

        // Рисуем декоративные звёзды
        self.context.set_fill_style(&"#ffffff".into());
        for i in 0..50 {
            let x = (i * 37 % CANVAS_WIDTH as i32) as f64;
            let y = (i * 53 % CANVAS_HEIGHT as i32) as f64;
            self.context.fill_rect(x, y, 2.0, 2.0);
        }

        // HUD: Счёт
        self.context.set_fill_style(&"#ffffff".into());
        self.context.set_font("30px Arial");
        self.context
            .fill_text(&format!("Score: {}", self.score), 10.0, 40.0)?;

        // Инструкции
        self.context.set_font("16px Arial");
        self.context.fill_text(
            "WASD/Arrows: Move | Space: +Score",
            10.0,
            CANVAS_HEIGHT - 10.0,
        )?;

        Ok(())
    }

    /// Получить текущий счёт (для JS)
    pub fn get_score(&self) -> u32 {
        self.score
    }
}

// ============================================================================
// ТОЧКА ВХОДА (опционально)
// ============================================================================

/// Инициализация при загрузке модуля
#[wasm_bindgen(start)]
pub fn main() -> Result<(), JsValue> {
    web_sys::console::log_1(&"WASM module loaded!".into());
    Ok(())
}
