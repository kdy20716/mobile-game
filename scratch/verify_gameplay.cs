// Complete gameplay validation script
var lm = UnityEngine.Object.FindAnyObjectByType<BlockBlast.LobbyManager>();
if (lm == null) return "No LobbyManager";

int step = 0;
string stepFile = "scratch/gameplay_step.txt";
if (System.IO.File.Exists(stepFile))
{
    int.TryParse(System.IO.File.ReadAllText(stepFile).Trim(), out step);
}

switch (step)
{
    case 0: // Transition from Lobby into Ingame
        lm.StartGameFromLobby();
        return "Transitioned to Ingame";

    case 1: // Place a shape on the board
        if (BlockBlast.BlockGridManager.Instance != null)
        {
            var shape = BlockBlast.BlockShapeData.GetRandomShape(0f);
            bool placed = BlockBlast.BlockGridManager.Instance.PlaceShape(shape, 3, 3);
            return $"Placed shape at (3,3): {placed}";
        }
        return "BlockGridManager not available";

    case 2: // Open Pause Modal
        if (BlockBlast.BlockBlastUIManager.Instance != null)
        {
            BlockBlast.BlockBlastUIManager.Instance.OpenPauseModal();
            return "Pause Modal Opened";
        }
        return "No UIManager";

    case 3: // Return to Lobby
        if (BlockBlast.BlockBlastUIManager.Instance != null)
        {
            BlockBlast.BlockBlastUIManager.Instance.ClosePauseModal();
        }
        lm.ReturnToLobby();
        return "Returned to Lobby";
}

return $"Step {step} done";
