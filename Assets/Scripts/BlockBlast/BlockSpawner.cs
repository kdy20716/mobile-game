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
            SpawnNewHand();
        }

        public void SpawnNewHand()
        {
            for (int i = 0; i < 3; i++)
            {
                if (slotParents != null && i < slotParents.Length && slotParents[i] != null)
                {
                    BlockShape shape = BlockShapeData.GetRandomShape(0.15f);

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
                if (BlockAudioManager.Instance != null)
                {
                    BlockAudioManager.Instance.PlayPickup();
                }
                StartCoroutine(CheckGameOverDeferred());
            }
        }

        private IEnumerator CheckGameOverDeferred()
        {
            // Give 0.35s for line clear animations and board updates to settle
            yield return new WaitForSeconds(0.35f);

            if (BlockGridManager.Instance == null) yield break;

            bool anyCanFit = false;

            for (int i = 0; i < 3; i++)
            {
                if (_activeBlocks[i] != null)
                {
                    if (CanShapeFitAnywhere(_activeBlocks[i].Shape))
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
                Debug.Log("[BlockBlast] No more moves! Game Over triggered.");
                if (BlockBlastUIManager.Instance != null)
                {
                    BlockBlastUIManager.Instance.ShowGameOver();
                }
            }
        }

        private bool CanShapeFitAnywhere(BlockShape shape)
        {
            for (int r = 0; r < BlockGridManager.GridSize; r++)
            {
                for (int c = 0; c < BlockGridManager.GridSize; c++)
                {
                    if (BlockGridManager.Instance.CanPlaceShape(shape, r, c))
                    {
                        return true;
                    }
                }
            }
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
