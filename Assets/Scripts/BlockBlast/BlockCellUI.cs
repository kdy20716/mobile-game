using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace BlockBlast
{
    public class BlockCellUI : MonoBehaviour
    {
        [SerializeField] private int row;
        [SerializeField] private int col;
        public int Row => row;
        public int Col => col;
        public bool IsOccupied { get; private set; } = false;
        public bool IsBomb { get; private set; } = false;
        public bool IsSpecial { get; private set; } = false;
        public int MascotIndex { get; private set; } = -1;
        public bool IsOneByOne { get; private set; } = false;

        [Header("UI References")]
        [SerializeField] private Image bgImage;
        [SerializeField] private Image fillImage;
        [SerializeField] private Image bombIcon;
        [SerializeField] private Image faceIcon;
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
            row = r;
            col = c;
            SetEmpty();
        }

        public void SetupComponents(Image bg, Image fill, Image bIcon, GameObject hl, Image face = null)
        {
            bgImage = bg;
            fillImage = fill;
            bombIcon = bIcon;
            highlightObj = hl;
            faceIcon = face;
        }

        public void SetOccupied(Color col, bool bomb, Sprite tileSprite = null, Sprite faceSprite = null, bool special = false, int mascotIdx = -1, bool oneByOne = false)
        {
            IsOccupied = true;
            IsBomb = bomb;
            IsSpecial = special;
            MascotIndex = mascotIdx;
            IsOneByOne = oneByOne;

            if (fillImage != null)
            {
                fillImage.gameObject.SetActive(true);
                fillImage.color = col;
                if (tileSprite != null) fillImage.sprite = tileSprite;
            }

            if (faceIcon != null)
            {
                if (faceSprite != null)
                {
                    faceIcon.gameObject.SetActive(true);
                    faceIcon.sprite = faceSprite;
                    faceIcon.color = Color.white;
                }
                else
                {
                    faceIcon.gameObject.SetActive(false);
                }
            }

            if (bombIcon != null)
            {
                bombIcon.gameObject.SetActive(bomb);
                if (bomb && faceSprite != null) bombIcon.sprite = faceSprite;
            }

            SetHighlight(false);
        }

        public void SetEmpty()
        {
            IsOccupied = false;
            IsBomb = false;
            IsSpecial = false;
            MascotIndex = -1;
            IsOneByOne = false;

            if (fillImage != null)
            {
                fillImage.gameObject.SetActive(false);
                fillImage.transform.localScale = Vector3.one;
            }

            if (faceIcon != null)
            {
                faceIcon.gameObject.SetActive(false);
            }

            if (bombIcon != null)
            {
                bombIcon.gameObject.SetActive(false);
            }

            SetHighlight(false);
        }

        public void SetHighlight(bool active, Sprite tileSprite = null, Sprite faceSprite = null, Color? color = null)
        {
            if (highlightObj != null)
            {
                highlightObj.SetActive(active);
                if (active)
                {
                    Image hlImg = highlightObj.GetComponent<Image>();
                    if (hlImg != null)
                    {
                        if (tileSprite != null) hlImg.sprite = tileSprite;
                        Color c = color ?? Color.white;
                        hlImg.color = new Color(c.r, c.g, c.b, 0.65f);
                    }

                    Transform faceChild = highlightObj.transform.Find("FaceHighlight");
                    if (faceChild != null)
                    {
                        Image faceImg = faceChild.GetComponent<Image>();
                        if (faceImg != null)
                        {
                            if (faceSprite != null)
                            {
                                faceChild.gameObject.SetActive(true);
                                faceImg.sprite = faceSprite;
                                faceImg.color = new Color(1f, 1f, 1f, 0.75f);
                            }
                            else
                            {
                                faceChild.gameObject.SetActive(false);
                            }
                        }
                    }
                }
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
