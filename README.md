# 🎓 Rapocalypse: GPA Saver

![Game Thumbnail](demo/demo3.png) 

**Rapocalypse: GPA Saver** is an action-packed 2D platformer and shooter developed in Unity. As the final apocalyptic grading cycle approaches, it's up to you to navigate treacherous terrain, battle relentless obstacles, and save students from a devastating GPA collapse! 

Your actions matter—with multiple endings depending on the number of students you can rescue, you'll need precise platforming, sharp combat skills, and strategic thinking to achieve the "True Ending."

---

## 🎥 Game Trailer
▶️ Watch the Trailer on YouTube: [https://www.youtube.com/watch?v=6rRu_UJ-KYo](https://www.youtube.com/watch?v=6rRu_UJ-KYo) <!-- Replace with your actual YouTube link -->

▶️ Watch the Gameplay on Twitch: [https://www.twitch.tv/videos/2772510131?t=08h05m35s](https://www.twitch.tv/videos/2772510131?t=08h05m35s)

![Gameplay Screenshot 1](demo/demo.png)
![Gameplay Screenshot 2](demo/demo2.png)

---

## 🎮 Gameplay Features

* **Dynamic 2D Platforming:** Master a fluid movement system complete with double jumps, dashing, and continuous momentum physics.
* **Intense Action & Combat:** Shoot your way through waves of dynamically spawning enemies (Chasers, Movers, and Flyers).
* **Defend Objectives:** Protect crucial characters like "Tony" during high-stakes potion-brewing survival waves (Level 2).
* **Multiple Endings:** Replayability is built-in! The ending you receive is directly tied to the number of students you manage to save during your run:
  * **Bad Ending:** $\le$ 5 students saved.
  * **Normal Ending:** 6 - 15 students saved.
  * **True Ending:** $\ge$ 16 students saved.
* **Cinematic Cutscenes:** Engaging opening and ending cinematics utilizing Unity's Timeline and subtitle integration.

---

## 🛠️ Technology Stack & Architecture

This project was built as part of **COMP3329 Computer Game Design and Programming**, utilizing modern Unity game dev tooling.

* **Engine:** Unity (2D Configuration)
* **Language:** C#
* **Physics:** Unity `Physics2D` (Rigidbodies, OverlapCircles for environment detection).
* **Animation & UI:** `Animator` controllers for dynamic character states (Jumping, Dashing, Hit/Win states) and canvas-based UIs for Wave Timers / Mana Bars.
* **Director / Cinematics:** Unity `Timeline` for smooth in-game cutscenes and event flows.
* **Audio:** Comprehensive Audio Manager triggering dedicated `AudioSource` nodes to allow simultaneous BGM and precise SFX (footsteps, shooting, dashing).

---

## 🚀 How to Import & Play

1. **Download:** Unzip the root project directory downloaded from the repository/Moodle.
2. **Setup:** Create a new 2D project in Unity (or Tuanjie).
3. **Overwrite:** Drag the `Assets`, `ProjectSettings`, and `Packages` folders into your new project, replacing the existing defaults.
4. **Launch:** Open the initial scene located in `Assets/Levels/OpeningScene.unity`.
5. **Play:** Press **Play** in the editor to start saving GPAs!

*Note: For a detailed breakdown of the game's mechanics, architecture, and design journey, please view our comprehensive [Game Report](COMP3329%20report.pdf).*

---

## 👥 Meet the Team (Group 24)

* **Chau Wai Yee**
* **Chung Ka Yi**
* **Hui Lok**
* **Lam Wui Yan**

> **Thank you for playing! Keep your dash ready, and save those GPAs!**
