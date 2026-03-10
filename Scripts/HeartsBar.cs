using Godot;

public partial class HeartsBar : Control
{
    [Export] public int MaxHearts = 3;

    [Export] public Texture2D FullHeart;
    [Export] public Texture2D HalfHeart;
    [Export] public Texture2D EmptyHeart;

    private int _halfHearts; // 6 = fuld, 0 = død
    private TextureRect[] _heartNodes;

    public override void _Ready()
    {
        _halfHearts = MaxHearts * 2; // starter med 6 halve hjerter

        _heartNodes = new TextureRect[MaxHearts];
        var container = GetNode<HBoxContainer>("HBoxContainer");

        for (int i = 0; i < MaxHearts; i++)
        {
             _heartNodes[i] = container.GetChild<TextureRect>(i);
    			_heartNodes[i].CustomMinimumSize = new Vector2(80, 80); 
    			_heartNodes[i].PivotOffset = new Vector2(32, 32);
        }

        UpdateDisplay();
    }

    public void TakeDamage(int halfHearts = 1)
    {
        _halfHearts = Mathf.Clamp(_halfHearts - halfHearts, 0, MaxHearts * 2);
        UpdateDisplay();
    }

    public void Heal(int halfHearts = 1)
    {
        _halfHearts = Mathf.Clamp(_halfHearts + halfHearts, 0, MaxHearts * 2);
        UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        for (int i = 0; i < MaxHearts; i++)
        {
            int heartValue = _halfHearts - (i * 2);

            if (heartValue >= 2)
                _heartNodes[i].Texture = FullHeart;
            else if (heartValue == 1)
                _heartNodes[i].Texture = HalfHeart;
            else
                _heartNodes[i].Texture = EmptyHeart;
        }
    }
}