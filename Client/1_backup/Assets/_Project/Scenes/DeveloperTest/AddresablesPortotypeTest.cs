using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System.Collections;
public class AddresablesPortotypeTest : MonoBehaviour
{

    private AsyncOperationHandle<GameObject> handle;
    private GameObject instance;
    IEnumerator Start()
    {
        string characterid = "p01";

        CharacterContentMapping.Content content =
            CharacterContentMapping.Get(characterid);
         handle = Addressables.InstantiateAsync(content.PrefabAddress);
        yield return handle;

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            Debug.Log("Addressables Load Success : " + handle.Result.name);
        }
        else
        {
            Debug.LogError("Addressables Load Failed");

        }
        instance=Instantiate(handle.Result);
        Addressables.Release(handle);
        Debug.Log("Instantiate Success : " + instance.name);

    }
    private void OnDisable()
    {
        if (instance != null)
            Destroy(instance);

        if (handle.IsValid())
            Addressables.Release(handle);
    }
}
