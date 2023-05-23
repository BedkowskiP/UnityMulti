using UnityEngine;
using Newtonsoft.Json;

public class UnityMultiSerializer<T> : MonoBehaviour
{
    public T temp;

    public virtual string Serialize(T obj)
    {
        return JsonConvert.SerializeObject(obj);
    }

    public virtual void Deserialize(string obj)
    {
        temp =  JsonConvert.DeserializeObject<T>(obj);
    }
}
