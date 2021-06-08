using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public enum BattleState { START, PLAYERTURN, ENEMYTURN, WON, LOST }

public class GameController : MonoBehaviour
{

    private AnimationStateController playerAnim;
    private AnimationStateController enemyAnim;
    
    private AudioSource[] endGameSFX;
    private AudioSource lossSFX;
    private AudioSource victorySFX;
    private AudioSource battleMusic;

    public GameObject playerPrefab;
    public GameObject enemyPrefab;
    public GameObject healPrefab;

    private ParticleSystem healParticle;
    
    private Unit playerUnit;
    private Unit enemyUnit;

    
    private Animator animator;
    // private AudioSource healSFX;

    [SerializeField] private Button showAttacksButton;
    [SerializeField] private Button attackButton;
    [SerializeField] private Button atkCritButton;
    [SerializeField] private Button atkComboButton;
    [SerializeField] private Button healButton;
    [SerializeField] private Button pauseButton;

    [SerializeField] private TextMeshProUGUI result;

    public GameObject battleMenuUI;

    private bool isCritUsed = false;
    private int turnCount = 0;
    
    public BattleHUD playerHUD;
    public BattleHUD enemyHUD;

    public BattleState state;
    // Start is called before the first frame update
    void Start()
    {
        state = BattleState.START;
        SetupBattle();
        endGameSFX = GetComponents<AudioSource>();
        lossSFX = endGameSFX[0];
        victorySFX = endGameSFX[1];
        battleMusic = endGameSFX[2];
    }

    void SetupBattle()
    {
        GameObject playerGO = Instantiate(playerPrefab);
        playerUnit = playerGO.GetComponent<Unit>();
        playerAnim = playerGO.GetComponent<AnimationStateController>();


        GameObject enemyGO = Instantiate(enemyPrefab);
        enemyUnit = enemyGO.GetComponent<Unit>();
        enemyAnim = enemyGO.GetComponent<AnimationStateController>();

        playerHUD.SetHUD(playerUnit);
        enemyHUD.SetHUD(enemyUnit);
        
        AtkButtonsVisibility(false);
        ButtonsInteract(false);

        // primo turno casuale
        System.Random rnd = new System.Random();
        int num = rnd.Next(2);
        if (num == 0)
        {
            state = BattleState.PLAYERTURN;
            PlayerTurn();
        }
        else
        {
            state = BattleState.ENEMYTURN;
            StartCoroutine(EnemyTurn());
        }
    }

    IEnumerator PlayerAttack(string attackType)
    {
        AtkButtonsVisibility(false);
        ButtonsInteract(false);
        bool isDead = false;

        if (attackType == "normal")
        {
            playerAnim.Attack("player");
            yield return new WaitForSeconds(0.4f);
            isDead = enemyUnit.TakeDamage(playerUnit.damage);
        }
        else if (attackType == "crit")
        {
            playerAnim.CritAttack();
            yield return new WaitForSeconds(0.4f);
            isDead = enemyUnit.TakeDamage(playerUnit.damage*2);
            isCritUsed = true;
        }
        else if (attackType == "combo")
        {
            playerAnim.ComboAttack();
            yield return new WaitForSeconds(0.6f);
            isDead = enemyUnit.TakeDamage(playerUnit.damage*3);
        }

        enemyHUD.setHP(enemyUnit.currentHP);
        
        //yield return new WaitForSeconds(1f);

        if (isDead)
        {
            enemyAnim.Death("enemy");
            //yield return new WaitForSeconds(1.2f);
            yield return new WaitForSeconds(enemyAnim.GetAnimator().GetCurrentAnimatorStateInfo(0).length);
            state = BattleState.WON;
            EndBattle();
        }
        else
        {
            enemyAnim.GetHit("enemy");
            state = BattleState.ENEMYTURN;
            StartCoroutine(EnemyTurn());
        }
    }

    IEnumerator EnemyTurn()
    {
        playerHUD.setMarkerVisibility(false);
        enemyHUD.setMarkerVisibility(true);
        
        yield return new WaitForSeconds(1f);
        
        enemyAnim.Attack("enemy");
        yield return new WaitForSeconds(0.4f);

        bool isDead = playerUnit.TakeDamage(enemyUnit.damage);
        playerHUD.setHP(playerUnit.currentHP);

        if (isDead)
        {
            playerAnim.Death("player");
            //yield return new WaitForSeconds(1.2f);
            yield return new WaitForSeconds(playerAnim.GetAnimator().GetCurrentAnimatorStateInfo(0).length+0.5f);
            state = BattleState.LOST;
            EndBattle();
        }
        else
        {
            playerAnim.GetHit("player");
            state = BattleState.PLAYERTURN;
            PlayerTurn();
        }
    }
    
    void EndBattle()
    {
        battleMusic.Stop();
        battleMenuUI.SetActive(true);
        if (state == BattleState.WON)
        {
            result.text = "YOU HAVE WON!";
            victorySFX.Play();
        } else if (state == BattleState.LOST)
        {
            result.text = "YOU WERE DEFEATED";
            lossSFX.Play();
        }
    }
    void PlayerTurn()
    {
        ButtonsInteract(true);

        turnCount++;
        playerHUD.setMarkerVisibility(true);
        enemyHUD.setMarkerVisibility(false);
    }

    public void OnAttackButton()
    {
        if (state != BattleState.PLAYERTURN)
            return;
        ShowAttackButtons();
    }

    public void OnNormalAttackButton()
    {
        StartCoroutine(PlayerAttack("normal"));
    }

    public void OnCritAttackButton()
    {
        StartCoroutine(PlayerAttack("crit"));
    }

    public void OnComboAttackButton()
    {
        StartCoroutine(PlayerAttack("combo"));
    }
    
    void ShowAttackButtons()
    {
        if (turnCount >= 3 && isCritUsed == false)
            atkCritButton.interactable = true;
        else
            atkCritButton.interactable = false;

        if (enemyUnit.currentHP <= (enemyUnit.maxHP) / 2)
            atkComboButton.interactable = true;
        else
            atkComboButton.interactable = false;
        
        AtkButtonsVisibility(true);
    }
    
    public void OnHealButton()
    {
        if (state != BattleState.PLAYERTURN)
            return;
        StartCoroutine(PlayerHeal());
    }
    
    IEnumerator PlayerHeal()
    {
        Instantiate(healPrefab);
        ButtonsInteract(false);

        playerUnit.Heal(playerUnit.healAmount);
        playerHUD.setHP(playerUnit.currentHP);
        
        AtkButtonsVisibility(false);
        
        yield return new WaitForSeconds(1f);
        
        state = BattleState.ENEMYTURN;
        StartCoroutine(EnemyTurn());
    }

    void ButtonsInteract(bool value)
    {
        if (value)
        {
            showAttacksButton.interactable = true;
            attackButton.interactable = true;
            atkCritButton.interactable = true;
            atkComboButton.interactable = true;
            healButton.interactable = true;
        }
        else
        {
            showAttacksButton.interactable = false;
            attackButton.interactable = false;
            healButton.interactable = false;
            atkCritButton.interactable = false;
            atkComboButton.interactable = false;
        }
    }

    void AtkButtonsVisibility(bool value)
    {
        if (value)
        {
            attackButton.gameObject.SetActive(true);
            atkCritButton.gameObject.SetActive(true);
            atkComboButton.gameObject.SetActive(true);
        }
        else
        {
            attackButton.gameObject.SetActive(false);
            atkCritButton.gameObject.SetActive(false);
            atkComboButton.gameObject.SetActive(false);
        }
    }

    public void ReloadScene()
    {
        SceneManager.LoadScene("Game");
    }
}
