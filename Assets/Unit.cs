using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{

	public string unitName;

	public int damage;

	public int maxHP;
	public int currentHP;

	public int DefencePoucentage;

	public int VitessePourcentage;

	public bool TakeDamage(int dmg)
	{
		float i = (float)DefencePoucentage/100;
		float d = (float)dmg-(float)dmg * i;
		int caca = Mathf.RoundToInt(d);
		currentHP -= caca;
        CreateBlood();

		if (currentHP <= 0)
			return true;
		else
			return false;
	}

	public void Heal(int amount)
	{
        CreatePHeal();
		currentHP += amount;
		if (currentHP > maxHP)
			currentHP = maxHP;
	}

	public Vector3 positionInitiale;
    public Vector3 positionCible;
    private bool estEnDeplacement = false;
    public float vitesseDeplacement = 0.5f; // Vitesse de déplacement du personnage en unités par seconde

    private void Start()
    {
        positionInitiale = transform.position;
    }

    private void Update()
    {
        if (estEnDeplacement)
        {
            float distance = vitesseDeplacement;
            transform.position = Vector3.MoveTowards(transform.position, positionCible, distance);

            if (transform.position == positionCible)
            {
                estEnDeplacement = false;
                // Effectuer l'attaque ou passer au tour suivant ici
            }
        }
    }

    public void DeplacerVersCoordonnees(float x, float y, float z)
    {
        positionCible = new Vector3(x, y, z);
        estEnDeplacement = true;
    }

    public void RevenirAPositionInitiale()
    {
        positionCible = positionInitiale;
        estEnDeplacement = true;
    }

    public ParticleSystem Blood;
    void CreateBlood()
    {
        Blood.Play();
    }
    public ParticleSystem PHeal;
    void CreatePHeal()
    {
        PHeal.Play();
    }

}
