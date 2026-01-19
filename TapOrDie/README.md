# Tap or Die 🎮

A hyper-casual one-tap arcade game built with Unity for Yandex.Games platform.

**Marketing Description (RU):** Прыгай или умри! Аркада на один тап.
**Marketing Description (EN):** Jump or die! One-tap arcade fun.

**Tags/Keywords:** arcade, casual, one-tap, endless runner, jumping, reflex, hyper-casual, mobile-friendly

---

## 🎯 Game Features

- **Simple Controls:** One tap to jump
- **Endless Gameplay:** Survive as long as you can
- **Progressive Difficulty:** Speed increases every 10 seconds
- **Score System:** Points increase over time
- **High Score Tracking:** Local save with PlayerPrefs
- **Yandex.Games Integration:** Fullscreen ads, rewarded videos, sticky banners
- **Localization:** Russian and English support
- **Mobile Optimized:** Touch-friendly controls

---

## 📁 Project Structure

```
TapOrDie/
├── Assets/
│   ├── Scripts/
│   │   ├── Core/
│   │   │   ├── GameManager.cs        # Main game logic
│   │   │   ├── GameInitializer.cs    # Singleton initialization
│   │   │   ├── BackgroundScroller.cs # Parallax scrolling
│   │   │   └── Ground.cs             # Infinite ground
│   │   ├── Player/
│   │   │   └── PlayerController.cs   # Player movement & jump
│   │   ├── Obstacles/
│   │   │   ├── ObstacleManager.cs    # Object pooling & spawning
│   │   │   └── Obstacle.cs           # Individual obstacle behavior
│   │   ├── UI/
│   │   │   ├── UIManager.cs          # Main UI controller
│   │   │   ├── MainMenuUI.cs         # Main menu handler
│   │   │   ├── GameHUD.cs            # In-game HUD
│   │   │   ├── PauseUI.cs            # Pause menu
│   │   │   └── GameOverUI.cs         # Game over screen
│   │   ├── Audio/
│   │   │   └── AudioManager.cs       # Sound & music management
│   │   ├── Yandex/
│   │   │   └── YandexAdsManager.cs   # Yandex SDK integration
│   │   └── Localization/
│   │       ├── LocalizationManager.cs
│   │       └── LocalizedText.cs
│   ├── Plugins/
│   │   └── WebGL/
│   │       └── YandexSDK.jslib       # JavaScript bridge
│   ├── WebGLTemplates/
│   │   └── YandexGames/
│   │       └── index.html            # Custom WebGL template
│   ├── Scenes/
│   ├── Prefabs/
│   ├── Sprites/
│   └── Audio/
├── ProjectSettings/
├── SCENE_SETUP.md                    # Detailed scene setup guide
└── README.md                         # This file
```

---

## 🚀 Quick Start

### Prerequisites
- Unity 2021.3 LTS or newer
- TextMeshPro package (usually included)

### Setup Steps

1. **Create Unity Project**
   ```
   Unity Hub > New Project > 2D Core > Name: TapOrDie
   ```

2. **Import Scripts**
   - Copy all files from `Assets/` into your Unity project

3. **Create Tags**
   - Go to Edit > Project Settings > Tags and Layers
   - Add tags: `Player`, `Obstacle`, `Ground`, `Collectible`
   - Add layer: `Ground` (Layer 6)

4. **Setup Scenes**
   - Follow `SCENE_SETUP.md` for detailed instructions
   - Or import the example scenes if provided

5. **Assign Prefabs**
   - Create obstacle prefabs with `Obstacle` tag
   - Assign to ObstacleManager's `obstaclePrefabs` array

6. **Test in Editor**
   - Press Play in GameScene
   - Click/Space to jump

---

## 🔧 WebGL Build Settings

### Player Settings (Edit > Project Settings > Player > WebGL)

**Resolution:**
- Width: 960
- Height: 540
- Run In Background: ✓

**Publishing:**
- Compression Format: **Disabled** (required for Yandex)
- Data Caching: ✗
- Decompression Fallback: ✓

**Memory:**
- Initial Memory: 256 MB
- Maximum Memory: 512 MB

### Quality Settings
- Create "WebGL" profile
- Disable shadows
- Set texture quality to "Half Res"
- Disable V-Sync

### Select Template
- Player Settings > Resolution and Presentation
- WebGL Template: **YandexGames**

---

## 🎮 Yandex.Games SDK Integration

### Supported Features

| Feature | Method | When to Use |
|---------|--------|-------------|
| Fullscreen Ad | `ShowFullscreenAd()` | After game over |
| Rewarded Video | `ShowRewardedAd(callback)` | Extra life button |
| Sticky Banner | `ShowBanner()` / `HideBanner()` | Menu & pause |
| Game Ready | `NotifyGameReady()` | After loading |

### Important Rules for Moderation

1. **No ads during gameplay** - Only show ads at natural breaks
2. **Minimum 60s between fullscreen ads** - Enforced in code
3. **Rewarded ads must give real rewards** - Extra life implemented
4. **Hide banner during active gameplay** - Auto-handled

### SDK Initialization

The SDK is initialized automatically via `index.html`:

```javascript
YaGames.init().then(ysdk => {
    window.ysdk = ysdk;
    console.log('SDK initialized');
});
```

---

## 📱 Controls

| Input | Action |
|-------|--------|
| Left Click | Jump |
| Space | Jump |
| Touch | Jump |

---

## 🌐 Localization

Supported languages:
- English (EN) - default
- Russian (RU) - auto-detected from Yandex SDK

### Adding New Translations

Edit `LocalizationManager.cs`:

```csharp
translations["new_key"] = new Dictionary<Language, string>
{
    { Language.EN, "English text" },
    { Language.RU, "Русский текст" }
};
```

---

## 🔊 Audio Setup

### Required Audio Clips

Place in `Assets/Audio/`:

| Clip | Description |
|------|-------------|
| `MenuMusic.mp3` | Background music for menu |
| `GameMusic.mp3` | Background music for gameplay |
| `Jump.wav` | Jump sound effect |
| `Hit.wav` | Obstacle collision |
| `GameOver.wav` | Game over jingle |
| `Button.wav` | UI button click |
| `SpeedUp.wav` | Speed increase notification |

### Placeholder Audio

For testing, you can generate simple placeholder sounds or use silence. The game will work without audio files.

---

## 📦 Building for Yandex.Games

### Build Steps

1. **Configure Build Settings**
   ```
   File > Build Settings > WebGL > Switch Platform
   ```

2. **Set Player Settings** (as described above)

3. **Build**
   ```
   File > Build Settings > Build
   Select output folder: Build/
   ```

4. **Output Files**
   ```
   Build/
   ├── index.html
   ├── Build/
   │   ├── [ProjectName].data
   │   ├── [ProjectName].framework.js
   │   ├── [ProjectName].loader.js
   │   └── [ProjectName].wasm
   └── StreamingAssets/ (if used)
   ```

### Upload to Yandex.Games

1. Go to [Yandex.Games Developer Console](https://games.yandex.ru/console)
2. Create new game or select existing
3. Upload ZIP archive of Build folder
4. Fill in game info:
   - Title: "Tap or Die"
   - Description: Marketing description
   - Tags: arcade, casual, hyper-casual
   - Screenshots: At least 3
   - Icon: 512x512 PNG
5. Submit for moderation

---

## 🐛 Debugging

### Console Logs

Open browser DevTools (F12) to see:
- SDK initialization status
- Ad callbacks
- Unity messages

### Common Issues

| Issue | Solution |
|-------|----------|
| Ads not showing | Ads only work on yandex.ru domain |
| Game freezes on ad | Check Time.timeScale is restored |
| Build too large | Enable code stripping, reduce textures |
| Touch not working | Verify canvas touch-action CSS |

### Local Testing

Ads won't work locally. To test:
1. Build WebGL
2. Upload to Yandex draft
3. Test via preview link

---

## 📋 Moderation Checklist

Before submitting:

- [ ] Game loads under 10 seconds
- [ ] No console errors
- [ ] Fullscreen ad shows after game over
- [ ] Rewarded video gives actual reward
- [ ] No ads during active gameplay
- [ ] Game pauses when tab loses focus
- [ ] Works on mobile browsers
- [ ] RU and EN languages work
- [ ] High score persists
- [ ] All buttons are responsive

---

## 📄 License

This project is provided as-is for educational purposes.

---

## 🙏 Credits

- Unity Engine
- Yandex.Games SDK
- TextMeshPro

---

## 📞 Support

For issues with:
- **Yandex SDK:** Check [Yandex.Games Documentation](https://yandex.ru/dev/games/doc/dg/concepts/about.html)
- **Unity WebGL:** Check [Unity WebGL Documentation](https://docs.unity3d.com/Manual/webgl.html)

---

Made with ❤️ for Yandex.Games
