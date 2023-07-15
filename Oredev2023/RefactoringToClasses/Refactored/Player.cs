namespace RefactoringToClasses.Refactored;

public class Player
{
    private readonly IInput _left;
    private readonly IInput _right;
    private readonly IInput _up;
    private readonly IInput _down;

    private readonly Stack<IInput> _inputs = new Stack<IInput>();

    public Player()
    {
        X = Y = 0;
        _left = new Left(this);
        _right = new Right(this);
        _up = new Up(this);
        _down = new Down(this);
    }

    public int X { get; private set; }
    public int Y { get; private set; }

    public void InputLeft()
    {
        _inputs.Push(_left);
    }

    public void InputRight()
    {
        _inputs.Push(_right);
    }

    public void InputUp()
    {
        _inputs.Push(_up);
    }

    public void InputDown()
    {
        _inputs.Push(_down);
    }

    public void HandleInputs()
    {
        while (_inputs.Count > 0)
        {
            var input = _inputs.Pop();
            input.Handle();
        }
    }

    private void MoveHorizontal(int offset)
    {
        X += offset;
    }

    private void MoveVertical(int offset)
    {
        Y += offset;
    }

    public class Left : IInput
    {
        private readonly Player _player;

        internal Left(Player player)
        {
            _player = player;
        }

        public void Handle()
        {
            _player.MoveHorizontal(-1);
        }
    }

    public class Right : IInput
    {
        private readonly Player _player;

        internal Right(Player player)
        {
            _player = player;
        }

        public void Handle()
        {
            _player.MoveHorizontal(1);
        }
    }

    public class Up : IInput
    {
        private readonly Player _player;

        internal Up(Player player)
        {
            _player = player;
        }

        public void Handle()
        {
            _player.MoveVertical(-1);
        }
    }

    public class Down : IInput
    {
        private readonly Player _player;

        internal Down(Player player)
        {
            _player = player;
        }

        public void Handle()
        {
            _player.MoveVertical(1);
        }
    }
}
