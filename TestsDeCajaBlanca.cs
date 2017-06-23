using UnityEngine;
using UnityEditor;
using NUnit.Framework;
using System.Collections.Generic;
using System;

public class TestsDeCajaBlanca{

	[Test]
	public void Test1() {
		//Arrange
		var gameObject = new GameObject();

		//Act
		//Try to rename the GameObject
		var newGameObjectName = "My game object";
		gameObject.name = newGameObjectName;

		//Assert
		//The object has a new name
		Assert.AreEqual(newGameObjectName, gameObject.name);
	}
    [Test]
    public void TestPlayerHandager()
    {
        PlayerHandager playerHandanger = new PlayerHandager();

        //expected object
        DuelistPlayer expectedPlayer = new DuelistPlayer("Player: Me", 8000);

        //actual object
        playerHandanger.createPlayers();
        DuelistPlayer actualPlayer = playerHandanger.getPlayer();

        //result
        Assert.AreEqual(actualPlayer.playerName, expectedPlayer.playerName);
    }
    [Test]
    public void TestDamageCalculationPlayer()
    {
        PlayerHandager playerHandanger = new PlayerHandager();
        DamageCalculation damageCalculation = new DamageCalculation();
        MonsterCard monsterCard = new MonsterCard(1, null, null, null, 150, 1, 1, null);
        GameCard gameCard = new GameCard(null, null, monsterCard, null, 1);

        //expected value
        playerHandanger.createPlayers();//aka instancias Thisplayer y EnemyPlayer, asi q abajo lo jalamos con playerHandanger.getPlayer();

        DuelistPlayer actualPlayer = playerHandanger.getPlayer();
        int expectedLifePoints = actualPlayer.lifePoints - monsterCard.attack;

        //actual value
        damageCalculation.playerhandager = playerHandanger;
        damageCalculation.calculateBattleDamageBT(gameCard);

        int actualLifePoint = actualPlayer.lifePoints;

        //result
        Assert.AreEqual(expectedLifePoints, actualLifePoint);//7850=8000-150
    }
    [Test]
    public void TestDamageCalculationEnemy()
    {
        PlayerHandager playerHandanger = new PlayerHandager();
        DamageCalculation damageCalculation = new DamageCalculation();
        MonsterCard monsterCard = new MonsterCard(1, null, null, null, 200, 1, 1, null);
        GameCard gameCard = new GameCard(null, null, monsterCard, null, 1);

        //expected value
        playerHandanger.createPlayers();//aka instancias Thisplayer y EnemyPlayer, asi q abajo lo jalamos con playerHandanger.getEnemy();

        DuelistPlayer actualEnemy = playerHandanger.getEnemy();
        int expectedLifePoints = actualEnemy.lifePoints - monsterCard.attack;

        //actual value
        damageCalculation.playerhandager = playerHandanger;
        damageCalculation.battleMachine = damageCalculation.battleMachine ?? new BattleStateMachine();//mas cool pz prrn
        damageCalculation.battleMachine.raycasterLists = new RaycasterLists();//tnms q instanciar esta mamada
        damageCalculation.battleMachine.raycasterStates = new RaycasterStates();//tnms q instanciar esta mamada
        damageCalculation.calculateBattleDamage(gameCard);

        int actualLifePoint = actualEnemy.lifePoints;

        //result
        Assert.AreEqual(expectedLifePoints, actualLifePoint);//7800=8000-200
    }
    [Test]
    public void TestMonsterAttack()
    {
        List<GameCard> magicAndTrapCards= new List<GameCard>();
        MonsterCard monsterCard = new MonsterCard(1, null, null, null, 200, 1, 1, null);
        GameCard gameCard = new GameCard(null, null, monsterCard, null, 4);//tipo 4 de cartas magilusho y trapos de donlusho
        magicAndTrapCards.Add(gameCard);

        //expected value: AskTrapActivationOnAction en una carta magica o trampa,
        string expectedValue = "AskTrapActivationOnAction";

        //actualValue
        MonsterAttack monsterAttack = new MonsterAttack();
        monsterAttack.playerHandager = new PlayerHandager();
        monsterAttack.playerHandager.createPlayers();//creamos player y enemy
        monsterAttack.playerHandager.getEnemy().magicAndTrapCards=magicAndTrapCards;//get DuelistPlayer de enmigo y seteamos sus cartas magicas y trampas

        monsterAttack.multiPlayerManager = new MultiPlayerManager();//Metodo AttemptAttack lo requiere, sino serial null y se cae
        monsterAttack.multiPlayerManager.btPackage = new BTPackage();
        monsterAttack.multiPlayerManager.btSettings = new BTsettings();

        monsterAttack.popupManager = new PopupManager();//Metodo AttemptAttack lo requiere, sino serial null y se cae
        monsterAttack.popupManager.freezePanel = new GameObject();
        monsterAttack.AttemptAttack();

        string actualValue = monsterAttack.multiPlayerManager.btPackage.packageType;

        //result
        Assert.AreEqual(expectedValue, actualValue);
    }
    [Test]
    public void TestDuelistPlayerHasTrapCards()
    {
        List<GameCard> magicAndTrapCards = new List<GameCard>();
        MonsterCard monsterCard = new MonsterCard(1, null, null, null, 200, 1, 1, null);
        GameCard gameCard = new GameCard(null, null, monsterCard, null, 4);//tipo 4 de cartas magilusho y trapos de donlusho
        magicAndTrapCards.Add(gameCard);

        //expected value
        bool expectedValue= true;

        //actual value
        MonsterAttack monsterAttack = new MonsterAttack();
        monsterAttack.playerHandager = new PlayerHandager();
        monsterAttack.playerHandager.createPlayers();//creamos player y enemy
        monsterAttack.playerHandager.getEnemy().magicAndTrapCards = magicAndTrapCards;
        bool actualValue = monsterAttack.playerHandager.getEnemy().HasTrapCards();//get DuelistPlayer de enmigo y seteamos sus cartas magicas y trampas

        //result
        Assert.AreEqual(expectedValue, actualValue);
    }
    [Test]
    public void TestTrapEffect()
    {
        MonsterCard monsterCard = new MonsterCard(1, null, null, null, 200, 1, 1, null);
        GameCard gameCard = new GameCard(null, null, monsterCard, null, 3);//3 para q entre en la linea 69: "This card is not trap"

        //expected value
        int expectedValue = gameCard.type;
        
        //actual value
        TrapEffect trapEffect = new TrapEffect();
        trapEffect.effectCard = gameCard;
        trapEffect.popupManager = new PopupManager();//Se requiere en el metodo checktrapcardset, sino dara error x null
        trapEffect.popupManager.QuickPopup = new GameObject();
        trapEffect.popupManager.DuelCanvas = new GameObject();
        trapEffect.popupManager.QuickPopup.AddComponent<UnityEngine.UI.Text>();
        trapEffect.CheckTrapCardSet(gameCard);
        int actualValue = trapEffect.effectCard.type;

        //result
        Assert.AreEqual(expectedValue, actualValue);
    }
    [Test]
    public void MonsterEffect()
    {
        UnityEngine.UI.Text newText = (new GameObject().AddComponent<UnityEngine.UI.Text>()).GetComponent<UnityEngine.UI.Text>();//no se puede instanciar un Text, me tira muchos errores como nivel de proteccion de Text de unity, asi q encontre esta formula... ojala funke...
        UnityEngine.UI.Button newButton = (new GameObject().AddComponent<UnityEngine.UI.Button>()).GetComponent<UnityEngine.UI.Button>();//no se puede instanciar un Button, me tira muchos errores como nivel de proteccion de Button de unity, asi q encontre esta formula... ojala funke...

        MonsterCard monsterCard = new MonsterCard(1, null, null, null, 200, 1, 1, null);
        GameCard gameCard = new GameCard(null, null, monsterCard, null, 2);//2 para q entre en la linea 69: "This card is not trap"
        MonsterEffect monsterEffect = new MonsterEffect();
        monsterEffect.popupManager = new PopupManager();
        monsterEffect.popupManager.popupPanelObject = new GameObject();
        monsterEffect.popupManager.okButtonText = newText;//solo sirve para instanciar y no se kede en null
        monsterEffect.popupManager.okButton = newButton;
        monsterEffect.popupManager.noButtonText = newText;
        monsterEffect.popupManager.noButton = newButton;
        monsterEffect.popupManager.message = newText;
        monsterEffect.popupManager.cancelButton = newButton;

        //expectedValue
        string expectedValue = "Do yo wish to activate  effect?";

        //actualValue
        monsterEffect.CheckIfEffectIsTriggered(gameCard, 0);//0 para q sea trigger el efecctGameCard, xq en el constructor del MonsterCard se asigna 0 a TriggerId, revisar linea 31 de MonsterEffect
        string actualValue = monsterEffect.popupManager.message.text;

        //result
        Assert.AreEqual(expectedValue, actualValue);
    }
}
