
using TMPro;
using UnityEngine;
using UnityEngine.UI;
//sort버튼 관련은 여기가 맞아
namespace menu { 
public class CharacterListSort : MonoBehaviour
{
    public Button CharactersortButton;
    public TextMeshProUGUI sortButtonText;
    public CharacterListSortUI sortui;
    public CharacterSortListUIHandler ScrollListHandlerUI;
    private void Awake()
    {
        CharactersortButton.onClick.AddListener(CharacterListsortOn);

        }
        public void UpdatesortButtonText()
        {
            switch (GameManager.instance.GamePlayCache.CharacterSortOption1)
            {
                case (int)charactersortoption1.LV:
                    sortButtonText.text = "LV";
                    break;
                case (int)charactersortoption1.name:
                    sortButtonText.text = "이름";
                    break;
                case (int)charactersortoption1.position:
                    sortButtonText.text = "포지션";
                    break;
            }
            switch (GameManager.instance.GamePlayCache.CharacterSortOption2)
            {
                case (int)charactersortoption2.Desc:
                    sortButtonText.text += "/내림차순";
                    break;
                case (int)charactersortoption2.Asc:
                    sortButtonText.text += "/오름차순";
                    break;
               
            }
        }
        public void CharacterListsortOn()
    {
        sortui.ActiveCharacterListSortUI(ScrollListHandlerUI);
    }
}
}