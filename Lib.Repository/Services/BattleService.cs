using Lib.Repository.Entities;

namespace Lib.Repository.Services;

public static class BattleService
{
    private static int getDamage (int attack, int defense)
    {
        if (attack > defense)
            return attack - defense;
        else
            return 1;
    }
    

    public static int ExecuteBattle(Monster monsterA, Monster monsterB)
    {
        // Choose who strike first
        bool monsterAStrike = (monsterA.Speed > monsterB.Speed) || (monsterA.Attack > monsterB.Attack);
        
        while (monsterA.Hp > 0 && monsterB.Hp > 0) {
            if (monsterAStrike) {
                var damage = getDamage(monsterA.Attack, monsterB.Defense);
                monsterB.Hp -= damage;
                monsterAStrike = false;
            } else {
                var damage = getDamage(monsterB.Attack, monsterA.Defense);
                monsterA.Hp -= damage;
                monsterAStrike = true;
            }
        }

        return monsterA.Hp <= 0 ? monsterB.Id.Value : monsterA.Id.Value;
    }

}
