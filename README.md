# Cute Memory Game

A matching-pairs memory card game for Android, built with Unity.

Flip cards on a grid, remember their faces, and match all pairs to win. Matching a pair plays a particle effect. The board is dealt after tapping Play and clears when every pair is found.

## Gameplay

- Tap **Play** to shuffle and deal a 4×4 board of 8 pairs
- Tap a card to flip it; tap a second card to try a match
- Matching pairs stay face-up and increment the matched-pairs counter
- Non-matching cards flip back after a short delay
- When all pairs are matched, the board clears and you can play again

Touch and mouse input are both supported, so you can play in the Unity editor or on a device.

## Requirements

- [Unity](https://unity.com/download) **5.5.2** (the version this project was built with) or a compatible later 5.x editor
- Android Build Support in Unity, if you want to install on a device

## Getting started

1. Clone this repository
2. Open the project folder in Unity (ignore a one-time reimport of the `Library` folder if it is missing)
3. Open `Assets/Scenes/scene1.unity`
4. Press **Play** in the editor, or use **File → Build Settings**, switch the platform to Android, and build

Generated folders such as `Library/`, `Temp/`, and `obj/` are created by Unity and are not part of the repository.

## Project layout

| Path | Contents |
| --- | --- |
| `Assets/Scripts/` | Game loop, card flip, and matching logic |
| `Assets/Scenes/` | Main play scene |
| `Assets/Sprites/` | Card front and back art |
| `Assets/Prefabs/` | Match particle effect and card quad |
| `Assets/Animations/` | Card animations |
| `Assets/Materials/` | Particle and effect materials |

## License

This project is licensed under the [MIT License](LICENSE).
