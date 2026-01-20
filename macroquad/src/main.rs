use macroquad::prelude::*;

// ============================================================================
// КОНСТАНТЫ ИГРЫ
// ============================================================================

const PLAYER_SPEED: f32 = 5.0;
const PLAYER_SIZE: f32 = 32.0;
const BULLET_SPEED: f32 = 8.0;
const BULLET_SIZE: f32 = 8.0;
const ENEMY_SPEED: f32 = 2.0;
const FAST_ENEMY_SPEED: f32 = 4.0;
const ENEMY_SIZE: f32 = 32.0;
const BONUS_SIZE: f32 = 24.0;
const BONUS_SPEED: f32 = 1.5;

// ============================================================================
// СТРУКТУРЫ ДАННЫХ
// ============================================================================

#[derive(Clone)]
struct Player {
    pos: Vec2,
    alive: bool,
    score: u32,
}

#[derive(Clone)]
struct Bullet {
    pos: Vec2,
    active: bool,
}

#[derive(Clone, PartialEq)]
enum EnemyType {
    Normal,  // Обычный враг
    Fast,    // Быстрый враг
}

#[derive(Clone)]
struct Enemy {
    pos: Vec2,
    active: bool,
    enemy_type: EnemyType,
}

#[derive(Clone)]
struct Bonus {
    pos: Vec2,
    active: bool,
}

enum GameState {
    Playing,
    Victory,
    GameOver,
}

// ============================================================================
// ОСНОВНОЙ ИГРОВОЙ ЦИКЛ
// ============================================================================

#[macroquad::main("Space Shooter")]
async fn main() {
    // Инициализация игрока
    let mut player = Player {
        pos: vec2(screen_width() / 2.0, screen_height() - 50.0),
        alive: true,
        score: 0,
    };

    // Пулы объектов для оптимизации (object pooling)
    let mut bullets: Vec<Bullet> = Vec::new();
    let mut enemies: Vec<Enemy> = Vec::new();
    let mut bonuses: Vec<Bonus> = Vec::new();

    let mut game_state = GameState::Playing;
    let mut spawn_timer = 0.0;
    let mut bonus_timer = 0.0;
    let mut enemies_killed = 0;

    const WIN_CONDITION: u32 = 30; // Победа при 30 убитых врагах

    loop {
        clear_background(Color::from_rgba(10, 10, 30, 255));

        match game_state {
            GameState::Playing => {
                // ============================================================
                // УПРАВЛЕНИЕ ИГРОКОМ
                // ============================================================

                if is_key_down(KeyCode::Left) || is_key_down(KeyCode::A) {
                    player.pos.x -= PLAYER_SPEED;
                }
                if is_key_down(KeyCode::Right) || is_key_down(KeyCode::D) {
                    player.pos.x += PLAYER_SPEED;
                }
                if is_key_down(KeyCode::Up) || is_key_down(KeyCode::W) {
                    player.pos.y -= PLAYER_SPEED;
                }
                if is_key_down(KeyCode::Down) || is_key_down(KeyCode::S) {
                    player.pos.y += PLAYER_SPEED;
                }

                // Ограничение движения границами экрана
                player.pos.x = player.pos.x.clamp(PLAYER_SIZE / 2.0, screen_width() - PLAYER_SIZE / 2.0);
                player.pos.y = player.pos.y.clamp(PLAYER_SIZE / 2.0, screen_height() - PLAYER_SIZE / 2.0);

                // Стрельба (пробел или клик мыши)
                if is_key_pressed(KeyCode::Space) || is_mouse_button_pressed(MouseButton::Left) {
                    bullets.push(Bullet {
                        pos: vec2(player.pos.x, player.pos.y - PLAYER_SIZE / 2.0),
                        active: true,
                    });
                }

                // ============================================================
                // ОБНОВЛЕНИЕ ПУЛЬ
                // ============================================================

                for bullet in &mut bullets {
                    if bullet.active {
                        bullet.pos.y -= BULLET_SPEED;

                        // Деактивируем пули за экраном
                        if bullet.pos.y < 0.0 {
                            bullet.active = false;
                        }
                    }
                }

                // ============================================================
                // СПАВН ВРАГОВ
                // ============================================================

                spawn_timer += get_frame_time();
                if spawn_timer > 1.0 {
                    spawn_timer = 0.0;
                    let x = rand::gen_range(ENEMY_SIZE, screen_width() - ENEMY_SIZE);

                    // 30% шанс на быстрого врага
                    let enemy_type = if rand::gen_range(0.0, 1.0) < 0.3 {
                        EnemyType::Fast
                    } else {
                        EnemyType::Normal
                    };

                    enemies.push(Enemy {
                        pos: vec2(x, -ENEMY_SIZE),
                        active: true,
                        enemy_type,
                    });
                }

                // ============================================================
                // ОБНОВЛЕНИЕ ВРАГОВ
                // ============================================================

                for enemy in &mut enemies {
                    if enemy.active {
                        let speed = match enemy.enemy_type {
                            EnemyType::Normal => ENEMY_SPEED,
                            EnemyType::Fast => FAST_ENEMY_SPEED,
                        };
                        enemy.pos.y += speed;

                        // Деактивируем врагов за экраном
                        if enemy.pos.y > screen_height() {
                            enemy.active = false;
                        }
                    }
                }

                // ============================================================
                // СПАВН БОНУСОВ
                // ============================================================

                bonus_timer += get_frame_time();
                if bonus_timer > 5.0 {
                    bonus_timer = 0.0;
                    let x = rand::gen_range(BONUS_SIZE, screen_width() - BONUS_SIZE);

                    bonuses.push(Bonus {
                        pos: vec2(x, -BONUS_SIZE),
                        active: true,
                    });
                }

                // ============================================================
                // ОБНОВЛЕНИЕ БОНУСОВ
                // ============================================================

                for bonus in &mut bonuses {
                    if bonus.active {
                        bonus.pos.y += BONUS_SPEED;

                        if bonus.pos.y > screen_height() {
                            bonus.active = false;
                        }
                    }
                }

                // ============================================================
                // КОЛЛИЗИИ: ПУЛИ И ВРАГИ
                // ============================================================

                for bullet in &mut bullets {
                    if !bullet.active {
                        continue;
                    }

                    for enemy in &mut enemies {
                        if !enemy.active {
                            continue;
                        }

                        if check_collision(bullet.pos, BULLET_SIZE, enemy.pos, ENEMY_SIZE) {
                            bullet.active = false;
                            enemy.active = false;

                            // Разные очки за разных врагов
                            let points = match enemy.enemy_type {
                                EnemyType::Normal => 10,
                                EnemyType::Fast => 20,
                            };
                            player.score += points;
                            enemies_killed += 1;

                            if enemies_killed >= WIN_CONDITION {
                                game_state = GameState::Victory;
                            }
                        }
                    }
                }

                // ============================================================
                // КОЛЛИЗИИ: ИГРОК И ВРАГИ
                // ============================================================

                for enemy in &enemies {
                    if !enemy.active {
                        continue;
                    }

                    if check_collision(player.pos, PLAYER_SIZE, enemy.pos, ENEMY_SIZE) {
                        player.alive = false;
                        game_state = GameState::GameOver;
                    }
                }

                // ============================================================
                // КОЛЛИЗИИ: ИГРОК И БОНУСЫ
                // ============================================================

                for bonus in &mut bonuses {
                    if !bonus.active {
                        continue;
                    }

                    if check_collision(player.pos, PLAYER_SIZE, bonus.pos, BONUS_SIZE) {
                        bonus.active = false;
                        player.score += 50;
                    }
                }

                // ============================================================
                // ОТРИСОВКА
                // ============================================================

                // Рисуем игрока (зелёный треугольник)
                draw_triangle(
                    vec2(player.pos.x, player.pos.y - PLAYER_SIZE / 2.0),
                    vec2(player.pos.x - PLAYER_SIZE / 2.0, player.pos.y + PLAYER_SIZE / 2.0),
                    vec2(player.pos.x + PLAYER_SIZE / 2.0, player.pos.y + PLAYER_SIZE / 2.0),
                    GREEN,
                );

                // Рисуем пули
                for bullet in &bullets {
                    if bullet.active {
                        draw_circle(bullet.pos.x, bullet.pos.y, BULLET_SIZE / 2.0, YELLOW);
                    }
                }

                // Рисуем врагов
                for enemy in &enemies {
                    if enemy.active {
                        let color = match enemy.enemy_type {
                            EnemyType::Normal => RED,
                            EnemyType::Fast => ORANGE,
                        };
                        draw_rectangle(
                            enemy.pos.x - ENEMY_SIZE / 2.0,
                            enemy.pos.y - ENEMY_SIZE / 2.0,
                            ENEMY_SIZE,
                            ENEMY_SIZE,
                            color,
                        );
                    }
                }

                // Рисуем бонусы (синие ромбы)
                for bonus in &bonuses {
                    if bonus.active {
                        draw_poly(bonus.pos.x, bonus.pos.y, 4, BONUS_SIZE / 2.0, 45.0, BLUE);
                    }
                }

                // HUD: Счётчик очков
                draw_text(
                    &format!("Score: {}", player.score),
                    10.0,
                    30.0,
                    30.0,
                    WHITE,
                );
                draw_text(
                    &format!("Enemies: {}/{}", enemies_killed, WIN_CONDITION),
                    10.0,
                    60.0,
                    30.0,
                    WHITE,
                );

                // Инструкции
                draw_text(
                    "WASD/Arrows: Move | Space/Click: Shoot",
                    10.0,
                    screen_height() - 10.0,
                    20.0,
                    GRAY,
                );
            }

            GameState::Victory => {
                draw_text(
                    "VICTORY!",
                    screen_width() / 2.0 - 120.0,
                    screen_height() / 2.0 - 50.0,
                    60.0,
                    GREEN,
                );
                draw_text(
                    &format!("Final Score: {}", player.score),
                    screen_width() / 2.0 - 100.0,
                    screen_height() / 2.0 + 20.0,
                    30.0,
                    WHITE,
                );
                draw_text(
                    "Press R to Restart",
                    screen_width() / 2.0 - 100.0,
                    screen_height() / 2.0 + 60.0,
                    25.0,
                    GRAY,
                );

                if is_key_pressed(KeyCode::R) {
                    // Сброс игры
                    player = Player {
                        pos: vec2(screen_width() / 2.0, screen_height() - 50.0),
                        alive: true,
                        score: 0,
                    };
                    bullets.clear();
                    enemies.clear();
                    bonuses.clear();
                    enemies_killed = 0;
                    game_state = GameState::Playing;
                }
            }

            GameState::GameOver => {
                draw_text(
                    "GAME OVER",
                    screen_width() / 2.0 - 130.0,
                    screen_height() / 2.0 - 50.0,
                    60.0,
                    RED,
                );
                draw_text(
                    &format!("Score: {}", player.score),
                    screen_width() / 2.0 - 80.0,
                    screen_height() / 2.0 + 20.0,
                    30.0,
                    WHITE,
                );
                draw_text(
                    "Press R to Restart",
                    screen_width() / 2.0 - 100.0,
                    screen_height() / 2.0 + 60.0,
                    25.0,
                    GRAY,
                );

                if is_key_pressed(KeyCode::R) {
                    // Сброс игры
                    player = Player {
                        pos: vec2(screen_width() / 2.0, screen_height() - 50.0),
                        alive: true,
                        score: 0,
                    };
                    bullets.clear();
                    enemies.clear();
                    bonuses.clear();
                    enemies_killed = 0;
                    game_state = GameState::Playing;
                }
            }
        }

        next_frame().await
    }
}

// ============================================================================
// ВСПОМОГАТЕЛЬНЫЕ ФУНКЦИИ
// ============================================================================

/// Проверка столкновения двух объектов (простая круговая коллизия)
fn check_collision(pos1: Vec2, size1: f32, pos2: Vec2, size2: f32) -> bool {
    let distance = pos1.distance(pos2);
    distance < (size1 + size2) / 2.0
}
