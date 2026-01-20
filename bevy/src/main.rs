use bevy::prelude::*;

// ============================================================================
// КОМПОНЕНТЫ
// ============================================================================

#[derive(Component)]
struct Player {
    speed: f32,
}

#[derive(Component)]
struct Enemy;

#[derive(Component)]
struct Velocity(Vec2);

// ============================================================================
// ОСНОВНАЯ ФУНКЦИЯ
// ============================================================================

fn main() {
    App::new()
        .add_plugins(DefaultPlugins.set(WindowPlugin {
            primary_window: Some(Window {
                title: "Bevy WASM Demo".to_string(),
                resolution: (800.0, 600.0).into(),
                canvas: Some("#bevy-canvas".to_string()), // ID canvas в HTML
                ..default()
            }),
            ..default()
        }))
        .add_systems(Startup, setup)
        .add_systems(Update, (
            move_player,
            move_enemies,
            check_collisions,
        ))
        .run();
}

// ============================================================================
// СИСТЕМЫ
// ============================================================================

/// Инициализация игры
fn setup(mut commands: Commands) {
    // Камера
    commands.spawn(Camera2dBundle::default());

    // Игрок (зелёный квадрат)
    commands.spawn((
        SpriteBundle {
            sprite: Sprite {
                color: Color::rgb(0.0, 1.0, 0.0),
                custom_size: Some(Vec2::new(50.0, 50.0)),
                ..default()
            },
            transform: Transform::from_xyz(0.0, -200.0, 0.0),
            ..default()
        },
        Player { speed: 300.0 },
    ));

    // Враги (красные квадраты)
    for i in 0..5 {
        commands.spawn((
            SpriteBundle {
                sprite: Sprite {
                    color: Color::rgb(1.0, 0.0, 0.0),
                    custom_size: Some(Vec2::new(40.0, 40.0)),
                    ..default()
                },
                transform: Transform::from_xyz(
                    -200.0 + i as f32 * 100.0,
                    200.0,
                    0.0,
                ),
                ..default()
            },
            Enemy,
            Velocity(Vec2::new(0.0, -50.0)),
        ));
    }
}

/// Движение игрока
fn move_player(
    keyboard: Res<Input<KeyCode>>,
    time: Res<Time>,
    mut query: Query<(&Player, &mut Transform)>,
) {
    for (player, mut transform) in query.iter_mut() {
        let mut direction = Vec2::ZERO;

        if keyboard.pressed(KeyCode::Left) || keyboard.pressed(KeyCode::A) {
            direction.x -= 1.0;
        }
        if keyboard.pressed(KeyCode::Right) || keyboard.pressed(KeyCode::D) {
            direction.x += 1.0;
        }
        if keyboard.pressed(KeyCode::Up) || keyboard.pressed(KeyCode::W) {
            direction.y += 1.0;
        }
        if keyboard.pressed(KeyCode::Down) || keyboard.pressed(KeyCode::S) {
            direction.y -= 1.0;
        }

        if direction != Vec2::ZERO {
            direction = direction.normalize();
        }

        transform.translation += (direction * player.speed * time.delta_seconds()).extend(0.0);

        // Ограничение движения
        transform.translation.x = transform.translation.x.clamp(-380.0, 380.0);
        transform.translation.y = transform.translation.y.clamp(-280.0, 280.0);
    }
}

/// Движение врагов
fn move_enemies(
    time: Res<Time>,
    mut query: Query<(&Velocity, &mut Transform), With<Enemy>>,
) {
    for (velocity, mut transform) in query.iter_mut() {
        transform.translation += (velocity.0 * time.delta_seconds()).extend(0.0);

        // Враги возвращаются наверх когда уходят за экран
        if transform.translation.y < -300.0 {
            transform.translation.y = 300.0;
        }
    }
}

/// Проверка коллизий
fn check_collisions(
    player_query: Query<&Transform, With<Player>>,
    enemy_query: Query<&Transform, With<Enemy>>,
) {
    for player_transform in player_query.iter() {
        for enemy_transform in enemy_query.iter() {
            let distance = player_transform
                .translation
                .distance(enemy_transform.translation);

            if distance < 45.0 {
                // Простая проверка столкновения
                println!("Collision detected!");
            }
        }
    }
}
