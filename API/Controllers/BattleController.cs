using Lib.Repository.Entities;
using Lib.Repository.Repository;
using Lib.Repository.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;


public class BattleController : BaseApiController
{
    private readonly IBattleOfMonstersRepository _repository;

    public BattleController(IBattleOfMonstersRepository repository)
    {
        this._repository = repository;
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> GetAll()
    {
        IEnumerable<Battle> battles = await _repository.Battles.GetAllAsync();
        return Ok(battles);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> Add([FromBody] Battle battle)    
    {
        if (battle.MonsterA == null || battle.MonsterB == null)
            return BadRequest("Missing ID");

        Monster monsterA = await _repository.Monsters.FindAsync(battle.MonsterA);
        Monster monsterB = await _repository.Monsters.FindAsync(battle.MonsterB);

        if (monsterA == null || monsterB == null)
            return NotFound("Missing Monster");

        battle.Winner = BattleService.ExecuteBattle(monsterA, monsterB);
         
        await _repository.Battles.AddAsync(battle);
        await _repository.Save();
        return Ok(battle);
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult> Remove(int id)
    {
        Battle battle = await _repository.Battles.FindAsync(id);

        if (battle == null)
            return NotFound("Battle not found");
            
        await _repository.Battles.RemoveAsync(id);
        await _repository.Save();
        return Ok();
    }
}


