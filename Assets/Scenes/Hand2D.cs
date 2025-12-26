using UnityEngine;
using System.Collections.Generic;
using Mediapipe.Tasks.Vision.HandLandmarker;

// Namespace önemli, Runner scriptini görmesi için:
namespace Mediapipe.Unity.Sample.HandLandmarkDetection
{
    public class MobileHandLinker : MonoBehaviour
    {
        [Header("Baðlantýlar")]
        public HandLandmarkerRunner handRunner;
        public Camera mainCamera;

        [Header("2D Ayarlarý")]
        [Tooltip("Top kameradan ne kadar uzakta dursun? (Sabit Z mesafesi)")]
        public float distanceFromCamera = 10f;

        [Header("Hassasiyet")]
        public float smoothSpeed = 12f;
        public bool mirrorX = true;

        private void Update()
        {
            // 1. GÜVENLÝK KONTROLÜ
            if (handRunner == null) return;

            // Struct null olamaz hatasýný önleyen doðru kontrol:
            if (handRunner.latestResult.handLandmarks == null) return;

            var result = handRunner.latestResult;

            // El tespit edildi mi?
            if (result.handLandmarks.Count > 0)
            {
                // Ýlk eli al (Paket olarak gelir)
                var currentHand = result.handLandmarks[0];

                // --- DÜZELTME BURADA ---
                // currentHand bir paket olduðu için direkt [8] diyemiyoruz.
                // Ýçindeki .landmarks listesine girip öyle [8] diyoruz.

                var indexTip = currentHand.landmarks[8];
                // Eðer üstteki satýr yine de kýrmýzý yanarsa 's' yi silip .landmark[8] dene.
                // Ama genelde .landmarks (çoðul) doðrudur.

                float xPos = indexTip.x;
                float yPos = indexTip.y;

                // 3. EKRAN AYARLARI
                if (mirrorX) xPos = 1 - xPos;
                yPos = 1 - yPos; // Yön düzeltmesi

                // 4. HAREKET (Z SABÝT)
                // Z derinliði hep 'distanceFromCamera' kadar olacak.
                Vector3 targetPos = mainCamera.ViewportToWorldPoint(new Vector3(xPos, yPos, distanceFromCamera));

                // 5. YUMUÞATMA
                transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * smoothSpeed);
            }
        }
    }
}