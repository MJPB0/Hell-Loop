using Assets.Scripts.Interactable;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class PlayerInventory : MonoBehaviour
{
    public UnityAction OnPassiveAdd;
    public UnityAction OnPassiveLevelUp;
    public UnityAction OnWeaponPickUp;
    public UnityAction OnWeaponLevelUp;
    public UnityAction OnWeaponReplace;

    private const string PLAYER_WEAPONS_PARENT_TAG = "Player Weapons Parent";
    private const string PLAYER_PASSIVES_PARENT_TAG = "Player Passives Parent";

    [Header("Weapons")]
    [SerializeField] private int maxWeaponsCount = 3;

    [Space]
    [SerializeField] private PlayerWeapon currentWeapon;
    [SerializeField] private List<PlayerWeapon> weapons;

    [Space]
    [SerializeField] private Transform playerWeaponsParent;

    [Header("Passives")]
    [SerializeField] private int maxPassivesCount = 5;

    [Space]
    [SerializeField] private List<PlayerPassive> passives;
    [SerializeField] private List<PlayerPassive> availablePassives;

    [Space]
    [SerializeField] private Transform playerPassivesParent;

    public PlayerWeapon CurrentWeapon { get { return currentWeapon; } }

    private Player player;
    private PlayerController playerController;
    private PlayerInventory playerInventory;

    private void Start()
    {
        weapons = new List<PlayerWeapon>();
        passives = new List<PlayerPassive>();

        playerWeaponsParent = GameObject.FindGameObjectWithTag(PLAYER_WEAPONS_PARENT_TAG).transform;
        playerPassivesParent = GameObject.FindGameObjectWithTag(PLAYER_PASSIVES_PARENT_TAG).transform;

        player = GetComponent<Player>();
        playerController = GetComponent<PlayerController>();
        playerInventory = GetComponent<PlayerInventory>();

        player.OnPlayerLevelUp += () =>
        {
            if (availablePassives.Count <= 0) return;
            StartCoroutine(ChoosePassive());
        };

        OnPassiveAdd += () => IsAbleToEvolveWeapon();
        OnWeaponLevelUp += () => IsAbleToEvolveWeapon();
        OnWeaponPickUp += () => IsAbleToEvolveWeapon();
        OnWeaponReplace += () => IsAbleToEvolveWeapon();
    }

    public void AddPassive(PlayerPassive newPassive)
    {
        GameObject passiveGO = Instantiate(newPassive.gameObject, transform.position, Quaternion.identity, playerPassivesParent);
        PlayerPassive passive = passiveGO.GetComponent<PlayerPassive>();
        passives.Add(passive);
        ApplyMultiplier(passive);
    }

    private void DetectAvailablePassives()
    {
        List<PassiveName> maxedPassives = new();
        passives.ForEach(passive =>
        {
            if (passive.CurrentLevel >= passive.MaxLevel)
                maxedPassives.Add(passive.PassiveName);
        });

        if (passives.Count >= maxPassivesCount)
        {
            List<PassiveName> types = new();
            passives.ForEach(passive => types.Add(passive.PassiveName));

            availablePassives = availablePassives.Where(p => types.Contains(p.PassiveName) && !maxedPassives.Contains(p.PassiveName)).ToList();
        }
        else
            availablePassives = availablePassives.Where(p => !maxedPassives.Contains(p.PassiveName)).ToList();
    }

    private void ApplyMultiplier(PlayerPassive passive)
    {
        var value = passive.CurrentLevel * passive.InceasePerLevel;
        player.SetMultiplier(passive.MultiplierType, value);
    }

    private IEnumerator ChoosePassive()
    {
        GameplayManager.Instance.PauseGame();

        List<PlayerPassive> choices = new List<PlayerPassive>();    
        if (availablePassives.Count <= 3)
            availablePassives.ForEach(passive => choices.Add(passive));
        else
        {
            List<PlayerPassive> tmp = new List<PlayerPassive>(availablePassives);
            for (int i = 0; i < 3; i++)
            {
                int choiceIndex = Random.Range(0, tmp.Count);
                choices.Add(tmp[choiceIndex]);
                tmp.RemoveAt(choiceIndex);
            }
        }

        // TODO remove lvlup debug
        Debug.Log("==========================================");
        choices.ForEach(choice => Debug.Log($"Choice {choices.IndexOf(choice)}: {choice.name}"));
        Debug.Log("==========================================");

        playerController.IndexSelected = -1;
        playerController.SwitchActionMap(PlayerActionTypes.PassiveSelect);

        yield return new WaitUntil(() => playerController.IndexSelected != -1 && playerController.IndexSelected + 1 <= choices.Count);

        PlayerPassive chosenPassive = choices[playerController.IndexSelected];

        PlayerPassive passiveInInventory = passives.Find(p => p.PassiveName == chosenPassive.PassiveName);
        if (passiveInInventory)
        {
            passiveInInventory.LevelUp();
            ApplyMultiplier(passiveInInventory);
            OnPassiveLevelUp?.Invoke();
        }
        else
        {
            AddPassive(chosenPassive);
            ApplyMultiplier(chosenPassive);
            OnPassiveAdd?.Invoke();
        }

        DetectAvailablePassives();

        playerController.SwitchActionMap(PlayerActionTypes.Gameplay);

        GameplayManager.Instance.ResumeGame();
    }

    public void PickUpWeapon(PlayerWeapon newWeapon)
    {
        if (weapons.Count >= maxWeaponsCount)
        {
            StartCoroutine(ChooseWeaponToReplace(newWeapon));
            return;
        }

        PlayerWeapon weaponInInventory = weapons.Find(p => p.WeaponName == newWeapon.WeaponName);
        if (weaponInInventory)
        {
            weaponInInventory.LevelUp();
            OnWeaponLevelUp?.Invoke();
            Destroy(newWeapon.gameObject);
            return;
        }

        weapons.Add(newWeapon);
        newWeapon.PickUp(playerWeaponsParent);
        OnWeaponPickUp?.Invoke();
        SelectCurrentWeapon(0);
    }

    private IEnumerator ChooseWeaponToReplace(PlayerWeapon newWeapon)
    {
        GameplayManager.Instance.PauseGame();

        playerController.IndexSelected = -1;
        playerController.SwitchActionMap(PlayerActionTypes.PassiveSelect);

        yield return new WaitUntil(() => playerController.IndexSelected != -1);

        ReplaceWeapon(newWeapon, playerController.IndexSelected);
        OnWeaponReplace?.Invoke();

        playerController.SwitchActionMap(PlayerActionTypes.Gameplay);

        GameplayManager.Instance.ResumeGame();
    }

    private void ReplaceWeapon(PlayerWeapon weapon, int index)
    {
        if (index >= maxWeaponsCount || index < 0) return;

        var prevWeapon = weapons[index];
        bool isCurrentlyUsed = prevWeapon == playerInventory.CurrentWeapon;

        weapons[index] = weapon;
        weapon.PickUp(playerWeaponsParent);

        if (isCurrentlyUsed)
            SelectCurrentWeapon(index);

        Destroy(prevWeapon.gameObject);

        //Debug.Log($"Replaced weapon at slot {index + 1}");
    }

    private void SelectCurrentWeapon(int index)
    {
        if (index < 0 || index >= maxWeaponsCount) return;
        currentWeapon = weapons[index];

        foreach (PlayerWeapon weapon in weapons)
        {
            if (weapon == currentWeapon)
                weapon.SetCurrentlyUsed();
            else
                weapon.SetNotCurrentlyUsed();
        }
    }

    public void UseWeaponEffect()
    {
        if (!currentWeapon || !currentWeapon.CanAttack || GameplayManager.Instance.IsPaused) return;

        currentWeapon.Use();
    }

    public void NextWeapon()
    {
        if (currentWeapon == null || weapons.Count == 1) return;  

        int nextIndex = weapons.IndexOf(currentWeapon) == weapons.Count - 1 ? 0 : weapons.IndexOf(currentWeapon) + 1;
        
        if (weapons[nextIndex] == null) return;

        currentWeapon.SetNotCurrentlyUsed();
        currentWeapon = weapons[nextIndex];
        currentWeapon.SetCurrentlyUsed();
    }
    
    public void PreviousWeapon()
    {
        if (currentWeapon == null || weapons.Count == 1) return;

        int nextIndex = weapons.IndexOf(currentWeapon) == 0 ? weapons.Count - 1 : weapons.IndexOf(currentWeapon) - 1;

        if (weapons[nextIndex] == null) return;

        currentWeapon.SetNotCurrentlyUsed();
        currentWeapon = weapons[nextIndex];
        currentWeapon.SetCurrentlyUsed();
    }

    public void TryToEvolveWeapon()
    {
        PlayerWeapon weapon = IsAbleToEvolveWeapon();

        if (player.IsAbleToEvolveWeapon) 
            EvolveWeapon(weapon);
    }

    public PlayerWeapon IsAbleToEvolveWeapon()
    {
        List<PassiveName> passivesInUse = new();
        passives.ForEach(passive => passivesInUse.Add(passive.PassiveName));

        PlayerWeapon weaponToEvolve = null;
        weapons.ForEach(weapon =>
        {
            if (weapon.CurrentLevel >= weapon.MaxLevel && !weapon.IsEvolved && passivesInUse.Contains(weapon.PassiveNeededToEvolve))
            {
                weaponToEvolve = weapon;
                return;
            }
        });

        player.IsAbleToEvolveWeapon = weaponToEvolve != null && weaponToEvolve.EvolvedWeapon != null;
        return weaponToEvolve;
    }

    private void EvolveWeapon(PlayerWeapon weaponToEvolve)
    {
        bool isUsed = currentWeapon == weaponToEvolve;
        if (isUsed)
        {
            currentWeapon = null;
            weaponToEvolve.SetNotCurrentlyUsed();
        }

        GameObject evolvedWeaponObj = Instantiate(weaponToEvolve.EvolvedWeapon, weaponToEvolve.transform.position, new Quaternion(0, 0, 0, 0), playerWeaponsParent);
        PlayerWeapon evolvedWeapon = evolvedWeaponObj.GetComponent<PlayerWeapon>();

        int weaponToEvolveIndex = weapons.IndexOf(weaponToEvolve);
        weapons.RemoveAt(weaponToEvolveIndex);
        weapons.Insert(weaponToEvolveIndex, evolvedWeapon);

        Destroy(weaponToEvolve.gameObject);
        
        if (isUsed)
            SelectCurrentWeapon(weaponToEvolveIndex);
    }
}
