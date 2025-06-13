namespace Amethyst.Server.Entities.Players.Sections;

public sealed class PlayerSections(PlayerEntity player)
{
    public PlayerEntity Player { get; } = player;

    public int SectionX => (int)(Player.Position.X / 16 / 200);
    public int SectionY => (int)(Player.Position.Y / 16 / 150);

    private readonly bool[,] _sentSections = new bool[8401 / 200, 2401 / 200];

    public bool IsValidSection(int sectionX, int sectionY)
    {
        return sectionX >= 0 && sectionY >= 0 && sectionX < _sentSections.GetLength(0) && sectionY < _sentSections.GetLength(1);
    }

    public bool IsSent(int sectionX, int sectionY)
    {
        if (sectionX < 0 || sectionY < 0 || sectionX >= _sentSections.GetLength(0) || sectionY >= _sentSections.GetLength(1))
        {
            return false;
        }

        return _sentSections[sectionX, sectionY];
    }

    public void MarkAsSent(int sectionX, int sectionY)
    {
        if (sectionX < 0 || sectionY < 0 || sectionX >= _sentSections.GetLength(0) || sectionY >= _sentSections.GetLength(1))
        {
            return;
        }

        _sentSections[sectionX, sectionY] = true;
    }
    public void UnmarkAsSent(int sectionX, int sectionY)
    {
        if (sectionX < 0 || sectionY < 0 || sectionX >= _sentSections.GetLength(0) || sectionY >= _sentSections.GetLength(1))
        {
            return;
        }

        _sentSections[sectionX, sectionY] = false;
    }

    public void Reset()
    {
        for (int x = 0; x < _sentSections.GetLength(0); x++)
        {
            for (int y = 0; y < _sentSections.GetLength(1); y++)
            {
                _sentSections[x, y] = false;
            }
        }
    }
}
