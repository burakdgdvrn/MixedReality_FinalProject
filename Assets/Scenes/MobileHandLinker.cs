using UnityEngine;
using System.Collections.Generic;
using Mediapipe.Unity.Sample.HandLandmarkDetection;
using Mediapipe.Tasks.Vision.HandLandmarker;

public class MobileHandLinker : MonoBehaviour
{
    [Header("Temel Baðlantýlar")]
    public HandLandmarkerRunner handRunner;
    public Camera mainCamera;

    [Header("Derinlik Ayarlarý")]
    public float sensitivity = 50f;
    public float maxDistance = 30f;
    public float minDistance = 5f;

    [Header("Yumuþatma ve Ayna")]
    public float smoothSpeed = 10f;
    public bool mirrorX = true;

    private void Update()
    {
        // 1. GÜVENLÝK KONTROLÜ (DÜZELTÝLDÝ)
        if (handRunner == null) return;

        // HATALI SATIR SÝLÝNDÝ: "handRunner.latestResult == null" yok.
        // YERÝNE: Direkt içindeki listenin varlýðýna bakýyoruz.
        if (handRunner.latestResult.handLandmarks == null) return;

        var result = handRunner.latestResult;

        // El tespit edildi mi?
        if (result.handLandmarks.Count > 0)
        {
            // 2. ELÝ AL
            var currentHand = result.handLandmarks[0];

            // 3. NOKTALARI AL
            // "landmarks" eriþimi en güvenli yoldur.
            var wrist = currentHand.landmarks[0];
            var middleFinger = currentHand.landmarks[9];
            var indexTip = currentHand.landmarks[8];

            // --- HESAPLAMALAR ---

            // El Büyüklüðü (Derinlik için)
            float handSize = Vector2.Distance(new Vector2(wrist.x, wrist.y), new Vector2(middleFinger.x, middleFinger.y));

            // Z Pozisyonu (Ýleri Geri)
            float targetZ = handSize * sensitivity;
            targetZ = Mathf.Clamp(targetZ, minDistance, maxDistance);

            // X ve Y Pozisyonu
            float xPos = indexTip.x;
            float yPos = indexTip.y;

            if (mirrorX) xPos = 1 - xPos;
            yPos = 1 - yPos;

            // Hareket
            Vector3 targetPos = mainCamera.ViewportToWorldPoint(new Vector3(xPos, yPos, targetZ));
            transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * smoothSpeed);
        }
    }
}