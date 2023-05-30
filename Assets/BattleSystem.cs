using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum BattleState { START, PLAYERTURN, ENEMYTURN, WON, LOST }

public class BattleSystem : MonoBehaviour
{
	
	public GameObject playerPrefab;
	public GameObject enemyPrefab;

	public Transform playerBattleStation;
	public Transform enemyBattleStation;

	Unit playerUnit;
	Unit enemyUnit;

	public Text dialogueText;

	public BattleHUD playerHUD;
	public BattleHUD enemyHUD;

	public BattleState state;

	public bool EsquiveActive = false;

	private Vector3 positionInitiale;
    private Vector3 positionCible;
    public float vitesseDeplacement = 5f;

    // Start is called before the first frame update
    void Start()
    {
		state = BattleState.START;
		StartCoroutine(SetupBattle());
    }

	IEnumerator SetupBattle()
	{
		GameObject playerGO = Instantiate(playerPrefab, playerBattleStation);
		playerUnit = playerGO.GetComponent<Unit>();

		GameObject enemyGO = Instantiate(enemyPrefab, enemyBattleStation);
		enemyUnit = enemyGO.GetComponent<Unit>();

		dialogueText.text =  enemyUnit.unitName + " approches il faut le conbattre.";
		yield return new WaitForSeconds(3f);
		dialogueText.text = "Il a "+ enemyUnit.DefencePoucentage.ToString()+"% de defense.";
		playerHUD.SetHUD(playerUnit);
		enemyHUD.SetHUD(enemyUnit);

		yield return new WaitForSeconds(1f);

		
		dialogueText.text =  "Ma vitesse est de "+playerUnit.VitessePourcentage.ToString() + " %";
		yield return new WaitForSeconds(1f);
		dialogueText.text =  "Mon Poucentage De Defense est de "+playerUnit.DefencePoucentage.ToString() + "%";
		yield return new WaitForSeconds(1f);
		state = BattleState.PLAYERTURN;
		PlayerTurn();
	}


	IEnumerator PlayerAttack()
	{


		enemyHUD.SetHP(enemyUnit.currentHP);
		dialogueText.text = "L'attaque a marché!";
		playerUnit.DeplacerVersCoordonnees(3,1,0);


		yield return new WaitForSeconds(0.75f);


		bool isDead = enemyUnit.TakeDamage(playerUnit.damage);
		playerUnit.RevenirAPositionInitiale();
		enemyHUD.SetHP(enemyUnit.currentHP);

		if(isDead)
		{
			state = BattleState.WON;
			EndBattle();
		} else
		{
			state = BattleState.ENEMYTURN;
			StartCoroutine(EnemyTurn());
		}
	}

	IEnumerator EnemyTurn()
	{
		yield return new WaitForSeconds(2f);
		dialogueText.text = enemyUnit.unitName + " vous attaque!";

		yield return new WaitForSeconds(1f);
		if(EsquiveActive)
		{
       		int chanceEsquive = playerUnit.VitessePourcentage;
        	int roll = Random.Range(0, 101);
			if (roll <= chanceEsquive)
        	{

				dialogueText.text = "L'esquive a marché!!!!";
				playerUnit.DeplacerVersCoordonnees(3,1,0);
				yield return new WaitForSeconds(0.75f);
				bool EnnemyisDead = enemyUnit.TakeDamage(playerUnit.damage);

				playerUnit.DeplacerVersCoordonnees(0,0,0);
				yield return new WaitForSeconds(0.75f);
				playerUnit.DeplacerVersCoordonnees(3,1,0);
				yield return new WaitForSeconds(0.5f);
				dialogueText.text = "Je peux l'attaquer 2 fois";
				enemyHUD.SetHP(enemyUnit.currentHP);

				yield return new WaitForSeconds(1f);
				if(EnnemyisDead)
				{
					state = BattleState.WON;
					EndBattle();
					yield return new WaitForSeconds(70f);
					
        		}
				bool EnnemyisDead2 = enemyUnit.TakeDamage(playerUnit.damage);
				playerUnit.RevenirAPositionInitiale();
				enemyHUD.SetHP(enemyUnit.currentHP);

				yield return new WaitForSeconds(1f);
				if(EnnemyisDead2)
				{
					state = BattleState.WON;
					EndBattle();
        		}
			}
			else
			{
				dialogueText.text = "L'esquive a raté!!!!";
				playerUnit.RevenirAPositionInitiale();
				yield return new WaitForSeconds(1.5f);
				enemyUnit.DeplacerVersCoordonnees(-3,-3,0);
				yield return new WaitForSeconds(1f);
				bool isDead = playerUnit.TakeDamage(enemyUnit.damage);


				enemyUnit.DeplacerVersCoordonnees(-1,-2,0);
				yield return new WaitForSeconds(0.75f);

				playerHUD.SetHP(playerUnit.currentHP);

				if(isDead)
				{
					state = BattleState.LOST;
					EndBattle();
										yield return new WaitForSeconds(70f);
				}
				dialogueText.text = enemyUnit.unitName+" vous attauqe 2 fois";
				yield return new WaitForSeconds(1f);
				enemyUnit.DeplacerVersCoordonnees(-3,-3,0);
				yield return new WaitForSeconds(1f);
				bool isDead2 = playerUnit.TakeDamage(enemyUnit.damage);
				enemyUnit.RevenirAPositionInitiale();
				playerHUD.SetHP(playerUnit.currentHP);
				yield return new WaitForSeconds(0.75f);
				if(isDead2)
				{
					state = BattleState.LOST;
					EndBattle();
										yield return new WaitForSeconds(70f);
				}
			}
		}
		else
		{
			bool isDead = playerUnit.TakeDamage(enemyUnit.damage);

			playerHUD.SetHP(playerUnit.currentHP);
			enemyUnit.DeplacerVersCoordonnees(-3,-3,0);



			yield return new WaitForSeconds(0.75f);
			enemyUnit.RevenirAPositionInitiale();

			if(isDead)
			{
				state = BattleState.LOST;
				EndBattle();
									yield return new WaitForSeconds(70f);
			}
		}
		EsquiveActive = false;
		state = BattleState.PLAYERTURN;
		PlayerTurn();


	}

	void EndBattle()
	{
		if(state == BattleState.WON)
		{
			dialogueText.text = "Vous avez gagné";
		} else if (state == BattleState.LOST)
		{
			dialogueText.text = "Vous avez perdu.";
		}
	}

	void PlayerTurn()
	{
		dialogueText.text = "Que veux tu faire :";
	}

	IEnumerator PlayerEsquive()
	{
		EsquiveActive = true;

		playerHUD.SetHP(playerUnit.currentHP);
		dialogueText.text = "Vous allez esquiver le prochain coup";

		yield return new WaitForSeconds(0.75f);
		playerUnit.DeplacerVersCoordonnees(-10,0,0);

		state = BattleState.ENEMYTURN;
		StartCoroutine(EnemyTurn());
	}

	public void OnAttackButton()
	{
		if (state != BattleState.PLAYERTURN)
			return;

		StartCoroutine(PlayerAttack());
	}

	public void OnEsquiveButton()
	{
		if (state != BattleState.PLAYERTURN)
			return;

		StartCoroutine(PlayerEsquive());
	}

	IEnumerator PlayerHeal()
	{
		playerUnit.Heal(5);

		playerHUD.SetHP(playerUnit.currentHP);
		dialogueText.text = "Un peu de vie sa fait du bien";

		yield return new WaitForSeconds(2f);

		state = BattleState.ENEMYTURN;
		StartCoroutine(EnemyTurn());
	}
		public void OnHealButton()
	{
		if (state != BattleState.PLAYERTURN)
			return;

		StartCoroutine(PlayerHeal());
	}

}
