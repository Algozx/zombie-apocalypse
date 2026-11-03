# Zombie Apocalypse

Top-down zombie shooter made in Unity. You pick a survivor, fight off the zombies, pick up medkits and try to survive until the boss shows up.

Play it in the browser: https://algozx.github.io/zombie-apocalypse/

## Controls

- Move: W A S D or the arrow keys
- Aim: mouse
- Shoot: hold the left mouse button
- Pause: P or Esc

On mobile or with a gamepad, the left stick moves and the right stick aims and shoots.

## About

This is an old project. I made it about 5 or 6 years ago while following a Unity course, and I'm keeping it as part of my portfolio. I didn't rewrite the gameplay code on purpose, so some of it (and some names and comments) is still in Portuguese.

I came back to it to make it run on current Unity:

- Updated to Unity 6
- Moved from the Built-in render pipeline to URP
- Moved from the old Input Manager to the Input System
- Added a WebGL build, hosted with GitHub Pages
- Fixed a few bugs (null references, medkits not healing, health bar not showing, slow bullets, invisible characters on the selection screen)

## Running the project

Open it with Unity 6000.6 or newer, open `Assets/_Project/_Scenes/Menu.unity` and press Play.

The WebGL build used by GitHub Pages is in the `docs` folder.
