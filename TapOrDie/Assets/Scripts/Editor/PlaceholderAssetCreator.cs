#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;

namespace TapOrDie.Editor
{
    /// <summary>
    /// Editor utility to create placeholder sprites and audio clips
    /// </summary>
    public class PlaceholderAssetCreator : EditorWindow
    {
        [MenuItem("TapOrDie/Create Placeholder Assets")]
        public static void CreatePlaceholderAssets()
        {
            CreateSprites();
            CreateAudioPlaceholders();
            AssetDatabase.Refresh();
            Debug.Log("Placeholder assets created successfully!");
        }

        private static void CreateSprites()
        {
            string spritePath = "Assets/Sprites";
            if (!Directory.Exists(spritePath))
                Directory.CreateDirectory(spritePath);

            // Player sprite (circle)
            CreateCircleSprite(spritePath + "/Player.png", 64, new Color(0.4f, 0.8f, 1f));

            // Obstacle sprites
            CreateSquareSprite(spritePath + "/Obstacle_Block.png", 64, new Color(0.91f, 0.27f, 0.37f));
            CreateTriangleSprite(spritePath + "/Obstacle_Spike.png", 64, new Color(0.91f, 0.27f, 0.37f));
            CreateRectSprite(spritePath + "/Obstacle_Tall.png", 32, 96, new Color(0.91f, 0.27f, 0.37f));

            // Ground sprite
            CreateSquareSprite(spritePath + "/Ground.png", 64, new Color(0.2f, 0.2f, 0.3f));

            // Background sprites
            CreateSquareSprite(spritePath + "/Background_Layer1.png", 256, new Color(0.1f, 0.1f, 0.18f));
            CreateSquareSprite(spritePath + "/Background_Layer2.png", 256, new Color(0.08f, 0.12f, 0.22f));

            // UI icons
            CreateCircleSprite(spritePath + "/Icon_Pause.png", 32, Color.white);
            CreateCircleSprite(spritePath + "/Icon_Music_On.png", 32, Color.white);
            CreateCircleSprite(spritePath + "/Icon_Music_Off.png", 32, new Color(1f, 1f, 1f, 0.5f));
        }

        private static void CreateCircleSprite(string path, int size, Color color)
        {
            Texture2D texture = new Texture2D(size, size);
            Color[] pixels = new Color[size * size];

            float center = size / 2f;
            float radius = size / 2f - 2f;

            for (int y = 0; y < size; y++)
            {
                for (int x = 0; x < size; x++)
                {
                    float distance = Vector2.Distance(new Vector2(x, y), new Vector2(center, center));
                    if (distance < radius)
                    {
                        // Add slight gradient for depth
                        float gradient = 1f - (distance / radius) * 0.3f;
                        pixels[y * size + x] = new Color(
                            color.r * gradient,
                            color.g * gradient,
                            color.b * gradient,
                            1f
                        );
                    }
                    else if (distance < radius + 1f)
                    {
                        // Anti-aliased edge
                        float alpha = 1f - (distance - radius);
                        pixels[y * size + x] = new Color(color.r, color.g, color.b, alpha);
                    }
                    else
                    {
                        pixels[y * size + x] = Color.clear;
                    }
                }
            }

            texture.SetPixels(pixels);
            texture.Apply();

            byte[] bytes = texture.EncodeToPNG();
            File.WriteAllBytes(path, bytes);
            Object.DestroyImmediate(texture);
        }

        private static void CreateSquareSprite(string path, int size, Color color)
        {
            Texture2D texture = new Texture2D(size, size);
            Color[] pixels = new Color[size * size];

            for (int i = 0; i < pixels.Length; i++)
            {
                pixels[i] = color;
            }

            // Add border
            for (int x = 0; x < size; x++)
            {
                pixels[x] = color * 0.7f; // Top
                pixels[(size - 1) * size + x] = color * 0.7f; // Bottom
            }
            for (int y = 0; y < size; y++)
            {
                pixels[y * size] = color * 0.7f; // Left
                pixels[y * size + size - 1] = color * 0.7f; // Right
            }

            texture.SetPixels(pixels);
            texture.Apply();

            byte[] bytes = texture.EncodeToPNG();
            File.WriteAllBytes(path, bytes);
            Object.DestroyImmediate(texture);
        }

        private static void CreateRectSprite(string path, int width, int height, Color color)
        {
            Texture2D texture = new Texture2D(width, height);
            Color[] pixels = new Color[width * height];

            for (int i = 0; i < pixels.Length; i++)
            {
                pixels[i] = color;
            }

            texture.SetPixels(pixels);
            texture.Apply();

            byte[] bytes = texture.EncodeToPNG();
            File.WriteAllBytes(path, bytes);
            Object.DestroyImmediate(texture);
        }

        private static void CreateTriangleSprite(string path, int size, Color color)
        {
            Texture2D texture = new Texture2D(size, size);
            Color[] pixels = new Color[size * size];

            // Initialize with transparent
            for (int i = 0; i < pixels.Length; i++)
            {
                pixels[i] = Color.clear;
            }

            // Draw triangle pointing up
            for (int y = 0; y < size; y++)
            {
                float progress = (float)y / size;
                int halfWidth = Mathf.RoundToInt(progress * size / 2f);
                int centerX = size / 2;

                for (int x = centerX - halfWidth; x <= centerX + halfWidth; x++)
                {
                    if (x >= 0 && x < size)
                    {
                        pixels[y * size + x] = color;
                    }
                }
            }

            texture.SetPixels(pixels);
            texture.Apply();

            byte[] bytes = texture.EncodeToPNG();
            File.WriteAllBytes(path, bytes);
            Object.DestroyImmediate(texture);
        }

        private static void CreateAudioPlaceholders()
        {
            string audioPath = "Assets/Audio";
            string sfxPath = audioPath + "/SFX";
            string musicPath = audioPath + "/Music";

            if (!Directory.Exists(sfxPath))
                Directory.CreateDirectory(sfxPath);
            if (!Directory.Exists(musicPath))
                Directory.CreateDirectory(musicPath);

            // Create placeholder text files explaining what audio is needed
            File.WriteAllText(sfxPath + "/README.txt",
                "Place your sound effects here:\n" +
                "- Jump.wav (short jump sound)\n" +
                "- Hit.wav (collision sound)\n" +
                "- GameOver.wav (game over jingle)\n" +
                "- Button.wav (UI click sound)\n" +
                "- Collect.wav (item pickup sound)\n" +
                "- SpeedUp.wav (speed increase notification)\n\n" +
                "Recommended format: WAV, 44.1kHz, 16-bit\n" +
                "Keep files under 100KB each for WebGL");

            File.WriteAllText(musicPath + "/README.txt",
                "Place your music here:\n" +
                "- MenuMusic.mp3 (looping menu background music)\n" +
                "- GameMusic.mp3 (looping gameplay music)\n\n" +
                "Recommended format: MP3, 128kbps\n" +
                "Keep files under 2MB each for WebGL");
        }

        [MenuItem("TapOrDie/Setup Project")]
        public static void SetupProject()
        {
            // Create Tags
            AddTag("Player");
            AddTag("Obstacle");
            AddTag("Ground");
            AddTag("Collectible");

            // Create Layers
            AddLayer(6, "Ground");
            AddLayer(7, "Player");
            AddLayer(8, "Obstacle");

            Debug.Log("Project setup complete! Tags and layers created.");
        }

        private static void AddTag(string tag)
        {
            SerializedObject tagManager = new SerializedObject(
                AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]
            );
            SerializedProperty tagsProp = tagManager.FindProperty("tags");

            // Check if tag already exists
            for (int i = 0; i < tagsProp.arraySize; i++)
            {
                if (tagsProp.GetArrayElementAtIndex(i).stringValue == tag)
                    return;
            }

            // Add new tag
            tagsProp.InsertArrayElementAtIndex(tagsProp.arraySize);
            tagsProp.GetArrayElementAtIndex(tagsProp.arraySize - 1).stringValue = tag;
            tagManager.ApplyModifiedProperties();
        }

        private static void AddLayer(int layerIndex, string layerName)
        {
            SerializedObject tagManager = new SerializedObject(
                AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]
            );
            SerializedProperty layersProp = tagManager.FindProperty("layers");

            if (layerIndex >= layersProp.arraySize)
                return;

            SerializedProperty layerProp = layersProp.GetArrayElementAtIndex(layerIndex);
            if (string.IsNullOrEmpty(layerProp.stringValue))
            {
                layerProp.stringValue = layerName;
                tagManager.ApplyModifiedProperties();
            }
        }
    }
}
#endif
