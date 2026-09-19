using System;
using System.Collections.Generic;
using UnityEngine;

namespace BlockBlast
{
    public class BlockGridManager : MonoBehaviour
    {
        public static BlockGridManager Instance { get; private set; }

        public const int GridSize = 8;
        private readonly BlockCellUI[,] _cells = new BlockCellUI[GridSize, GridSize];

        [Header("Sprites")]
        [SerializeField] private Sprite gemTileSprite;
        [SerializeField] private Sprite bombIconSprite;

        [Header("Board Metrics")]
        public RectTransform BoardRect { get; private set; }
        public float CellSize { get; private set; } = 108f;
        public float CellSpacing { get; private set; } = 10f;
        public float StartGridX { get; private set; }
        public float StartGridY { get; private set; }

        public int CurrentCombo { get; private set; } = 0;

        public event Action<int, int> OnLinesCleared; // (combo, totalLines)
        public event Action<int> OnScoreAdded; // points
        public event Action<float> OnFeverAdded; // fever percentage
        public event Action OnBombExploded;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        public void SetupBoardMetrics(RectTransform bRect, float cSize, float cSpacing, float startX, float startY)
        {
            BoardRect = bRect;
            CellSize = cSize;
            CellSpacing = cSpacing;
            StartGridX = startX;
            StartGridY = startY;
        }

        public Vector2Int? GetGridCoordFromLocalPoint(Vector2 localPoint)
        {
            float step = CellSize + CellSpacing;
            // Calculate column and row relative to startGridX and startGridY
            float relX = localPoint.x - (StartGridX - CellSize * 0.5f);
            float relY = (StartGridY + CellSize * 0.5f) - localPoint.y;

            if (relX < 0 || relY < 0) return null;

            int col = Mathf.FloorToInt(relX / step);
            int row = Mathf.FloorToInt(relY / step);

            if (row >= 0 && row < GridSize && col >= 0 && col < GridSize)
            {
                return new Vector2Int(row, col);
            }
            return null;
        }

        public void RegisterCell(int r, int c, BlockCellUI cell)
        {
            _cells[r, c] = cell;
        }

        public BlockCellUI GetCell(int r, int c)
        {
            if (r < 0 || r >= GridSize || c < 0 || c >= GridSize) return null;
            return _cells[r, c];
        }

        public bool CanPlaceShape(BlockShape shape, int startR, int startC)
        {
            for (int r = 0; r < shape.Rows; r++)
            {
                for (int c = 0; c < shape.Cols; c++)
                {
                    if (shape.matrix[r, c] == 1)
                    {
                        int gr = startR + r;
                        int gc = startC + c;

                        if (gr < 0 || gr >= GridSize || gc < 0 || gc >= GridSize) return false;
                        if (_cells[gr, gc] != null && _cells[gr, gc].IsOccupied) return false;
                    }
                }
            }
            return true;
        }

        public void HighlightPreview(BlockShape shape, int startR, int startC, bool active)
        {
            ClearHighlights();
            if (!active) return;

            for (int r = 0; r < shape.Rows; r++)
            {
                for (int c = 0; c < shape.Cols; c++)
                {
                    if (shape.matrix[r, c] == 1)
                    {
                        int gr = startR + r;
                        int gc = startC + c;

                        if (gr >= 0 && gr < GridSize && gc >= 0 && gc < GridSize)
                        {
                            if (_cells[gr, gc] != null) _cells[gr, gc].SetHighlight(true);
                        }
                    }
                }
            }
        }

        public void ClearHighlights()
        {
            for (int r = 0; r < GridSize; r++)
            {
                for (int c = 0; c < GridSize; c++)
                {
                    if (_cells[r, c] != null) _cells[r, c].SetHighlight(false);
                }
            }
        }

        public bool PlaceShape(BlockShape shape, int startR, int startC)
        {
            if (!CanPlaceShape(shape, startR, startC)) return false;

            int blockCount = 0;
            for (int r = 0; r < shape.Rows; r++)
            {
                for (int c = 0; c < shape.Cols; c++)
                {
                    if (shape.matrix[r, c] == 1)
                    {
                        int gr = startR + r;
                        int gc = startC + c;

                        _cells[gr, gc].SetOccupied(shape.blockColor, shape.isBomb, gemTileSprite, bombIconSprite);
                        blockCount++;
                    }
                }
            }

            OnScoreAdded?.Invoke(blockCount * 10);
            CheckLineClears();
            return true;
        }

        public void CheckLineClears()
        {
            List<int> fullRows = new List<int>();
            List<int> fullCols = new List<int>();

            // Check horizontal rows
            for (int r = 0; r < GridSize; r++)
            {
                bool rowFull = true;
                for (int c = 0; c < GridSize; c++)
                {
                    if (_cells[r, c] == null || !_cells[r, c].IsOccupied)
                    {
                        rowFull = false;
                        break;
                    }
                }
                if (rowFull) fullRows.Add(r);
            }

            // Check vertical columns
            for (int c = 0; c < GridSize; c++)
            {
                bool colFull = true;
                for (int r = 0; r < GridSize; r++)
                {
                    if (_cells[r, c] == null || !_cells[r, c].IsOccupied)
                    {
                        colFull = false;
                        break;
                    }
                }
                if (colFull) fullCols.Add(c);
            }

            int totalLines = fullRows.Count + fullCols.Count;
            if (totalLines > 0)
            {
                CurrentCombo++;
                HashSet<Vector2Int> cellsToClear = new HashSet<Vector2Int>();
                List<Vector2Int> bombPositions = new List<Vector2Int>();

                foreach (int r in fullRows)
                {
                    for (int c = 0; c < GridSize; c++)
                    {
                        cellsToClear.Add(new Vector2Int(r, c));
                        if (_cells[r, c].IsBomb) bombPositions.Add(new Vector2Int(r, c));
                    }
                }

                foreach (int c in fullCols)
                {
                    for (int r = 0; r < GridSize; r++)
                    {
                        cellsToClear.Add(new Vector2Int(r, c));
                        if (_cells[r, c].IsBomb) bombPositions.Add(new Vector2Int(r, c));
                    }
                }

                // 🧨 Elemental Bomb Blast: 3x3 Explosion!
                bool bombExploded = false;
                if (bombPositions.Count > 0)
                {
                    bombExploded = true;
                    OnBombExploded?.Invoke();

                    foreach (var bp in bombPositions)
                    {
                        for (int dr = -1; dr <= 1; dr++)
                        {
                            for (int dc = -1; dc <= 1; dc++)
                            {
                                int nr = bp.x + dr;
                                int nc = bp.y + dc;
                                if (nr >= 0 && nr < GridSize && nc >= 0 && nc < GridSize)
                                {
                                    cellsToClear.Add(new Vector2Int(nr, nc));
                                }
                            }
                        }
                    }
                }

                // Animate and clear
                float delay = 0f;
                foreach (var pos in cellsToClear)
                {
                    if (_cells[pos.x, pos.y] != null)
                    {
                        _cells[pos.x, pos.y].PlayClearAnim(delay);
                        delay += 0.015f; // wave cascade
                    }
                }

                if (BlockAudioManager.Instance != null)
                {
                    BlockAudioManager.Instance.PlayClear(CurrentCombo);
                    if (bombExploded) BlockAudioManager.Instance.PlayBomb();
                }

                int points = Mathf.RoundToInt((cellsToClear.Count * 20) * (CurrentCombo * 1.5f));
                OnScoreAdded?.Invoke(points);
                OnLinesCleared?.Invoke(CurrentCombo, totalLines);
                OnFeverAdded?.Invoke(totalLines * 25f + (bombExploded ? 30f : 0f));
            }
            else
            {
                CurrentCombo = 0;
            }
        }

        public void ResetBoard()
        {
            CurrentCombo = 0;
            for (int r = 0; r < GridSize; r++)
            {
                for (int c = 0; c < GridSize; c++)
                {
                    if (_cells[r, c] != null) _cells[r, c].SetEmpty();
                }
            }
        }

        public void SetSprites(Sprite gem, Sprite bomb)
        {
            gemTileSprite = gem;
            bombIconSprite = bomb;
        }
    }
}
