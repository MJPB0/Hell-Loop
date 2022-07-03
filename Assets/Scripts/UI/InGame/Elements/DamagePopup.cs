using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamagePopup : MonoBehaviour
{
    private const string DAMAGE_POPUP_PARENT_TAG = "Damage Numbers Parent";

    private TextMeshPro textMesh;

    private float disappearTimer;

    private Color textColor;

    private float minRangeX = -2f;
    private float maxRangeX = 2f;

    private float moveSpeedY = 2f;

    private float disappearSpeed = 3f;

    private void Awake()
    {
        textMesh = transform.GetComponent<TextMeshPro>();
        textColor = textMesh.color;
        textMesh.fontSize = 4;
        disappearTimer = 0.5f;
    }

    void Update()
    {
        transform.position += new Vector3(Random.Range(minRangeX, maxRangeX), moveSpeedY) * Time.deltaTime;

        disappearTimer -= Time.deltaTime;
        if (disappearTimer < 0)
        {
            textColor.a -= disappearSpeed * Time.deltaTime;
            textMesh.color = textColor;

            if (textColor.a < 0)
                Destroy(gameObject);
        }
    }

    public static DamagePopup Create(Vector3 position, string damage, DamagePopupOwner popupOwner)
    {
        Transform popupParent = GameObject.FindGameObjectWithTag(DAMAGE_POPUP_PARENT_TAG).transform;
        Transform damagePopupTransform = Instantiate(GameAssets.Instance.pfDamagePopup, position, Quaternion.identity, popupParent);
        
        DamagePopup damagePopup = damagePopupTransform.GetComponent<DamagePopup>();
        damagePopup.Setup(damage, popupOwner);
        
        return damagePopup;
    }

    public void Setup(string damage, DamagePopupOwner popupOwner)
    {
        textMesh.SetText(damage.ToString());

        switch (popupOwner)
        {
            case DamagePopupOwner.PLAYER_HIT:
                textColor = new Color(0.501f, 0.023f, 0.023f);
                break;
            case DamagePopupOwner.ENEMY_HIT:
                textColor = new Color(0.807f, 0.819f, 0.066f);
                break;
            case DamagePopupOwner.ENEMY_CRITICAL_HIT:
                textColor = new Color(0.968f, 0.596f, 0.066f);
                break;
            case DamagePopupOwner.PLAYER_HEAL:
                textColor = new Color(0.05f, 0.749f, 0.039f);
                break;
            case DamagePopupOwner.ENEMY_HEAL:
                textColor = new Color(0.05f, 0.749f, 0.039f);
                break;
        }

        textMesh.color = textColor;
    }
}
