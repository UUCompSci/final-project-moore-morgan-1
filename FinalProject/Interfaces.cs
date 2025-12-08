using Entities;

namespace Interfaces;

interface iAttack{
void Attack(LivingThing target);
}

interface iConverse
{
    string VoiceLine {get; set;}
    void Converse();
}

interface iMove
{
    void Move(Coords NextLocation);
   
}

interface iEquipable
{
    //Makes it where only people, and subsequently ghouls, can equip items.
    void Equip(Person target);
}