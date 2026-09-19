using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace BlockBlast
{
    public class BlockCellUI : MonoBehaviour
    {
        public int Row { get; private set; }
        public int Col { get; private set; }
        public bool IsOccupied { get; private set; } = false;
        public bool IsBomb { get; private set; } = false;

        [Header("UI References")]
        [SerializeField] private Image bgImage;
        [SerializeField] private Image fillImage;
        [SerializeField] private Image bombIcon;
        [SerializeField] private GameObject highlightObj;

        private Button cellButton;
        public static event Action<int, int> OnCellClickedForHammer;

        private void Awake()
        {
            cellButton = GetComponent<Button>();
            if (cellButton != null)
            {
                cellButton.onClick.AddListener(() =>
                {
                    OnCellClickedForHammer?.Invoke(Row, Col);
                });
            }
        }

        public void Init(int r, int c)
        {
            Row = r;
            Col = c;
            SetEmpty();
        }

        public void SetOccupied(Color col, bool bomb, Sprite tileSprite = null, Sprite bombSprite = null)
        {
            IsOccupied = true;
            IsBomb = bomb;

            if (fillImage != null)
            {
                fillImage.gameObject.SetActive(true);
                fillImage.color = col;
                if (tileSprite != null) fillImage.sprite = tileSprite;
            }

            if (bombIcon != null)
            {
                bombIcon.gameObject.SetActive(bomb);
                if (bomb && bombSprite != null) bombIcon.sprite = bombSprite;
            }

            SetHighlight(false);
        }

        public void SetEmpty()
        {
            IsOccupied = false;
            IsBomb = false;

            if (fillImage != null)
            {
                fillImage.gameObject.SetActive(false);
                fillImage.transform.localScale = Vector3.one;
            }

            if (bombIcon != null)
            {
                bombIcon.gameObject.SetActive(false);
            }

            SetHighlight(false);
        }

        public void SetHighlight(bool active)
        {
            if (highlightObj != null)
            {
                highlightObj.SetActive(active);
            }
        }

        public void PlayClearAnim(float delay = 0f)
        {
            StartCoroutine(ClearAnimCoroutine(delay));
        }

        private IEnumerator ClearAnimCoroutine(float delay)
        {
            if (delay > 0f) yield return new WaitForSeconds(delay);

            if (fillImage != null && fillImage.gameObject.activeSelf)
            {
                Color origColor = fillImage.color;
                Vector3 origScale = fillImage.transform.localScale;
                float dur = 0.28f;
                float elapsed = 0f;

                // Spawn 4 cute jelly sparkle particles
                SpawnPopParticles(origColor);

                while (elapsed < dur)
                {
                    elapsed += Time.deltaTime;
                    float t = elapsed / dur;

                    float s;
                    if (t < 0.35f)
                    {
                        float p = t / 0.35f;
                        s = Mathf.Lerp(1.0f, 1.35f, p);
                        // Flash to sweet pastel white
                        fillImage.color = Color.Lerp(origColor, Color.white, p * 0.9f);
                    }
                    else
                    {
                        float p = (t - 0.35f) / 0.65f;
                        s = Mathf.Lerp(1.35f, 0f, p * p);
                        Color c = origColor;
                        c.a = Mathf.Lerp(1f, 0f, p);
                        fillImage.color = c;
                    }

                    fillImage.transform.localScale = origScale * s;
                    fillImage.transform.localRotation = Quaternion.Euler(0f, 0f, Mathf.Sin(t * Mathf.PI * 2f) * 12f);
                    yield return null;
                }

                fillImage.transform.localRotation = Quaternion.identity;
                fillImage.color = origColor;
            }

            SetEmpty();
        }

        private void SpawnPopParticles(Color color)
        {
            if (transform.parent == null) return;

            // 4 mini sparkles/droplets in 4 diagonal directions
            Vector2[] dirs = new Vector2[]
            {
                new Vector2(0.85f, 0.85f).normalized,
                new Vector2(-0.85f, 0.85f).normalized,
                new Vector2(-0.85f, -0.85f).normalized,
                new Vector2(0.85f, -0.85f).normalized,
            };

            for (int i = 0; i < dirs.Length; i++)
            {
                GameObject pObj = new GameObject($"Sparkle_{i}", typeof(RectTransform), typeof(Image));
                pObj.transform.SetParent(transform.parent, false);
                RectTransform rt = pObj.GetComponent<RectTransform>();
                rt.anchoredPosition = (transform as RectTransform).anchoredPosition;
                rt.sizeDelta = new Vector2(26f, 26f);

                Image img = pObj.GetComponent<Image>();
                img.sprite = fillImage != null ? fillImage.sprite : null;
                img.color = color;
                img.raycastTarget = false;

                StartCoroutine(ParticleFlyCoroutine(rt, img, dirs[i]));
            }
        }

        private IEnumerator ParticleFlyCoroutine(RectTransform pRt, Image pImg, Vector2 dir)
        {
            float elapsed = 0f;
            float dur = 0.32f;
            Vector2 startPos = pRt.anchoredPosition;
            float dist = UnityEngine.Random.Range(45f, 75f);
            Vector2 targetPos = startPos + dir * dist;

            while (elapsed < dur)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / dur;
                float moveT = Mathf.Sin(t * Mathf.PI * 0.5f);
                pRt.anchoredPosition = Vector2.Lerp(startPos, targetPos, moveT);
                pRt.localScale = Vector3.Lerp(Vector3.one * 1.1f, Vector3.zero, t * t);

                Color c = pImg.color;
                c.a = Mathf.Lerp(1f, 0f, t);
                pImg.color = c;

                yield return null;
            }

            Destroy(pRt.gameObject);
        }

        public void SetupComponents(Image bg, Image fill, Image bomb, GameObject hl)
        {
            bgImage = bg;
            fillImage = fill;
            bombIcon = bomb;
            highlightObj = hl;
        }
    }
}
