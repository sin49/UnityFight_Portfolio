using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterUpgradeModeling : MonoBehaviour
{
    GameObject upgradeModel;
    List<GameObject> modelLibrary;
    public Transform ModelTransform;
    
    public void CreateModel(GameObject obj)
    {
        if (upgradeModel != null && upgradeModel == obj)
            return;
        else if(upgradeModel != null && upgradeModel != obj)
        {
            upgradeModel.SetActive(false);
            upgradeModel = null;
        }
        if (modelLibrary == null)
            modelLibrary = new List<GameObject>();

        if (modelLibrary.Contains(obj))
        {
            upgradeModel = obj;
            upgradeModel.SetActive(true);
        }
        else
        {
            upgradeModel = Instantiate(obj, ModelTransform);
            if (upgradeModel.GetComponent<Rigidbody>() != null)
                Destroy(upgradeModel.GetComponent<Rigidbody>());
            if (upgradeModel.GetComponent<GameCharacter>() != null)
                Destroy(upgradeModel.GetComponent<GameCharacter>());
            upgradeModel.transform.position = ModelTransform.position;
            modelLibrary.Add(upgradeModel);
        }
    }
}
