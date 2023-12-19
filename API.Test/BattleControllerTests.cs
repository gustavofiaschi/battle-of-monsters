using API.Controllers;
using API.Test.Fixtures;
using FluentAssertions;
using Lib.Repository.Entities;
using Lib.Repository.Repository;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace API.Test;

public class BattleControllerTests
{
    private readonly Mock<IBattleOfMonstersRepository> _repository;

    public BattleControllerTests()
    {
        this._repository = new Mock<IBattleOfMonstersRepository>();        
    }

    [Fact]
    public async void Get_OnSuccess_ReturnsListOfBattles()
    {
        this._repository
            .Setup(x => x.Battles.GetAllAsync())
            .ReturnsAsync(BattlesFixture.GetBattlesMock());

        BattleController sut = new BattleController(_repository.Object);
        ActionResult result = await sut.GetAll();
        OkObjectResult objectResults = (OkObjectResult) result;
        objectResults?.Value.Should().BeOfType<Battle[]>();
    }
    
    [Fact]
    public async Task Post_BadRequest_When_StartBattle_With_nullMonster()
    {
        Monster[] monstersMock = MonsterFixture.GetMonstersMock().ToArray();
        
        Battle b = new Battle()
        {
            MonsterA = null,
            MonsterB = monstersMock[1].Id
        };

        this._repository.Setup(x => x.Battles.AddAsync(b));

        int? idMonsterA = null;
        this._repository
            .Setup(x => x.Monsters.FindAsync(idMonsterA))
            .ReturnsAsync(() => null);

        int? idMonsterB = monstersMock[1].Id;
        Monster monsterB = monstersMock[1];

        this._repository
            .Setup(x => x.Monsters.FindAsync(idMonsterB))
            .ReturnsAsync(monsterB);

        BattleController sut = new BattleController(_repository.Object);

        ActionResult result = await sut.Add(b);
        BadRequestObjectResult objectResults = (BadRequestObjectResult) result;
        result.Should().BeOfType<BadRequestObjectResult>();
        Assert.Equal("Missing ID", objectResults.Value);
    }
    
    [Fact]
    public async Task Post_OnNoMonsterFound_When_StartBattle_With_NonexistentMonster()
    {        
        Monster[] monstersMock = MonsterFixture.GetMonstersMock().ToArray();
        
        Battle b = new Battle()
        {
            MonsterA = 8,
            MonsterB = monstersMock[1].Id
        };

        this._repository.Setup(x => x.Battles.AddAsync(b));

        int? idMonsterA = 8;
        this._repository
            .Setup(x => x.Monsters.FindAsync(idMonsterA))
            .ReturnsAsync(() => null);

        int? idMonsterB = monstersMock[1].Id;
        Monster monsterB = monstersMock[1];

        this._repository
            .Setup(x => x.Monsters.FindAsync(idMonsterB))
            .ReturnsAsync(monsterB);

        BattleController sut = new BattleController(_repository.Object);

        ActionResult result = await sut.Add(b);
        NotFoundObjectResult objectResults = (NotFoundObjectResult) result;
        result.Should().BeOfType<NotFoundObjectResult>();
        Assert.Equal("Missing Monster", objectResults.Value);
    }

    [Fact]
    public async Task Post_OnSuccess_Returns_With_MonsterAWinning()
    {
        Monster[] monstersMock = MonsterFixture.GetMonstersMock().ToArray();
        Monster monsterA = monstersMock[1];
        Monster monsterB = monstersMock[0];

        Battle b = new Battle()
        {
            MonsterA = monsterA.Id,
            MonsterB = monsterB.Id
        };

        this._repository.Setup(x => x.Battles.AddAsync(b));
        
        this._repository
            .Setup(x => x.Monsters.FindAsync(monsterA.Id))
            .ReturnsAsync(monsterA);
        
        this._repository
            .Setup(x => x.Monsters.FindAsync(monsterB.Id))
            .ReturnsAsync(monsterB);

        BattleController sut = new BattleController(_repository.Object);

        ActionResult result = await sut.Add(b);
        OkObjectResult objectResult = (OkObjectResult) result;
        result.Should().BeOfType<OkObjectResult>();

        var expected = objectResult.Value as Battle;

        Assert.Equal(monsterA.Id, expected.Winner);        
    }


    [Fact]
    public async Task Post_OnSuccess_Returns_With_MonsterBWinning()
    {
        Monster[] monstersMock = MonsterFixture.GetMonstersMock().ToArray();
        Monster monsterA = monstersMock[0];
        Monster monsterB = monstersMock[1];

        Battle b = new Battle()
        {
            MonsterA = monsterA.Id,
            MonsterB = monsterB.Id
        };

        this._repository.Setup(x => x.Battles.AddAsync(b));
        
        this._repository
            .Setup(x => x.Monsters.FindAsync(monsterA.Id))
            .ReturnsAsync(monsterA);
        
        this._repository
            .Setup(x => x.Monsters.FindAsync(monsterB.Id))
            .ReturnsAsync(monsterB);

        BattleController sut = new BattleController(_repository.Object);

        ActionResult result = await sut.Add(b);
        OkObjectResult objectResult = (OkObjectResult) result;
        result.Should().BeOfType<OkObjectResult>();

        var expected = objectResult.Value as Battle;

        Assert.Equal(monsterB.Id, expected.Winner);  
    }

    [Fact]
    public async Task Post_OnSuccess_Returns_With_MonsterAWinning_When_TheirSpeedsSame_And_MonsterA_Has_Higher_Attack()
    {
        Monster[] monstersMock = MonsterFixture.GetMonstersMock().ToArray();
        Monster monsterA = monstersMock[0];
        Monster monsterB = monstersMock[5];

        Battle b = new Battle()
        {
            MonsterA = monsterA.Id,
            MonsterB = monsterB.Id
        };

        this._repository.Setup(x => x.Battles.AddAsync(b));
        
        this._repository
            .Setup(x => x.Monsters.FindAsync(monsterA.Id))
            .ReturnsAsync(monsterA);
        
        this._repository
            .Setup(x => x.Monsters.FindAsync(monsterB.Id))
            .ReturnsAsync(monsterB);

        BattleController sut = new BattleController(_repository.Object);

        ActionResult result = await sut.Add(b);
        OkObjectResult objectResult = (OkObjectResult) result;
        result.Should().BeOfType<OkObjectResult>();

        var expected = objectResult.Value as Battle;

        Assert.True(monsterA.Speed == monsterB.Speed); 
        Assert.True(monsterA.Attack > monsterB.Attack);
        Assert.Equal(monsterA.Id, expected.Winner);    
    }

    [Fact]
    public async Task Post_OnSuccess_Returns_With_MonsterBWinning_When_TheirSpeedsSame_And_MonsterB_Has_Higher_Attack()
    {
        Monster[] monstersMock = MonsterFixture.GetMonstersMock().ToArray();
        Monster monsterA = monstersMock[5];
        Monster monsterB = monstersMock[0];

        Battle b = new Battle()
        {
            MonsterA = monsterA.Id,
            MonsterB = monsterB.Id
        };

        this._repository.Setup(x => x.Battles.AddAsync(b));
        
        this._repository
            .Setup(x => x.Monsters.FindAsync(monsterA.Id))
            .ReturnsAsync(monsterA);
        
        this._repository
            .Setup(x => x.Monsters.FindAsync(monsterB.Id))
            .ReturnsAsync(monsterB);

        BattleController sut = new BattleController(_repository.Object);

        ActionResult result = await sut.Add(b);
        OkObjectResult objectResult = (OkObjectResult) result;
        result.Should().BeOfType<OkObjectResult>();

        var expected = objectResult.Value as Battle;

        Assert.True(monsterB.Speed == monsterA.Speed); 
        Assert.True(monsterB.Attack > monsterA.Attack);
        Assert.Equal(monsterB.Id, expected.Winner); 
    }

    [Fact]
    public async Task Post_OnSuccess_Returns_With_MonsterAWinning_When_TheirDefensesSame_And_MonsterA_Has_Higher_Speed()
    {
        Monster[] monstersMock = MonsterFixture.GetMonstersMock().ToArray();
        Monster monsterA = monstersMock[0];
        Monster monsterB = monstersMock[2];

        Battle b = new Battle()
        {
            MonsterA = monsterA.Id,
            MonsterB = monsterB.Id
        };

        this._repository.Setup(x => x.Battles.AddAsync(b));
        
        this._repository
            .Setup(x => x.Monsters.FindAsync(monsterA.Id))
            .ReturnsAsync(monsterA);
        
        this._repository
            .Setup(x => x.Monsters.FindAsync(monsterB.Id))
            .ReturnsAsync(monsterB);

        BattleController sut = new BattleController(_repository.Object);

        ActionResult result = await sut.Add(b);
        OkObjectResult objectResult = (OkObjectResult) result;
        result.Should().BeOfType<OkObjectResult>();

        var expected = objectResult.Value as Battle;

        Assert.True(monsterA.Defense == monsterA.Defense);
        Assert.True(monsterA.Speed > monsterB.Speed);
        Assert.Equal(monsterA.Id, expected.Winner); 
    }

    [Fact]
    public async Task Delete_OnSuccess_RemoveBattle()
    {   
        var battleId = 1;     

        Battle battle = new Battle()
        {
            MonsterA = 1,
            MonsterB = 2,
            Winner = 1
        };

        this._repository.Setup(x => x.Battles.RemoveAsync(battleId));
        this._repository.Setup(x => x.Battles.FindAsync(battleId)).ReturnsAsync(battle);
                
        BattleController sut = new BattleController(_repository.Object);

        ActionResult result = await sut.Remove(battleId);
        OkResult objectResult = (OkResult) result;
        result.Should().BeOfType<OkResult>();        
    }

    [Fact]
    public async Task Delete_OnNoBattleFound_Returns404()
    {
        var battleId = 1;     

        this._repository.Setup(x => x.Battles.RemoveAsync(battleId));
        this._repository.Setup(x => x.Battles.FindAsync(battleId)).ReturnsAsync(() => null);
                
        BattleController sut = new BattleController(_repository.Object);

        ActionResult result = await sut.Remove(battleId);
        NotFoundObjectResult objectResult = (NotFoundObjectResult) result;
        Assert.Equal("Battle not found", objectResult.Value);
    }
}
