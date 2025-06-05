using UnityEngine;

public class WeaponHandling : MonoBehaviour
{
    [SerializeField] private KeyCode quickSwitchKey;
    [SerializeField] private GameObject[] weaponGameobjects;//order matters for animations. can make into struct to also store availability of weapon.
    [SerializeField] private Animator weaponPositioningAnimator;
    private int lastWeaponEquiped = 0;
    private int currentWeaponEquiped = 0;
    //controls: quick switch. scroll wheel for scrolling through weapons. number keys for manual switching.
    private void Start()
    {
        for (int i = 0; i < weaponGameobjects.Length; i++)//have the active weapon be selected. for ease of use in testing
        {
            if (weaponGameobjects[i].activeSelf)
            {
                foreach (GameObject weapon in weaponGameobjects)
                {
                    weapon.SetActive(false);
                }
                SwitchToWeapon(i);
                break;
            }
        }
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            SwitchToWeapon(0);
        else if (Input.GetKeyDown(KeyCode.Alpha2))
            SwitchToWeapon(1);
        else if (Input.GetKeyDown(KeyCode.Alpha3))
            SwitchToWeapon(2);
        else if (Input.GetKeyDown(KeyCode.Alpha4))
            SwitchToWeapon(3);

        float scroll = Input.GetAxisRaw("Mouse ScrollWheel");
        if (scroll > 0)//up
            SwitchToWeapon(LoopWeaponInt(currentWeaponEquiped + 1));
        else if (scroll < 0)//down
            SwitchToWeapon(LoopWeaponInt(currentWeaponEquiped - 1));

        if (Input.GetKeyDown(quickSwitchKey))
            SwitchToWeapon(lastWeaponEquiped);
    }
    private int LoopWeaponInt(int desriedWeaponIntToLoop)
    {
        if (desriedWeaponIntToLoop >= weaponGameobjects.Length)
            return 0;
        else if (desriedWeaponIntToLoop < 0)
            return weaponGameobjects.Length - 1;
        return desriedWeaponIntToLoop;
    }
    private void SwitchToWeapon(int weaponInt)
    {
        if (weaponInt >= weaponGameobjects.Length)
            return;
        
        weaponPositioningAnimator.SetTrigger("Switch Weapon");
        weaponPositioningAnimator.SetInteger("Hold Weapon", weaponInt);

        weaponGameobjects[currentWeaponEquiped].SetActive(false);
        lastWeaponEquiped = currentWeaponEquiped;
        currentWeaponEquiped = weaponInt;
        weaponGameobjects[currentWeaponEquiped].SetActive(true);
    }
}
