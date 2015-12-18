using UnityEngine;

public class SnakeControlPad : MonoBehaviour
{
    [HideInInspector] public ClientSnake Snake;

    protected void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            OnInput(-1, 0);
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            OnInput(1, 0);
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            OnInput(0, 1);
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            OnInput(0, -1);
        }
    }

    public void OnLeftButtonClick()
    {
        OnInput(-1, 0);
    }

    public void OnRightButtonClick()
    {
        OnInput(1, 0);
    }

    public void OnUpButtonClick()
    {
        OnInput(0, 1);
    }

    public void OnDownButtonClick()
    {
        OnInput(0, -1);
    }

    private void OnInput(int orientX, int orientY)
    {
        if (Snake != null)
            Snake.QueueInput(orientX, orientY);
    }
}
