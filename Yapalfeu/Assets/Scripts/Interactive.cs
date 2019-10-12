public interface Interactive
{
    void Select();
    void Deselect();
    Action GetAction(PlayerState state);
    void DoAction(PlayerState state);
}
