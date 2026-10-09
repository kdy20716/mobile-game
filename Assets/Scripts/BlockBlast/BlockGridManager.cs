using System;
using System.Collections;
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
        [SerializeField] private Sprite starBombBlockSprite;

        public void SetupPastelBlockSprites(Sprite pink, Sprite mint, Sprite gold, Sprite purple, Sprite bomb)
        {
            pinkBlockSprite = pink;
            mintBlockSprite = mint;
            goldBlockSprite = gold;
            purpleBlockSprite = purple;
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
            if (h >= 0.09f && h <= 0.22f) return goldBlockSprite;
            if (h >= 0.25f && h < 0.68f) return mintBlockSprite;
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

            EnsureBlockSprites();
            InitializeGridCells();
        }

        private void EnsureBlockSprites()
        {
            if (pinkBlockSprite == null || pinkBlockSprite.name.Contains("Mascot"))
            {
#if UNITY_EDITOR
                pinkBlockSprite = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Textures/BlockBlastCute/Block_Pink.png");
                mintBlockSprite = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Textures/BlockBlastCute/Block_Mint.png");
                goldBlockSprite = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Textures/BlockBlastCute/Block_Gold.png");
                purpleBlockSprite = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Textures/BlockBlastCute/Block_Purple.png");
                starBombBlockSprite = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Textures/BlockBlastCute/Block_Star_Bomb.png");
#endif
            }
        }

        private void Start()
        {
            EnsureBlockSprites();
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
                            _cells[gr, gc].SetOccupied(Color.white, shape.isBomb, blockSp, null, shape.isSpecial, shape.mascotIndex, shape.isOneByOne);
                            blockCount++;
                        }
                    }
                }
            }

            OnScoreAdded?.Invoke(blockCount * 10);
            OnShapePlaced?.Invoke();

            // Special Block Activation upon Placement:
            if (shape.isSpecial)
            {
                // 1. Angel Mascot (idx 8)
                // Concept: "블록이 서로 닿으면 줄을 안채워도 줄이 터진다던가"
                if (shape.mascotIndex == 8)
                {
                    if (shape.isOneByOne)
                    {
                        StartCoroutine(AngelSpecialAllClearBlast(startR, startC));
                    }
                    else
                    {
                        CheckAngelTouchExplosion(shape, startR, startC);
                    }
                }
                // 2. Cloud Mascot (idx 7)
                // Concept: 2돌파 무지개 롱바가 가로 1줄을 즉시 싹쓸이 관통
                else if (shape.mascotIndex == 7 && !shape.isOneByOne)
                {
                    TriggerImmediateRowClear(startR);
                }
            }

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

                // Special Mascot Block Effects on Clear:
                foreach (var pos in cellsToClear)
                {
                    var cell = _cells[pos.x, pos.y];
                    if (cell != null && cell.IsSpecial)
                    {
                        // 1. Mint Mascot (idx 1): Timer increase! "특수블록이 터지면 시간이 늘어 난다거나"
                        if (cell.MascotIndex == 1)
                        {
                            float bonusSec = cell.IsOneByOne ? 10f : 5f;
                            if (BlockBlastUIManager.Instance != null)
                            {
                                BlockBlastUIManager.Instance.AddBonusTime(bonusSec);
                            }
                        }
                        // 2. Gold Mascot (idx 2): Gold bonus
                        else if (cell.MascotIndex == 2)
                        {
                            int bonusGold = cell.IsOneByOne ? 1000 : 500;
                            if (LobbyManager.Instance != null)
                            {
                                LobbyManager.Instance.AddCoins(bonusGold);
                            }
                        }
                        // 3. Pink Mascot (idx 0): Heart pop score bonus
                        else if (cell.MascotIndex == 0)
                        {
                            OnScoreAdded?.Invoke(cell.IsOneByOne ? 1500 : 800);
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
                // Gold Mascot Ability: Extra bonus score per cleared line (+100 pts per line, +25 per upgrade level)
                if (PlayerPrefs.GetInt("Selected_Mascot_Idx", 0) == 2)
                {
                    int lvl = LobbyManager.GetMascotLevel(2);
                    points += totalLines * (100 + (lvl - 1) * 25);
                }
                OnScoreAdded?.Invoke(points);
                OnLinesCleared?.Invoke(CurrentCombo, totalLines);
                OnFeverAdded?.Invoke(totalLines * 25f + (bombExploded ? 30f : 0f));
            }
            else
            {
                CurrentCombo = 0;
            }
        }

        public void ClearAllBlocksWithExplosion(Action onComplete = null)
        {
            StartCoroutine(ClearAllBlocksRoutine(onComplete));
        }

        private IEnumerator ClearAllBlocksRoutine(Action onComplete)
        {
            CurrentCombo++;
            List<BlockCellUI> cellsToClear = new List<BlockCellUI>();
            for (int r = 0; r < GridSize; r++)
            {
                for (int c = 0; c < GridSize; c++)
                {
                    if (_cells[r, c] != null && _cells[r, c].IsOccupied)
                    {
                        cellsToClear.Add(_cells[r, c]);
                    }
                }
            }

            float delay = 0f;
            foreach (var cell in cellsToClear)
            {
                cell.PlayClearAnim(delay);
                delay += 0.005f;
            }

            if (BlockAudioManager.Instance != null)
            {
                BlockAudioManager.Instance.PlayBomb();
                BlockAudioManager.Instance.PlayClear(Mathf.Max(3, CurrentCombo));
            }

            int points = Mathf.Max(100, cellsToClear.Count * 50);
            OnScoreAdded?.Invoke(points);
            OnFeverAdded?.Invoke(50f);

            yield return new WaitForSeconds(delay + 0.3f);

            for (int r = 0; r < GridSize; r++)
            {
                for (int c = 0; c < GridSize; c++)
                {
                    if (_cells[r, c] != null) _cells[r, c].SetEmpty();
                }
            }

            onComplete?.Invoke();
        }

        private void CheckAngelTouchExplosion(BlockShape shape, int startR, int startC)
        {
            HashSet<int> rowsToBlast = new HashSet<int>();
            HashSet<int> colsToBlast = new HashSet<int>();
            bool touchedSpecial = false;

            for (int r = 0; r < shape.Rows; r++)
            {
                for (int c = 0; c < shape.Cols; c++)
                {
                    if (shape.matrix[r, c] == 1)
                    {
                        int gr = startR + r;
                        int gc = startC + c;

                        int[] dr = new int[] { -1, 1, 0, 0 };
                        int[] dc = new int[] { 0, 0, -1, 1 };

                        for (int i = 0; i < 4; i++)
                        {
                            int nr = gr + dr[i];
                            int nc = gc + dc[i];

                            if (nr >= 0 && nr < GridSize && nc >= 0 && nc < GridSize)
                            {
                                bool isCurrentPlacement = (nr >= startR && nr < startR + shape.Rows && nc >= startC && nc < startC + shape.Cols && shape.matrix[nr - startR, nc - startC] == 1);
                                if (!isCurrentPlacement && _cells[nr, nc] != null && _cells[nr, nc].IsOccupied && _cells[nr, nc].IsSpecial)
                                {
                                    touchedSpecial = true;
                                    rowsToBlast.Add(gr);
                                    rowsToBlast.Add(nr);
                                    colsToBlast.Add(gc);
                                    colsToBlast.Add(nc);
                                }
                            }
                        }
                    }
                }
            }

            if (touchedSpecial)
            {
                TriggerCustomLineClears(rowsToBlast, colsToBlast);
            }
        }

        public void TriggerCustomLineClears(IEnumerable<int> rows, IEnumerable<int> cols)
        {
            HashSet<Vector2Int> cellsToClear = new HashSet<Vector2Int>();
            int lines = 0;
            foreach (int r in rows)
            {
                lines++;
                for (int c = 0; c < GridSize; c++) cellsToClear.Add(new Vector2Int(r, c));
            }
            foreach (int c in cols)
            {
                lines++;
                for (int r = 0; r < GridSize; r++) cellsToClear.Add(new Vector2Int(r, c));
            }

            if (cellsToClear.Count > 0)
            {
                CurrentCombo++;
                float delay = 0f;
                foreach (var pos in cellsToClear)
                {
                    if (_cells[pos.x, pos.y] != null && _cells[pos.x, pos.y].IsOccupied)
                    {
                        _cells[pos.x, pos.y].PlayClearAnim(delay);
                        delay += 0.015f;
                    }
                }
                if (BlockAudioManager.Instance != null)
                {
                    BlockAudioManager.Instance.PlayFairyMagic();
                    BlockAudioManager.Instance.PlayClear(CurrentCombo);
                }
                if (FairyScreenTransition.Instance != null)
                {
                    FairyScreenTransition.Instance.EmitCornerSparkles();
                }
                OnScoreAdded?.Invoke(cellsToClear.Count * 50 * CurrentCombo);
                OnLinesCleared?.Invoke(CurrentCombo, lines);
                OnFeverAdded?.Invoke(lines * 35f);
            }
        }

        public void TriggerImmediateRowClear(int row)
        {
            HashSet<int> rList = new HashSet<int>() { row };
            TriggerCustomLineClears(rList, new int[0]);
        }

        private IEnumerator AngelSpecialAllClearBlast(int r, int c)
        {
            yield return new WaitForSeconds(0.1f);
            HashSet<int> rList = new HashSet<int>() { r };
            HashSet<int> cList = new HashSet<int>() { c };
            TriggerCustomLineClears(rList, cList);
            yield return new WaitForSeconds(0.2f);
            ClearAllBlocksWithExplosion();
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
