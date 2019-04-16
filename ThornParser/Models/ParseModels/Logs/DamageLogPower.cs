namespace ThornParser.Models.ParseModels
{
    public class DamageLogPower : DamageLog
    {
        public DamageLogPower(long time, CombatItem c) : base(time, c)
        {
            Damage =  c.Value;
        }
    }
}