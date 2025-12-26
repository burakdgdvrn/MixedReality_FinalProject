# Touchless Interaction System: Global Explorer (Mixed Reality)

**Course:** Mixed Reality Applications (Fall 2025)  
**Student:** Burak Dağdeviren  
**Scenario:** Scenario 1 - Global Explorer (Flag & Landmark Matching)

## 📖 Project Overview
This project is a mobile Mixed Reality application developed using **Unity** and **MediaPipe**. The objective is to create a "Touchless Interaction System" where users interact with 3D digital content using physical hand gestures captured by a standard smartphone camera.

Users must identify countries, physically "grab" their floating flags using hand tracking, and drag them to the correct landmark (e.g., matching the Turkish Flag to the Galata Tower).

## 🛠 Technical Architecture & Custom Scripts

This project relies on a custom-built pipeline that bridges MediaPipe's 2D Landmark detection and Unity's 3D Physics system. This interaction is achieved through two distinct logical layers:

### 1. The Input Layer: `Hand2D` Logic (Raw Data)
This logical layer interacts directly with the **MediaPipe Hand Landmarker** to extract raw data before game processing.
* **Data Extraction:** It accesses the `HandLandmarkerRunner` result to retrieve raw landmark data.
* **Targeting:** Specifically filters for **Landmark 8 (Index Finger Tip)**, ignoring the other 20 points to optimize performance.
* **Normalization:** Handles the raw normalized coordinates (0 to 1) provided by the AI model before passing them to the game engine.

### 2. The 3D Bridge: `MobileHandLinker.cs` (Interaction Controller)
This script acts as the "Bridge" that converts the 2D data from the input layer into 3D physical movement in the Unity scene.
* **Bridge Logic:** To prevent crashes, strict **Null Checks** (`handLandmarks == null`) were implemented for data safety.
* **Coordinate Mapping:** Uses `Camera.ViewportToWorldPoint` to project the 2D screen coordinates into 3D World Space.
* **Mirroring:** The X-axis is inverted (`1 - x`) to create a mirror effect, ensuring that when the user moves their hand right, the virtual cursor also moves right.
* **Dynamic Depth:** The Z-depth of the cursor is calculated based on the hand size or a fixed sensitivity value to allow interaction at different distances.

### 3. The Game Logic: `FlagLogic.cs`
This script manages the state machine of the interactive objects (Flags).
* **State Machine:** Tracks whether the flag is `isHeld` (Grabbed) or `isMatched` (Placed).
* **Grabbing Mechanic:** Uses `OnTriggerEnter` with tag detection (`Player`). It includes a logic check (`transform.childCount`) to ensure the user cannot hold two flags simultaneously.
* **Matching Algorithm:** When the flag touches a building (`Landmark`), the script compares the `ulkeIsmi` string with the building's name using `.Contains()`.
* **Feedback System:**
    * **Success:** Triggers a particle effect, snaps the flag to the building, and activates an **Educational Info Panel**.
    * **Fail:** Triggers a `WrongAnswerEffect` coroutine that shakes the object and flashes it red.
* **UI Animation:** Implements a custom Coroutine `AnimatePanelRoutine` to smoothly scale the info panel up and down using sine waves (`Mathf.Sin`), storing the `originalPanelScale` to avoid scaling issues.

### 4. Visual Polish: `FloatingText.cs` & `Rotator.cs`
* **FloatingText.cs:** A polish script that creates a "Pop-up" text effect when a match is successful. It uses linear interpolation (`Mathf.Lerp`) for a bounce effect and fades out the text alpha over time.
* **Rotator.cs:** Adds a continuous rotation to floating objects to make them visually distinct from static landmarks.

## ⚙️ Optimization & Settings
To ensure the application runs smoothly on mobile devices:
1.  **Physics:** "Negative Scale" collider issues were resolved by separating visual meshes from collider parents.
2.  **Graphics API:** Forced **OpenGLES3** (Vulkan disabled) to prevent black/grey screen issues on Android builds.
3.  **Architecture:** Built using **IL2CPP** and **ARM64** to support the heavy computation required by the MediaPipe ML models.

## 📱 How to Play
1.  **Start:** Launch the app and grant camera permissions.
2.  **Detect:** Show your hand. A Yellow Sphere (Cursor) follows your index finger.
3.  **Grab:** Move the cursor to a floating flag. It will snap to your hand.
4.  **Match:** Drag the flag to the corresponding 3D Landmark.
    * *Correct:* The flag snaps to the building, and an info card appears.
    * *Incorrect:* The flag shakes and turns red.

---
*Developed for Mixed Reality Applications Final Assignment.*
