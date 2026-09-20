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

        [Header("Cute Face Sprites")]
        [SerializeField] private Sprite pinkMascotSprite;
        [SerializeField] private Sprite mintMascotSprite;
        [SerializeField] private Sprite crownSprite;
        [SerializeField] private Sprite diceSprite;
        [SerializeField] private Sprite starBombSprite;

        [Header("Pastel Block Sprites (With Mascots Embedded)")]
        [SerializeField] private Sprite pinkBlockSprite;
        [SerializeField] private Sprite mintBlockSprite;
        [SerializeField] private Sprite goldBlockSprite;
        [SerializeField] private Sprite purpleBlockSprite;
        [SerializeField] private Sprite blueBlockSprite;
        [SerializeField] private Sprite starBombBlockSprite;

        public void SetupPastelBlockSprites(Sprite pink, Sprite mint, Sprite gold, Sprite purple, Sprite blue, Sprite bomb)
        {
            pinkBlockSprite = pink;
            mintBlockSprite = mint;
            goldBlockSprite = gold;
            purpleBlockSprite = purple;
            blueBlockSprite = blue;
            starBombBlockSprite = bomb;

            pinkMascotSprite = pink;
            mintMascotSprite = mint;
            crownSprite = gold;
            diceSprite = purple;
            starBombSprite = bomb;
            bombIconSprite = bomb;
        }

        public Sprite GetBlockSpriteForColor(Color col, bool isBomb)
        {
            if (isBomb) return starBombBlockSprite ?? bombIconSprite;
            Color.RGBToHSV(col, out float h, out float s, out float v);
            if (h >= 0.88f || h <= 0.08f) return pinkBlockSprite;
            if (h >= 0.35f && h <= 0.58f) return mintBlockSprite;
            if (h >= 0.09f && h <= 0.22f) return goldBlockSprite;
            if (h > 0.50f && h < 0.68f) return blueBlockSprite;
            return purpleBlockSprite ?? pinkBlockSprite;
        }

        public void SetupFaceSprites(Sprite gem, Sprite pink, Sprite mint, Sprite crown, Sprite dice, Sprite bomb)
        {
            gemTileSprite = gem;
            pinkMascotSprite = pink;
            mintMascotSprite = mint;
            crownSprite = crown;
            diceSprite = dice;
            starBombSprite = bomb;
            bombIconSprite = bomb;
        }

        public Sprite GetFaceSpriteForColor(Color col, bool isBomb)
        {
            if (isBomb) return starBombSprite ?? bombIconSprite;
            Color.RGBToHSV(col, out float h, out float s, out float v);
            if (h >= 0.88f || h <= 0.08f) return pinkMascotSprite;
            if (h >= 0.35f && h <= 0.58f) return mintMascotSprite;
            if (h >= 0.09f && h <= 0.22f) return crownSprite;
            return diceSprite;
        }

        [Header("Board Metrics")]
        [SerializeField] private RectTransform boardRect;
        [SerializeField] private float cellSize = 108f;
        [SerializeField] private float cellSpacing = 10f;
        [SerializeField] private float startGridX = -385f;
        [SerializeField] private float startGridY = 385f;

        public RectTransform BoardRect => boardRect;
        public float CellSize => cellSize;
        public float CellSpacing => cellSpacing;
        public float StartGridX => startGridX;
        public float StartGridY => startGridY;

        public int CurrentCombo { get; private set; } = 0;

        public event Action<int, int> OnLinesCleared; // (combo, totalLines)
        public event Action<int> OnScoreAdded; // points
        public event Action<float> OnFeverAdded; // fever percentage
        public event Action OnBombExploded;
        public event Action OnShapePlaced;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);

            InitializeGridCells();
        }

        private void Start()
        {
            InitializeGridCells();
        }

        public void InitializeGridCells()
        {
            if (boardRect == null)
            {
                var bObj = GameObject.Find("BoardPanel") ?? GameObject.Find("BoardContainer") ?? GameObject.Find("Board");
                if (bObj != null) boardRect = bObj.GetComponent<RectTransform>();
            }

            BlockCellUI[] foundCells = GetComponentsInChildren<BlockCellUI>(true);
            if ((foundCells == null || foundCells.Length == 0) && boardRect != null)
            {
                foundCells = boardRect.GetComponentsInChildren<BlockCellUI>(true);
            }

            if (foundCells != null)
            {
                foreach (var cell in foundCells)
                {
                    if (cell != null && cell.Row >= 0 && cell.Row < GridSize && cell.Col >= 0 && cell.Col < GridSize)
                    {
                        _cells[cell.Row, cell.Col] = cell;
                    }
                }
            }
        }

        public void SetupBoardMetrics(RectTransform bRect, float cSize, float cSpacing, float startX, float startY)
        {
            boardRect = bRect;
            cellSize = cSize;
            cellSpacing = cSpacing;
            startGridX = startX;
            startGridY = startY;
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
            if (_cells[0, 0] == null) InitializeGridCells();

            for (int r = 0; r < shape.Rows; r++)
            {
                for (int c = 0; c < shape.Cols; c++)
                {
                    if (shape.matrix[r, c] == 1)
                    {
                        int gr = startR + r;
                        int gc = startC + c;

                        if (gr < 0 || gr >= GridSize || gc < 0 || gc >= GridSize) return false;
                        if (_cells[gr, gc] == null || _cells[gr, gc].IsOccupied) return false;
                    }
                }
            }
            return true;
        }

        public void HighlightPreview(BlockShape shape, int startR, int startC, bool active)
        {
            ClearHighlights();
            if (!active || shape == null) return;

            Sprite blockSp = GetBlockSpriteForColor(shape.blockColor, shape.isBomb) 
                ?? GetFaceSpriteForColor(shape.blockColor, shape.isBomb) 
                ?? gemTileSprite;
            Sprite faceSp = GetFaceSpriteForColor(shape.blockColor, shape.isBomb);

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
                            if (_cells[gr, gc] != null) _cells[gr, gc].SetHighlight(true, blockSp, faceSp, shape.blockColor);
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
            Sprite blockSp = GetBlockSpriteForColor(shape.blockColor, shape.isBomb) 
                ?? GetFaceSpriteForColor(shape.blockColor, shape.isBomb) 
                ?? gemTileSprite;

            for (int r = 0; r < shape.Rows; r++)
            {
                for (int c = 0; c < shape.Cols; c++)
                {
                    if (shape.matrix[r, c] == 1)
                    {
                        int gr = startR + r;
                        int gc = startC + c;

                        if (_cells[gr, gc] != null)
                        {
                            _cells[gr, gc].SetOccupied(Color.white, shape.isBomb, blockSp, null);
                            blockCount++;
                        }
                    }
                }
            }

            OnScoreAdded?.Invoke(blockCount * 10);
            OnShapePlaced?.Invoke();
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
