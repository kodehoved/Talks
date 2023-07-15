namespace RefactoringToClasses.Alternative;

public class Player
{
    public int X { get; private set; }
    public int Y { get; private set; }

    private readonly Stack<(int xDelta, int yDelta)> _inputs = new();

    public void InputLeft()
    {
        _inputs.Push((-1, 0));
    }

    public void InputRight()
    {
        _inputs.Push((1, 0));
    }

    public void InputUp()
    {
        _inputs.Push((0, -1));
    }

    public void InputDown()
    {
        _inputs.Push((0, 1));
    }

    public void HandleInputs()
    {
        while (_inputs.Count > 0)
        {
            var (xDelta, yDelta) = _inputs.Pop();
            X += xDelta;
            Y += yDelta;
        }
    }
}
