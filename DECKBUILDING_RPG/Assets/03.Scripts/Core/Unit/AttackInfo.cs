
public class AttackInfo
{
    public bool isMelee;
    public string bulletName;
    public int range;
    public int damage;
    public int offsetBulletDamage;
    public int radius;
    public int pushForce;

    public AttackInfo(bool isMelee, string bulletName, int range, int damage, int offsetBulletDamage, int radius, int pushForce)
    {
        this.isMelee = isMelee;
        this.bulletName = bulletName;
        this.range = range;
        this.damage = damage;
        this.offsetBulletDamage = offsetBulletDamage;
        this.radius = radius;
        this.pushForce = pushForce;
    }
}