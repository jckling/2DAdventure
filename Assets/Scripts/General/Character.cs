using UnityEngine;
using UnityEngine.Events;

public class Character : MonoBehaviour, ISaveable
{
    public float maxHealth;
    public float currentHealth;
    public float maxPower;
    public float currentPower;
    public float powerRecoverSpeed;

    public float invulnerableDuration;
    public float invulnerableCounter;
    public bool invulnerable;

    public UnityEvent<Transform> OnTakeDamage;
    public UnityEvent OnDie;
    public UnityEvent<Character> onHealthChange;

    public VoidEventSO newGameEvent;

    private void NewGame()
    {
        currentHealth = maxHealth;
        currentPower = maxPower;
        onHealthChange?.Invoke(this);
    }

    private void Start()
    {
        currentHealth = maxHealth;
    }

    private void OnEnable()
    {
        newGameEvent.OnEventRaised += NewGame;
        ISaveable saveable = this;
        saveable.RegisterSaveData();
    }

    private void OnDisable()
    {
        newGameEvent.OnEventRaised -= NewGame;
        ISaveable saveable = this;
        saveable.UnRegisterSaveData();
    }

    private void Update()
    {
        if (invulnerable)
        {
            invulnerableCounter -= Time.deltaTime;
            if (invulnerableCounter <= 0)
            {
                invulnerable = false;
            }
        }

        if (currentPower < maxPower)
        {
            currentPower += powerRecoverSpeed * Time.deltaTime;
        }
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Water") && currentHealth > 0)
        {
            currentHealth = 0;
            onHealthChange?.Invoke(this);
            OnDie?.Invoke();
        }
    }

    public void TakeDamage(Attack attacker)
    {
        if (invulnerable) return;
        if (currentHealth - attacker.damage > 0)
        {
            currentHealth -= attacker.damage;
            TriggerInvulnerable();
            OnTakeDamage?.Invoke(attacker.transform);
        }
        else
        {
            currentHealth = 0;
            OnDie?.Invoke();
        }

        onHealthChange?.Invoke(this);
    }

    private void TriggerInvulnerable()
    {
        if (!invulnerable)
        {
            invulnerable = true;
            invulnerableCounter = invulnerableDuration;
        }
    }

    public void OnSlide(int cost)
    {
        currentPower -= cost;
        onHealthChange?.Invoke(this);
    }

    public DataDefination GetDataID()
    {
        return GetComponent<DataDefination>();
    }

    public void GetSaveData(Data data)
    {
        var id = GetDataID().ID;
        data.characterPosDict[id] = new SerializeVector3(transform.position);
        data.floatSaveData[id + "health"] = currentHealth;
        data.floatSaveData[id + "power"] = currentPower;
    }

    public void LoadData(Data data)
    {
        var id = GetDataID().ID;
        if (data.characterPosDict.TryGetValue(id, out var position))
        {
            transform.position = position.ToVector3();
        }

        if (data.floatSaveData.TryGetValue(id + "health", out var health))
        {
            currentHealth = health;
        }

        if (data.floatSaveData.TryGetValue(id + "power", out var power))
        {
            currentPower = power;
        }

        onHealthChange?.Invoke(this);
    }
}