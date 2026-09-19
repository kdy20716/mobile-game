using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace BlockBlast
{
    public class DraggableBlockUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public BlockShape Shape { get; private set; }
        public int SlotIndex { get; private set; }

        [Header("Drag Settings")]
        [Tooltip("Y-offset in screen pixels so finger doesn't cover the block on mobile")]
        [SerializeField] private float fingerOffsetY = 110f;
        [SerializeField] private float dragScale = 1.12f;

        private RectTransform _rectTransform;
        private Canvas _parentCanvas;
        private Transform _originalParent;
        private Vector2 _originalAnchoredPosition;
        private Vector3 _originalScale;
        private bool _isPlaced = false;

        public event Action<int> OnBlockPlaced; // slotIndex

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
            _originalScale = _rectTransform.localScale;
        }

        public void Init(BlockShape shape, int slotIdx, Canvas canvas, Sprite gemSprite, Sprite bombSprite)
        {
            Shape = shape;
            SlotIndex = slotIdx;
            _parentCanvas = canvas;
            _originalParent = transform.parent;
            _originalAnchoredPosition = _rectTransform.anchoredPosition;

            BuildVisuals(gemSprite, bombSprite);
        }

        public void RebuildVisuals(Sprite gemSprite, Sprite bombSprite)
        {
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }
            BuildVisuals(gemSprite, bombSprite);
        }

        private void BuildVisuals(Sprite gemSprite, Sprite bombSprite)
        {
            float cellSize = 36f;
            float spacing = 3f;

            float totalWidth = Shape.Cols * cellSize + (Shape.Cols - 1) * spacing;
            float totalHeight = Shape.Rows * cellSize + (Shape.Rows - 1) * spacing;
            _rectTransform.sizeDelta = new Vector2(totalWidth, totalHeight);

            float startX = -totalWidth * 0.5f + cellSize * 0.5f;
            float startY = totalHeight * 0.5f - cellSize * 0.5f;

            for (int r = 0; r < Shape.Rows; r++)
            {
                for (int c = 0; c < Shape.Cols; c++)
                {
                    if (Shape.matrix[r, c] == 1)
                    {
                        GameObject cellObj = new GameObject($"Cell_{r}_{c}", typeof(RectTransform), typeof(Image));
                        cellObj.transform.SetParent(transform, false);

                        RectTransform rt = cellObj.GetComponent<RectTransform>();
                        rt.sizeDelta = new Vector2(cellSize, cellSize);
                        rt.anchoredPosition = new Vector2(startX + c * (cellSize + spacing), startY - r * (cellSize + spacing));

                        Image img = cellObj.GetComponent<Image>();
                        img.color = Shape.blockColor;
                        if (gemSprite != null) img.sprite = gemSprite;

                        // Bomb Icon
                        if (Shape.isBomb && r == 0 && c == 0 && bombSprite != null)
                        {
                            GameObject bObj = new GameObject("BombIcon", typeof(RectTransform), typeof(Image));
                            bObj.transform.SetParent(cellObj.transform, false);
                            RectTransform bRt = bObj.GetComponent<RectTransform>();
                            bRt.sizeDelta = new Vector2(cellSize * 0.8f, cellSize * 0.8f);
                            bRt.anchoredPosition = Vector2.zero;
                            Image bImg = bObj.GetComponent<Image>();
                            bImg.sprite = bombSprite;
                        }
                    }
                }
            }
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (_isPlaced) return;

            if (BlockAudioManager.Instance != null)
            {
                BlockAudioManager.Instance.PlayPickup();
            }

            transform.SetParent(_parentCanvas.transform, true);
            transform.SetAsLastSibling();
            _rectTransform.localScale = _originalScale * dragScale;

            UpdateDragPosition(eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (_isPlaced) return;
            UpdateDragPosition(eventData);
            UpdateSnapPreview(eventData);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (_isPlaced) return;

            if (BlockGridManager.Instance != null)
            {
                BlockGridManager.Instance.ClearHighlights();
            }

            Vector2Int? gridPos = GetTargetGridPosition(eventData);
            bool placed = false;

            if (gridPos.HasValue && BlockGridManager.Instance != null)
            {
                if (BlockGridManager.Instance.CanPlaceShape(Shape, gridPos.Value.x, gridPos.Value.y))
                {
                    placed = BlockGridManager.Instance.PlaceShape(Shape, gridPos.Value.x, gridPos.Value.y);
                }
            }

            if (placed)
            {
                _isPlaced = true;
                if (BlockAudioManager.Instance != null)
                {
                    BlockAudioManager.Instance.PlayPlace();
                }

                OnBlockPlaced?.Invoke(SlotIndex);
                Destroy(gameObject);
            }
            else
            {
                StartCoroutine(ReturnToSlot());
            }
        }

        private void UpdateDragPosition(PointerEventData eventData)
        {
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _parentCanvas.transform as RectTransform,
                eventData.position + new Vector2(0f, fingerOffsetY),
                _parentCanvas.worldCamera,
                out localPoint
            );
            _rectTransform.anchoredPosition = localPoint;
        }

        private void UpdateSnapPreview(PointerEventData eventData)
        {
            if (BlockGridManager.Instance == null) return;

            Vector2Int? gridPos = GetTargetGridPosition(eventData);
            if (gridPos.HasValue)
            {
                bool canPlace = BlockGridManager.Instance.CanPlaceShape(Shape, gridPos.Value.x, gridPos.Value.y);
                BlockGridManager.Instance.HighlightPreview(Shape, gridPos.Value.x, gridPos.Value.y, canPlace);
            }
            else
            {
                BlockGridManager.Instance.ClearHighlights();
            }
        }

        private Vector2Int? GetTargetGridPosition(PointerEventData eventData)
        {
            if (BlockGridManager.Instance == null) return null;

            Vector2 dragWorldPos = _rectTransform.position;
            float minDistance = float.MaxValue;
            Vector2Int closest = Vector2Int.zero;
            bool foundAny = false;

            for (int r = 0; r < BlockGridManager.GridSize; r++)
            {
                for (int c = 0; c < BlockGridManager.GridSize; c++)
                {
                    BlockCellUI cell = BlockGridManager.Instance.GetCell(r, c);
                    if (cell != null)
                    {
                        float d = Vector2.Distance(dragWorldPos, cell.transform.position);
                        if (d < minDistance)
                        {
                            minDistance = d;
                            closest = new Vector2Int(r, c);
                            foundAny = true;
                        }
                    }
                }
            }

            // Cell spacing threshold (about 65 pixels in screen space)
            if (foundAny && minDistance < 120f)
            {
                // Align shape center with closest cell
                int startR = closest.x - Mathf.FloorToInt(Shape.Rows / 2f);
                int startC = closest.y - Mathf.FloorToInt(Shape.Cols / 2f);
                return new Vector2Int(startR, startC);
            }

            return null;
        }

        private IEnumerator ReturnToSlot()
        {
            float elapsed = 0f;
            float dur = 0.15f;
            Vector3 startPos = _rectTransform.position;
            Vector3 startScale = _rectTransform.localScale;

            transform.SetParent(_originalParent, true);
            Vector3 targetPos = _originalParent.position;

            while (elapsed < dur)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / dur;
                _rectTransform.position = Vector3.Lerp(startPos, targetPos, t);
                _rectTransform.localScale = Vector3.Lerp(startScale, _originalScale, t);
                yield return null;
            }

            _rectTransform.anchoredPosition = _originalAnchoredPosition;
            _rectTransform.localScale = _originalScale;
        }
    }
}
