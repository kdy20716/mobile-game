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

            if (fillImage != null)
            {
                float elapsed = 0f;
                float dur = 0.22f;
                Vector3 origScale = fillImage.transform.localScale;

                while (elapsed < dur)
                {
                    elapsed += Time.deltaTime;
                    float t = elapsed / dur;
                    // Pop up then vanish
                    float s = Mathf.Lerp(1.25f, 0f, t);
                    fillImage.transform.localScale = origScale * s;
                    yield return null;
                }
            }

            SetEmpty();
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
