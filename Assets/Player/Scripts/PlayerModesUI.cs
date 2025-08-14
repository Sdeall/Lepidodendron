using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerModesUI : MonoBehaviour
{
    [SerializeField] int avalibleModes;
    [SerializeField] TMP_Text[] _texts;

    PlayerModes playerModes;
    GameObject[] arrows;
    int selectedMode;

    void Awake()
    {
        SetComponents();
    }

    void SetComponents()
    {
        arrows = new GameObject[_texts.Length];

        playerModes = GetComponent<PlayerModes>();

        for (int i = 0; i < _texts.Length; i++)
        {
            arrows[i] = _texts[i].GetComponentInChildren<RawImage>().gameObject;
        }

        UpdateArrows();
        SetMode(selectedMode);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
        {
            if (selectedMode > 0)
                selectedMode--;
            else
                selectedMode = avalibleModes;

            UpdateArrows();

        }
        else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
        {
            if (selectedMode < avalibleModes)
                selectedMode++;
            else
                selectedMode = 0;

            UpdateArrows();
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            SetMode(selectedMode);
        }
    }

    void UpdateArrows()
    {
        for (int i = 0; i < arrows.Length; i++)
            {
                if (i == selectedMode)
                    arrows[i].SetActive(true);
                else
                    arrows[i].SetActive(false);
            }
    }

    void SetMode(int mode)
    {
        for (int i = 0; i < _texts.Length; i++)
        {
            if (i == mode)
                _texts[i].color = Color.yellow;
            else
                _texts[i].color = Color.white;
        }
        switch (mode)
        {
            case 0:
                playerModes.SetMode(ControlMode.Walk);
                break;
            case 1:
                playerModes.SetMode(ControlMode.Interact);
                break;
            case 2:
                playerModes.SetMode(ControlMode.Inventory);
                break;
            case 3:
                playerModes.SetMode(ControlMode.Weapon);
                break;
            default:
                break;
        }
    }
}
