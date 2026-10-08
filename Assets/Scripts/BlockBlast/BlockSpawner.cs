using System.Collections;
using UnityEngine;

namespace BlockBlast
{
    public class BlockSpawner : MonoBehaviour
    {
        public static BlockSpawner Instance { get; private set; }

        [Header("Slot Containers (3 Slots)")]
        [SerializeField] private Transform[] slotParents;
        private readonly DraggableBlockUI[] _activeBlocks = new DraggableBlockUI[3];

        [Header("Sprites")]
        [SerializeField] private Sprite gemTileSprite;
        [SerializeField] private Sprite bombIconSprite;
        [SerializeField] private Canvas mainCanvas;

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        private void Start()
        {
            if (slotParents != null && slotParents.Length > 0 && slotParents[0] != null && slotParents[0].gameObject.activeInHierarchy)
            {
                SpawnNewHand();
            }
        }

        public void SpawnNewHand()
        {
            ClearHand();

            // Purple Mascot Ability: 5% base chance (+1% per upgrade level) that all 3 spawned blocks become 2x2 Purple blocks!
            bool isPurpleMascot = PlayerPrefs.GetInt("Selected_Mascot_Idx", 0) == 3;
            float purpleRate = 0.05f;
            if (isPurpleMascot)
            {
                int lvl = LobbyManager.GetMascotLevel(3);
                purpleRate = 0.05f + (lvl - 1) * 0.01f;
            }
            bool purpleSpecialTriggered = isPurpleMascot && (Random.value < purpleRate);

            for (int i = 0; i < 3; i++)
            {
                if (slotParents != null && i < slotParents.Length && slotParents[i] != null)
                {
                    BlockShape shape;
                    if (purpleSpecialTriggered)
                    {
                        // 2x2 block with purple color (BlockShapeData.Palette[4])
                        int[,] m2x2 = new int[,] { { 1, 1 }, { 1, 1 } };
                        Color purpleCol = BlockShapeData.Palette[4]; // Lavender Purple
                        shape = new BlockShape(m2x2, purpleCol, false, "2x2_Purple_Magic");
                    }
                    else
                    {
                        shape = BlockShapeData.GetRandomShape(0f);
                    }

                    GameObject blockObj = new GameObject($"Block_{i}", typeof(RectTransform));
                    blockObj.transform.SetParent(slotParents[i], false);

                    DraggableBlockUI drag = blockObj.AddComponent<DraggableBlockUI>();
                    drag.Init(shape, i, mainCanvas, gemTileSprite, bombIconSprite);
                    drag.OnBlockPlaced += HandleBlockPlaced;

                    _activeBlocks[i] = drag;
                }
            }

            StartCoroutine(CheckGameOverDeferred());
        }

        private void HandleBlockPlaced(int slotIndex)
        {
            _activeBlocks[slotIndex] = null;

            bool allPlaced = true;
            for (int i = 0; i < 3; i++)
            {
                if (_activeBlocks[i] != null)
                {
                    allPlaced = false;
                    break;
                }
            }

            if (allPlaced)
            {
                SpawnNewHand();
            }
            else
            {
                StartCoroutine(CheckGameOverDeferred());
            }
        }

        // 🔄 90-degree Rotation
        public void RotateHandBlocks()
        {
            bool rotatedAny = false;
            for (int i = 0; i < 3; i++)
            {
                if (_activeBlocks[i] != null)
                {
                    _activeBlocks[i].Shape.Rotate90Clockwise();
                    _activeBlocks[i].RebuildVisuals(gemTileSprite, bombIconSprite);
                    rotatedAny = true;
                }
            }

            if (rotatedAny)
            {
                StartCoroutine(CheckGameOverDeferred());
            }
        }

        // 🎲 Skip Current Blocks and Reroll New Set
        public void SkipHandBlocks()
        {
            SpawnNewHand();
        }

        public void RerollHand()
        {
            SpawnNewHand();
        }

        private IEnumerator CheckGameOverDeferred()
        {
            // Wait for line clear animations and board updates to settle
            yield return new WaitForSeconds(0.35f);

            if (BlockGridManager.Instance == null) yield break;

            bool anyCanFit = false;

            for (int i = 0; i < 3; i++)
            {
                if (_activeBlocks[i] != null)
                {
                    // Check all 4 rotations (0, 90, 180, 270) so players aren't unfairly blocked!
                    if (CanShapeFitAnywhereWithRotation(_activeBlocks[i].Shape))
                    {
                        anyCanFit = true;
                        break;
                    }
                }
            }

            bool hasRemainingBlocks = false;
            for (int i = 0; i < 3; i++)
            {
                if (_activeBlocks[i] != null) { hasRemainingBlocks = true; break; }
            }

            if (hasRemainingBlocks && !anyCanFit)
            {
                Debug.Log("[BlockBlast] No more moves in current hand. Waiting for Skip or timer expiration.");
                if (BlockBlastUIManager.Instance != null)
                {
                    BlockBlastUIManager.Instance.NotifyNoMovesAvailable();
                }
            }
        }

        // Check 4 rotations so game doesn't end if rotating can fit!
        private bool CanShapeFitAnywhereWithRotation(BlockShape shape)
        {
            int[,] originalMatrix = (int[,])shape.matrix.Clone();

            for (int rot = 0; rot < 4; rot++)
            {
                for (int r = 0; r < BlockGridManager.GridSize; r++)
                {
                    for (int c = 0; c < BlockGridManager.GridSize; c++)
                    {
                        if (BlockGridManager.Instance.CanPlaceShape(shape, r, c))
                        {
                            // Restore original matrix before returning
                            shape.matrix = originalMatrix;
                            return true;
                        }
                    }
                }
                shape.Rotate90Clockwise();
            }

            // Restore original matrix
            shape.matrix = originalMatrix;
            return false;
        }

        public void ClearHand()
        {
            for (int i = 0; i < 3; i++)
            {
                if (_activeBlocks[i] != null)
                {
                    Destroy(_activeBlocks[i].gameObject);
                    _activeBlocks[i] = null;
                }
            }
        }

        public void SetupReferences(Transform[] slots, Canvas canvas, Sprite gem, Sprite bomb)
        {
            slotParents = slots;
            mainCanvas = canvas;
            gemTileSprite = gem;
            bombIconSprite = bomb;
        }
    }
}
