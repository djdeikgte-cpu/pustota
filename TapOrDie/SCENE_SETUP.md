# Tap or Die - Scene Setup Guide

This guide explains how to set up the Unity scenes for the "Tap or Die" game.

---

## Project Setup

### 1. Create New Unity Project
1. Open Unity Hub
2. Create new project with **2D Core** template
3. Unity version: **2021.3 LTS** or newer
4. Name: `TapOrDie`

### 2. Import Scripts
Copy all scripts from `Assets/Scripts/` into your Unity project.

### 3. Install Required Packages
Open **Window > Package Manager** and install:
- TextMeshPro (usually included)
- 2D Sprite (usually included)

### 4. Create Tags and Layers

**Tags** (Edit > Project Settings > Tags and Layers):
- `Player`
- `Obstacle`
- `Ground`
- `Collectible`

**Layers**:
- `Ground` (Layer 6)
- `Player` (Layer 7)
- `Obstacle` (Layer 8)

---

## Scene 1: MainMenu

### Hierarchy Structure:
```
MainMenu
├── Main Camera
├── Canvas (Screen Space - Overlay)
│   ├── Background (Image - dark color)
│   ├── Title (TextMeshPro - "TAP OR DIE")
│   ├── PlayButton (Button)
│   │   └── Text (TextMeshPro - "PLAY")
│   ├── HighScoreText (TextMeshPro)
│   ├── MusicToggleButton (Button)
│   │   └── MusicIcon (Image)
│   └── VersionText (TextMeshPro - optional)
├── EventSystem
├── GameInitializer (Empty GameObject)
│   └── [Add GameInitializer.cs script]
└── AudioListener (if not on camera)
```

### MainMenu Canvas Setup:
1. Create **Canvas**: UI > Canvas
   - Render Mode: Screen Space - Overlay
   - UI Scale Mode: Scale With Screen Size
   - Reference Resolution: 1920x1080
   - Match: 0.5 (Width-Height)

2. Add **CanvasScaler** settings:
   - UI Scale Mode: Scale With Screen Size
   - Reference Resolution: 1920 x 1080

3. Create **MainMenuUI** component on Canvas or separate GameObject

### Button Styling:
```
PlayButton:
- Width: 300, Height: 80
- Colors: Normal=#E94560, Highlighted=#FF6B6B, Pressed=#C73E54
- Font Size: 48
- Font Style: Bold
```

---

## Scene 2: GameScene

### Hierarchy Structure:
```
GameScene
├── Main Camera
│   └── [Position: 0, 0, -10]
├── Canvas (Screen Space - Overlay)
│   ├── HUD
│   │   ├── ScoreText (TextMeshPro - top center)
│   │   ├── HighScoreText (TextMeshPro - top right)
│   │   ├── PauseButton (Button - top right)
│   │   │   └── PauseIcon (Image)
│   │   ├── MusicButton (Button - top right)
│   │   │   └── MusicIcon (Image)
│   │   └── SpeedUpText (TextMeshPro - center, hidden)
│   ├── PausePanel (Panel - initially inactive)
│   │   ├── PauseBackground (Image - semi-transparent)
│   │   ├── PauseTitle (TextMeshPro)
│   │   ├── ResumeButton (Button)
│   │   ├── RestartButton (Button)
│   │   ├── MenuButton (Button)
│   │   └── MusicToggle (Button)
│   └── GameOverPanel (Panel - initially inactive)
│       ├── GameOverBackground (Image)
│       ├── GameOverTitle (TextMeshPro)
│       ├── ScoreText (TextMeshPro)
│       ├── HighScoreText (TextMeshPro)
│       ├── NewRecordText (TextMeshPro - hidden by default)
│       ├── RestartButton (Button)
│       ├── MenuButton (Button)
│       └── RewardedAdButton (Button)
├── EventSystem
├── Player
│   ├── PlayerSprite (SpriteRenderer - Circle)
│   ├── GroundCheck (Empty - child, position Y=-0.5)
│   ├── JumpParticles (ParticleSystem - optional)
│   └── Components:
│       ├── Rigidbody2D (Gravity: 3, Freeze Z rotation)
│       ├── CircleCollider2D
│       └── PlayerController.cs
├── Ground
│   ├── GroundSegment1 (SpriteRenderer - wide rectangle)
│   │   └── BoxCollider2D
│   ├── GroundSegment2
│   └── GroundSegment3
│   └── Ground.cs component
├── ObstacleManager (Empty GameObject)
│   └── ObstacleManager.cs
├── Background
│   ├── BackgroundLayer1 (SpriteRenderer, Order: -10)
│   │   └── BackgroundScroller.cs (parallax: 0.3)
│   ├── BackgroundLayer2 (SpriteRenderer, Order: -5)
│   │   └── BackgroundScroller.cs (parallax: 0.5)
├── Managers
│   └── [GameHUD.cs, PauseUI.cs, GameOverUI.cs]
└── Audio (if separate from AudioManager singleton)
```

### Player Setup:
```
Player GameObject:
- Position: (-5, 0, 0)
- Tag: Player
- Layer: Player

Rigidbody2D:
- Body Type: Dynamic
- Gravity Scale: 3
- Freeze Rotation: Z (checked)
- Collision Detection: Continuous

CircleCollider2D:
- Radius: 0.5

PlayerController.cs:
- Jump Force: 10
- Max Jump Velocity: 12
- Fall Multiplier: 2.5
- Ground Check Radius: 0.2
- Ground Layer: Ground layer
```

### Ground Setup:
```
Ground Segments:
- 3 segments, each 20 units wide
- Position Y: -4
- Tag: Ground
- Layer: Ground

BoxCollider2D:
- Size matches sprite
```

### Obstacle Prefabs (create in Prefabs folder):

**Obstacle_Spike:**
```
- SpriteRenderer (Triangle/spike shape)
- BoxCollider2D (Is Trigger: true)
- Tag: Obstacle
- Color: Red (#E94560)
```

**Obstacle_Block:**
```
- SpriteRenderer (Square)
- BoxCollider2D (Is Trigger: true)
- Tag: Obstacle
- Color: Red (#E94560)
```

**Obstacle_Tall:**
```
- SpriteRenderer (Tall rectangle)
- BoxCollider2D (Is Trigger: true)
- Tag: Obstacle
```

---

## Scene 3: GameOverScene (Optional - can use panel in GameScene)

If creating separate scene:
```
GameOverScene
├── Main Camera
├── Canvas
│   ├── Background
│   ├── GameOverTitle
│   ├── ScoreText
│   ├── HighScoreText
│   ├── NewRecordBadge
│   ├── RestartButton
│   ├── MenuButton
│   └── WatchAdButton
└── EventSystem
```

---

## Prefab Setup

### Create these prefabs in Assets/Prefabs/:

**1. GameManager Prefab:**
```
Empty GameObject
├── GameManager.cs
Settings:
- Base Speed: 5
- Speed Increase Interval: 10
- Speed Increase Amount: 0.5
- Max Speed: 15
```

**2. AudioManager Prefab:**
```
Empty GameObject
├── AudioManager.cs
├── MusicSource (AudioSource - child)
│   └── Loop: true, Play On Awake: false
├── SFXSource (AudioSource - child)
│   └── Loop: false, Play On Awake: false
```

**3. LocalizationManager Prefab:**
```
Empty GameObject
├── LocalizationManager.cs
```

**4. YandexAdsManager Prefab:**
```
Empty GameObject
├── YandexAdsManager.cs
Settings:
- Enable Ads: true
- Min Time Between Fullscreen Ads: 60
```

**5. Obstacle Prefabs (multiple):**
```
Obstacle_Spike
├── SpriteRenderer
├── BoxCollider2D (Is Trigger: true)
├── Obstacle.cs (optional)
Tag: Obstacle
```

---

## Camera Settings

```
Main Camera:
- Clear Flags: Solid Color
- Background: #1A1A2E (dark blue)
- Projection: Orthographic
- Size: 5
- Position: (0, 0, -10)
```

---

## Build Settings

### Add Scenes to Build:
1. File > Build Settings
2. Add scenes in order:
   - Scenes/MainMenu (index 0)
   - Scenes/GameScene (index 1)

### WebGL Player Settings:
1. Edit > Project Settings > Player
2. Select WebGL tab

**Resolution and Presentation:**
- Default Canvas Width: 960
- Default Canvas Height: 540
- Run In Background: true

**Other Settings:**
- Color Space: Gamma
- Auto Graphics API: true
- Strip Engine Code: false (for debugging)

**Publishing Settings:**
- Compression Format: Disabled (for Yandex)
- Data Caching: false
- Decompression Fallback: true

**Memory:**
- Initial Memory Size: 256 (MB)
- Memory Growth Mode: Geometric
- Maximum Memory Size: 512

### Select WebGL Template:
1. Player Settings > Resolution and Presentation
2. WebGL Template: YandexGames

---

## Input Setup

The game uses:
- Left Mouse Button (GetMouseButtonDown(0))
- Space Key (GetKeyDown(KeyCode.Space))
- Touch Input (Input.GetTouch(0))

No additional Input Manager setup needed.

---

## Quality Settings

For WebGL (Edit > Project Settings > Quality):
1. Create "WebGL" quality level
2. Set as default for WebGL
3. Settings:
   - V Sync Count: Don't Sync
   - Anti Aliasing: Disabled
   - Texture Quality: Half Res
   - Shadows: Disable
   - Shadow Resolution: Low

---

## Physics 2D Settings

Edit > Project Settings > Physics 2D:
- Gravity: (0, -20)
- Default Contact Offset: 0.01
- Velocity Iterations: 8
- Position Iterations: 3

### Layer Collision Matrix:
- Player collides with: Ground, Obstacle
- Obstacle collides with: Player
- Ground collides with: Player

---

## Assembly Definition (Optional)

Create Assembly Definitions for better compilation:
```
Assets/Scripts/TapOrDie.asmdef
- References: Unity.TextMeshPro
```

---

## Testing Checklist

Before building, verify:
- [ ] Player jumps on tap/click/space
- [ ] Obstacles spawn and move left
- [ ] Collision with obstacles triggers game over
- [ ] Score increases every second
- [ ] Speed increases every 10 seconds
- [ ] Pause button works
- [ ] Music toggle works
- [ ] High score saves to PlayerPrefs
- [ ] Game restarts properly
- [ ] Main menu loads correctly
- [ ] All UI elements display correctly

---

## Common Issues

**Player falls through ground:**
- Check Ground layer is assigned
- Check Player has CircleCollider2D
- Check Rigidbody2D collision detection

**Obstacles don't trigger game over:**
- Check Obstacle tag is applied
- Check collider "Is Trigger" is enabled
- Check Player has trigger collision method

**Score doesn't update:**
- Check UIManager is subscribed to GameManager events
- Check TextMeshPro components are assigned

**Ads don't show:**
- Ads only work on Yandex.Games platform
- Check console for SDK initialization errors
- Verify YandexSDK.jslib is in Plugins/WebGL folder
