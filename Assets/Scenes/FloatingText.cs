using UnityEngine;
using System.Collections; // Coroutine (zamanlý iþlemler) için gerekli

public class FloatingText : MonoBehaviour
{
    [Header("Ayarlar")]
    public float moveSpeed = 1f;    // Yukarý çýkma hýzý
    public float lifeTime = 2.5f;   // Ekranda kalma süresi
    public Vector3 finalScale = new Vector3(1f, 1f, 1f); // Ulaþacaðý son boyut

    private TextMesh textMesh;
    private Color initialColor;

    void Start()
    {
        textMesh = GetComponent<TextMesh>();
        if (textMesh == null)
        {
            Debug.LogError("HATA: FloatingText scripti bir TextMesh objesinde deðil!");
            Destroy(gameObject); // Hatalýysa hemen yok et
            return;
        }

        initialColor = textMesh.color;
        transform.localScale = Vector3.zero; // Baþlangýçta görünmez (boyut 0)

        // Animasyonu baþlat
        StartCoroutine(AnimateText());
    }

    IEnumerator AnimateText()
    {
        float timer = 0f;

        while (timer < lifeTime)
        {
            timer += Time.deltaTime;
            float progress = timer / lifeTime; // 0 ile 1 arasýnda ilerleme durumu

            // 1. SÜREKLÝ YUKARI HAREKET
            transform.Translate(Vector3.up * moveSpeed * Time.deltaTime);

            // 2. BÜYÜME EFEKTÝ (Pop-Up) - Ýlk %25'lik kýsýmda
            if (progress <= 0.25f)
            {
                // Elastik bir büyüme efekti için 'SmoothStep' veya 'Lerp' kullanýlabilir.
                // Burada hýzlýca büyüyüp biraz taþmasýný (overshoot) saðlýyoruz.
                float scaleProgress = progress / 0.25f;
                float scaleMultiplier = Mathf.Sin(scaleProgress * Mathf.PI * 0.5f) * 1.1f; // Biraz büyüt
                transform.localScale = Vector3.Lerp(Vector3.zero, finalScale * scaleMultiplier, scaleProgress);
            }
            else if (progress > 0.25f && progress <= 0.4f)
            {
                // Biraz küçülerek normal boyutuna dönsün (Bounce efekti)
                float bounceProgress = (progress - 0.25f) / 0.15f;
                transform.localScale = Vector3.Lerp(finalScale * 1.1f, finalScale, bounceProgress);
            }

            // 3. ÞEFFAFLAÞMA EFEKTÝ (Fade-Out) - Son %30'luk kýsýmda
            if (progress > 0.7f)
            {
                float fadeProgress = (progress - 0.7f) / 0.3f;
                // Rengin 'alpha' (þeffaflýk) deðerini 1'den 0'a indiriyoruz
                textMesh.color = new Color(initialColor.r, initialColor.g, initialColor.b, Mathf.Lerp(1f, 0f, fadeProgress));
            }

            yield return null; // Bir sonraki kareyi bekle
        }

        // Süre dolunca nesneyi tamamen yok et
        Destroy(gameObject);
    }
}