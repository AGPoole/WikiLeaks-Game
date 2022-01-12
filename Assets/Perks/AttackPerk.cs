public class AttackPerk : PerkBase
{
    public override void OnClick()
    {
        m_xSystemOwner.Attack(true);
    }
}
