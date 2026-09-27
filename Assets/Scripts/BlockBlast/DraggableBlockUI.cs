using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace BlockBlast
{
    public class DraggableBlockUI : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerUpHandler
    {
        public static DraggableBlockUI CurrentlyDraggedBlock { get; private set; }

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
        private bool _isDragging = false;
        private bool _isDragCanceled = false;

        public event Action<int> OnBlockPlaced; // slotIndex

        private void Awake()
        {
            EnsureRectTransform();
        }

        private void OnDisable()
        {
            if (CurrentlyDraggedBlock == this)
            {
                CurrentlyDraggedBlock = null;
            }
            if (_isDragging && !_isPlaced)
            {
                ReturnToSlotImmediate();
            }
            if (BlockGridManager.Instance != null)
            {
                BlockGridManager.Instance.ClearHighlights();
            }
        }

        private void OnDestroy()
        {
            if (CurrentlyDraggedBlock == this)
            {
                CurrentlyDraggedBlock = null;
            }
            if (BlockGridManager.Instance != null)
            {
                BlockGridManager.Instance.ClearHighlights();
            }
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
            bool isDragging = (CurrentlyDraggedBlock == this);
            Vector2 savedPos = _rectTransform != null ? _rectTransform.anchoredPosition : Vector2.zero;

            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }
            BuildVisuals(gemSprite, bombSprite);

            if (isDragging && _rectTransform != null)
            {
                _rectTransform.anchoredPosition = savedPos;
                _rectTransform.localScale = Vector3.one * dragScale;
                UpdateSnapPreviewFromCurrentPointer();
                StopCoroutine("RotateJellyAnim");
                StartCoroutine("RotateJellyAnim");
            }
        }

        private IEnumerator RotateJellyAnim()
        {
            if (_rectTransform == null) yield break;
            float elapsed = 0f;
            float dur = 0.16f;
            Vector3 baseScale = Vector3.one * dragScale;
            while (elapsed < dur)
            {
                elapsed += Time.unscaledDeltaTime;
                float t = elapsed / dur;
                float s = 1f + Mathf.Sin(t * Mathf.PI) * 0.15f;
                _rectTransform.localScale = baseScale * s;
                yield return null;
            }
            _rectTransform.localScale = baseScale;
        }

        private void BuildVisuals(Sprite gemSprite, Sprite bombSprite)
        {
            // Root hit-box to ensure entire shape boundary catches touches & clicks
            Image rootHitbox = GetComponent<Image>();
            if (rootHitbox == null) rootHitbox = gameObject.AddComponent<Image>();
            rootHitbox.color = Color.clear;
            rootHitbox.raycastTarget = true;

            // Maximum allowable visual bounding box inside the slot card to prevent ANY protrusion on rotation
            float maxFitWidth = 220f;
            float maxFitHeight = 175f;

            float cellSize = 50f;
            float spacing = 4f;

            float totalWidth = Shape.Cols * cellSize + (Shape.Cols - 1) * spacing;
            float totalHeight = Shape.Rows * cellSize + (Shape.Rows - 1) * spacing;

            float fitScale = Mathf.Min(1f, Mathf.Min(maxFitWidth / totalWidth, maxFitHeight / totalHeight));

            if (_rectTransform != null)
            {
                _rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
                _rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
                _rectTransform.pivot = new Vector2(0.5f, 0.5f);
                _rectTransform.sizeDelta = new Vector2(totalWidth, totalHeight);

                _originalScale = Vector3.one * fitScale;
                _originalAnchoredPosition = Vector2.zero;

                if (CurrentlyDraggedBlock != this)
                {
                    _rectTransform.anchoredPosition = Vector2.zero;
                    _rectTransform.localScale = _originalScale;
                }
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
                        rt.anchorMin = new Vector2(0.5f, 0.5f);
                        rt.anchorMax = new Vector2(0.5f, 0.5f);
                        rt.pivot = new Vector2(0.5f, 0.5f);
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

        private float ActiveOffsetY
        {
            get
            {
#if ENABLE_INPUT_SYSTEM
                bool hasTouch = UnityEngine.InputSystem.Touchscreen.current != null && UnityEngine.InputSystem.Touchscreen.current.touches.Count > 0;
                return (Application.isMobilePlatform || hasTouch) ? fingerOffsetY : 20f;
#else
                return (Application.isMobilePlatform || Input.touchCount > 0) ? fingerOffsetY : 20f;
#endif
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (_isPlaced) return;
            if (Time.timeScale <= 0f || (BlockBlastUIManager.Instance != null && BlockBlastUIManager.Instance.IsPaused)) return;

            _isDragging = true;
            _isDragCanceled = false;
            CurrentlyDraggedBlock = this;

            if (BlockAudioManager.Instance != null)
            {
                BlockAudioManager.Instance.PlayGrab();
            }

            transform.SetParent(_parentCanvas.transform, true);
            transform.SetAsLastSibling();

            StopAllCoroutines();
            StartCoroutine(PickupBounceAnim());

            UpdateDragPosition(eventData.position);
            UpdateSnapPreviewAtPosition(eventData.position);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (_isPlaced) return;
            if (Time.timeScale <= 0f || (BlockBlastUIManager.Instance != null && BlockBlastUIManager.Instance.IsPaused))
            {
                CancelDrag(immediate: true);
                return;
            }

            _isDragging = true;
            _isDragCanceled = false;
            CurrentlyDraggedBlock = this;
            UpdateDragPosition(eventData.position);
        }

        private IEnumerator PickupBounceAnim()
        {
            float elapsed = 0f;
            float dur = 0.18f;
            Vector3 targetScale = Vector3.one * dragScale;

            while (elapsed < dur)
            {
                elapsed += Time.unscaledDeltaTime;
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
            if (_isPlaced || _isDragCanceled || !_isDragging) return;
            if (Time.timeScale <= 0f || (BlockBlastUIManager.Instance != null && BlockBlastUIManager.Instance.IsPaused))
            {
                CancelDrag(immediate: true);
                return;
            }

            UpdateDragPosition(eventData.position);
            UpdateSnapPreviewAtPosition(eventData.position);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (_isPlaced) return;

            // If drag was canceled or game is paused, NEVER place on board!
            if (_isDragCanceled || !_isDragging || Time.timeScale <= 0f || (BlockBlastUIManager.Instance != null && BlockBlastUIManager.Instance.IsPaused))
            {
                _isDragCanceled = false;
                _isDragging = false;
                if (CurrentlyDraggedBlock == this)
                {
                    CurrentlyDraggedBlock = null;
                }
                ReturnToSlotImmediate();
                return;
            }

            _isDragging = false;
            _isDragCanceled = false;

            if (CurrentlyDraggedBlock == this)
            {
                CurrentlyDraggedBlock = null;
            }

            if (BlockGridManager.Instance != null)
            {
                BlockGridManager.Instance.ClearHighlights();
            }

            Vector2Int? gridPos = GetTargetGridPosition(eventData.position);
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

        public void OnPointerUp(PointerEventData eventData)
        {
            if (_isDragCanceled || !_isDragging)
            {
                _isDragCanceled = false;
                _isDragging = false;
                if (CurrentlyDraggedBlock == this)
                {
                    CurrentlyDraggedBlock = null;
                }
                ReturnToSlotImmediate();
            }
        }

        public void ReturnToSlotImmediate()
        {
            StopAllCoroutines();
            if (_originalParent != null)
            {
                transform.SetParent(_originalParent, true);
            }
            if (_rectTransform != null)
            {
                _rectTransform.anchoredPosition = _originalAnchoredPosition;
                _rectTransform.localScale = _originalScale;
            }
            if (BlockGridManager.Instance != null)
            {
                BlockGridManager.Instance.ClearHighlights();
            }
            _isDragging = false;
            _isDragCanceled = false;
        }

        public void CancelDrag(bool immediate = false)
        {
            if (_isPlaced) return;

            _isDragCanceled = true;
            _isDragging = false;

            if (CurrentlyDraggedBlock == this)
            {
                CurrentlyDraggedBlock = null;
            }

            if (BlockGridManager.Instance != null)
            {
                BlockGridManager.Instance.ClearHighlights();
            }

            StopAllCoroutines();

            if (BlockAudioManager.Instance != null)
            {
                BlockAudioManager.Instance.PlayGrabDown();
            }

            if (immediate || Time.timeScale <= 0f)
            {
                ReturnToSlotImmediate();
            }
            else
            {
                StartCoroutine(ReturnToSlot());
            }
        }

        public void CancelDragImmediate()
        {
            CancelDrag(immediate: true);
        }

        public static void CancelAllActiveDrags(bool immediate = true)
        {
            if (CurrentlyDraggedBlock != null)
            {
                CurrentlyDraggedBlock.CancelDrag(immediate);
                CurrentlyDraggedBlock = null;
            }

            var allBlocks = UnityEngine.Object.FindObjectsOfType<DraggableBlockUI>();
            if (allBlocks != null)
            {
                for (int i = 0; i < allBlocks.Length; i++)
                {
                    if (allBlocks[i] != null && (allBlocks[i]._isDragging || allBlocks[i].transform.parent != allBlocks[i]._originalParent))
                    {
                        allBlocks[i].CancelDrag(immediate);
                    }
                }
            }

            if (BlockGridManager.Instance != null)
            {
                BlockGridManager.Instance.ClearHighlights();
            }
        }

        private void UpdateDragPosition(Vector2 screenPos)
        {
            if (_parentCanvas == null || _rectTransform == null) return;

            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _parentCanvas.transform as RectTransform,
                screenPos + new Vector2(0f, ActiveOffsetY),
                _parentCanvas.worldCamera,
                out localPoint
            );
            _rectTransform.anchoredPosition = localPoint;
        }

        public void UpdateSnapPreviewFromCurrentPointer()
        {
            Vector2 pos = GetCurrentPointerScreenPosition();
            UpdateSnapPreviewAtPosition(pos);
        }

        private void UpdateSnapPreviewAtPosition(Vector2 screenPos)
        {
            if (BlockGridManager.Instance == null) return;

            Vector2Int? gridPos = GetTargetGridPosition(screenPos);
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

        private Vector2 GetCurrentPointerScreenPosition()
        {
#if ENABLE_INPUT_SYSTEM
            if (UnityEngine.InputSystem.Touchscreen.current != null && UnityEngine.InputSystem.Touchscreen.current.primaryTouch.press.isPressed)
            {
                return UnityEngine.InputSystem.Touchscreen.current.primaryTouch.position.ReadValue();
            }
            if (UnityEngine.InputSystem.Mouse.current != null)
            {
                return UnityEngine.InputSystem.Mouse.current.position.ReadValue();
            }
#endif
            if (Input.touchCount > 0)
            {
                return Input.GetTouch(0).position;
            }
            return Input.mousePosition;
        }

        // 100% Mathematically Precise Grid Snapping via Board Local Coordinates
        private Vector2Int? GetTargetGridPosition(Vector2 screenPos)
        {
            if (BlockGridManager.Instance == null || BlockGridManager.Instance.BoardRect == null || _parentCanvas == null) return null;

            Vector2 targetScreenPoint = screenPos + new Vector2(0f, ActiveOffsetY);
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
                elapsed += Time.unscaledDeltaTime;
                float t = elapsed / dur;
                _rectTransform.position = Vector3.Lerp(startPos, targetPos, t);
                _rectTransform.localScale = Vector3.Lerp(startScale, _originalScale, t);
                yield return null;
            }

            _rectTransform.anchoredPosition = _originalAnchoredPosition;
            _rectTransform.localScale = _originalScale;
            _isDragging = false;
            _isDragCanceled = false;
        }
    }
}
