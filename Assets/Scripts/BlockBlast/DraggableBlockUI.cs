using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace BlockBlast
{
    public class DraggableBlockUI : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public BlockShape Shape { get; private set; }
        public int SlotIndex { get; private set; }

        [Header("Drag Settings")]
        [Tooltip("Y-offset in screen pixels so finger doesn't cover the block on mobile")]
        [SerializeField] private float fingerOffsetY = 110f;
        [SerializeField] private float dragScale = 1.15f;

        private RectTransform _rectTransform;
        private Canvas _parentCanvas;
        private Transform _originalParent;
        private Vector2 _originalAnchoredPosition;
        private Vector3 _originalScale;
        private bool _isPlaced = false;

        public event Action<int> OnBlockPlaced; // slotIndex

        private void Awake()
        {
            EnsureRectTransform();
        }

        private void EnsureRectTransform()
        {
            if (_rectTransform == null)
            {
                _rectTransform = GetComponent<RectTransform>();
                if (_rectTransform != null)
                {
                    _originalScale = _rectTransform.localScale;
                }
            }
        }

        public void Init(BlockShape shape, int slotIdx, Canvas canvas, Sprite gemSprite, Sprite bombSprite)
        {
            EnsureRectTransform();
            Shape = shape;
            SlotIndex = slotIdx;
            _parentCanvas = canvas;
            _originalParent = transform.parent;
            if (_rectTransform != null)
            {
                _originalAnchoredPosition = _rectTransform.anchoredPosition;
            }

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
            // Root hit-box to ensure entire shape boundary catches touches & clicks
            Image rootHitbox = GetComponent<Image>();
            if (rootHitbox == null) rootHitbox = gameObject.AddComponent<Image>();
            rootHitbox.color = Color.clear;
            rootHitbox.raycastTarget = true;

            float cellSize = 50f;
            float spacing = 4f;

            float totalWidth = Shape.Cols * cellSize + (Shape.Cols - 1) * spacing;
            float totalHeight = Shape.Rows * cellSize + (Shape.Rows - 1) * spacing;
            if (_rectTransform != null)
            {
                _rectTransform.sizeDelta = new Vector2(totalWidth, totalHeight);
            }

            float startX = -totalWidth * 0.5f + cellSize * 0.5f;
            float startY = totalHeight * 0.5f - cellSize * 0.5f;

            Sprite blockSp = null;
            if (BlockGridManager.Instance != null)
            {
                blockSp = BlockGridManager.Instance.GetBlockSpriteForColor(Shape.blockColor, Shape.isBomb)
                    ?? BlockGridManager.Instance.GetFaceSpriteForColor(Shape.blockColor, Shape.isBomb);
            }

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
                        img.sprite = blockSp ?? gemSprite;
                        img.color = Color.white;
                        img.raycastTarget = true; // Enabled for precise click detection!
                    }
                }
            }
        }

        private float ActiveOffsetY => (Application.isMobilePlatform || Input.touchCount > 0) ? fingerOffsetY : 20f;

        public void OnPointerDown(PointerEventData eventData)
        {
            if (_isPlaced) return;

            if (BlockAudioManager.Instance != null)
            {
                BlockAudioManager.Instance.PlayGrab();
            }

            transform.SetParent(_parentCanvas.transform, true);
            transform.SetAsLastSibling();

            StopAllCoroutines();
            StartCoroutine(PickupBounceAnim());

            UpdateDragPosition(eventData);
            UpdateSnapPreview(eventData);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (_isPlaced) return;
            UpdateDragPosition(eventData);
        }

        private IEnumerator PickupBounceAnim()
        {
            float elapsed = 0f;
            float dur = 0.18f;
            Vector3 targetScale = _originalScale * dragScale;

            while (elapsed < dur)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / dur;
                // Squash and stretch jelly bounce: Y stretches, X compresses, then settles
                float squashX = 1f - 0.12f * Mathf.Sin(t * Mathf.PI);
                float stretchY = 1f + 0.18f * Mathf.Sin(t * Mathf.PI);
                Vector3 curScale = Vector3.Lerp(_originalScale, targetScale, Mathf.SmoothStep(0f, 1f, t));
                _rectTransform.localScale = new Vector3(curScale.x * squashX, curScale.y * stretchY, curScale.z);
                yield return null;
            }

            _rectTransform.localScale = targetScale;
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
                    BlockAudioManager.Instance.PlayGrabDown();
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
                eventData.position + new Vector2(0f, ActiveOffsetY),
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

        // 100% Mathematically Precise Grid Snapping via Board Local Coordinates
        private Vector2Int? GetTargetGridPosition(PointerEventData eventData)
        {
            if (BlockGridManager.Instance == null || BlockGridManager.Instance.BoardRect == null) return null;

            Vector2 targetScreenPoint = eventData.position + new Vector2(0f, ActiveOffsetY);
            Vector2 localInBoard;

            // Map screen point directly into Board RectTransform local space
            if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
                BlockGridManager.Instance.BoardRect,
                targetScreenPoint,
                _parentCanvas.worldCamera,
                out localInBoard
            ))
            {
                Vector2Int? hitCoord = BlockGridManager.Instance.GetGridCoordFromLocalPoint(localInBoard);
                if (hitCoord.HasValue)
                {
                    // Center the shape around the pointed cell
                    int startR = hitCoord.Value.x - Mathf.FloorToInt(Shape.Rows / 2f);
                    int startC = hitCoord.Value.y - Mathf.FloorToInt(Shape.Cols / 2f);
                    return new Vector2Int(startR, startC);
                }
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
