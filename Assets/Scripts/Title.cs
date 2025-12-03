using UnityEngine;

/// <summary>
/// Handles the title screen input.
/// Starts a new game when the player presses Enter (Return key).
/// </summary>
public class Title : MonoBehaviour
{
    private void Update()
    {
        // When the player presses Enter, start a new game if the GameManager exists.
        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (GameManager.Instance != null)
            {
                GameManager.Instance.NewGame();
            }
            else
            {
                Debug.LogWarning("[Title] GameManager.Instance is null — cannot start a new game.");
            }
        }
    }
}