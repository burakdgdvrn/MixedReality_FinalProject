using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FlagLogic : MonoBehaviour
{
    private bool isHeld = false;
    private bool isMatched = false;

    // YENİ: Panelin orijinal boyutunu hafızada tutacağız
    private Vector3 originalPanelScale;

    [Header("Ayarlar")]
    public string ulkeIsmi = "Turkey";

    [Header("Eğitici Mod (Bilgi Kartı)")]
    [TextArea] public string binaBilgisi;
    public GameObject infoPanel;
    public Text infoTextUI;

    [Header("Görsel Efektler")]
    public GameObject hitEffectPrefab;
    public GameObject floatingTextPrefab;

    void Start()
    {
        // OYUN BAŞLARKEN PANELİN SENİN AYARLADIĞIN BOYUTUNU KAYDET
        if (infoPanel != null)
        {
            originalPanelScale = infoPanel.transform.localScale;
            // Boyutu aldıktan sonra paneli gizle
            infoPanel.SetActive(false);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!isHeld && !isMatched && other.CompareTag("Player"))
        {
            if (other.transform.childCount > 0) return;
            GrabFlag(other.transform);
        }
        else if (isHeld && other.CompareTag("Landmark"))
        {
            string binaAdi = other.name.ToUpper();
            string arananIsim = ulkeIsmi.ToUpper();

            if (binaAdi.Contains(arananIsim))
            {
                MatchSuccess(other.transform);
            }
            else
            {
                StartCoroutine(WrongAnswerEffect());
            }
        }
    }

    void GrabFlag(Transform hand)
    {
        isHeld = true;
        transform.SetParent(hand);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        transform.localScale = Vector3.one;
    }

    IEnumerator WrongAnswerEffect()
    {
        Renderer rend = GetComponentInChildren<Renderer>();
        Color originalColor = Color.white;
        if (rend != null)
        {
            originalColor = rend.material.color;
            rend.material.color = Color.red;
        }

        Vector3 startPos = transform.localPosition;
        float timer = 0f;
        while (timer < 0.4f)
        {
            timer += Time.deltaTime;
            float x = Random.Range(-0.1f, 0.1f);
            float y = Random.Range(-0.1f, 0.1f);
            transform.localPosition = startPos + new Vector3(x, y, 0);
            yield return null;
        }

        transform.localPosition = startPos;
        if (rend != null) rend.material.color = originalColor;
    }

    void MatchSuccess(Transform building)
    {
        isHeld = false;
        isMatched = true;

        if (hitEffectPrefab != null) Instantiate(hitEffectPrefab, building.position + Vector3.up * 2f, Quaternion.identity);

        if (floatingTextPrefab != null)
        {
            Vector3 fixedOffset = new Vector3(0f, -1.2f, -0.5f);
            GameObject yazi = Instantiate(floatingTextPrefab, building.position + fixedOffset, Camera.main.transform.rotation);
            TextMesh tm = yazi.GetComponent<TextMesh>();
            if (tm != null)
            {
                tm.text = ulkeIsmi.ToUpper() + "!";
                tm.color = new Color(1f, 0.8f, 0f, 1f);
            }
        }

        transform.localScale = Vector3.zero;

        if (infoPanel != null && infoTextUI != null)
        {
            infoTextUI.text = binaBilgisi;
            StartCoroutine(AnimatePanelRoutine());
        }
        else
        {
            Destroy(gameObject);
        }
    }

    IEnumerator AnimatePanelRoutine()
    {
        infoPanel.SetActive(true);
        infoPanel.transform.localScale = Vector3.zero;

        // AÇILMA (Senin ayarladığın boyuta kadar)
        float timer = 0f;
        while (timer < 0.5f)
        {
            timer += Time.deltaTime;
            float progress = timer / 0.5f;
            float scale = Mathf.Sin(progress * Mathf.PI * 0.5f);

            // BURASI DEĞİŞTİ: Vector3.one yerine 'originalPanelScale' kullanıyoruz
            infoPanel.transform.localScale = originalPanelScale * scale;

            yield return null;
        }
        // Garanti olsun diye tam boyuta eşitle
        infoPanel.transform.localScale = originalPanelScale;

        yield return new WaitForSeconds(5f);

        // KAPANMA
        timer = 0f;
        while (timer < 0.5f)
        {
            timer += Time.deltaTime;
            float progress = timer / 0.5f;
            float scale = Mathf.Lerp(1f, 0f, progress);

            // BURASI DEĞİŞTİ
            infoPanel.transform.localScale = originalPanelScale * scale;

            yield return null;
        }

        infoPanel.transform.localScale = Vector3.zero;
        infoPanel.SetActive(false);

        Destroy(gameObject);
    }
}