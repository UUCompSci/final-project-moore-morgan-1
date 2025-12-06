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
    
}