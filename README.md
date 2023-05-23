# UnityMulti

Celem projektu jest stworzenie systemu (klient unity, docelowo unitypackage i servera) multiplayer dla unity przy wykorzystaniu websocketów.

## Instalacja

1. Pobierz pliki serwerowe: (narazie pobieranie odbywa się przy użyciu [download-directory.github.io](https://download-directory.github.io/):
  - [Pliki serverowe](https://github.com/BedkowskiP/UnityMulti/tree/main/server)
  - [Pliki klienta](https://github.com/BedkowskiP/UnityMulti/tree/main/Client/Assets/UnityMulti)
2. Instalacja serwera: //TODO by CeinyL
3. Instalacja klienta:
- Pliki klienta umieścić w folderze `../PathToUnityProject/Assets/ExampleFolderName`.

## Klient

Każdy skrypt chcący uzyskać dostęp do połączenia z serwerem powinien dziedziczyć klasę `UnityMultiNetworkingCallbacks`:
```
public class MyScript : UnityMultiNetworkingCallbacks
{
  void Start() { }
  void Update() { }
}
```

 lub:
 
 a) po starcie aplikacji ręcznie znaleźć i wyszukać obiekt `MultiNetworking` i pobrać z niego komponent `UnityMultiNetworking` (pod warunkiem, że instacja obiektu została już stworzona);
 
 b) stworzenie instancji obiektu `MultiNetworking`:
```
public class MyScript : MonoBehaviour
{
  public UnityMultiNetworking multiNetworking;
  void Start() {
    multiNetworking = UnityMultiNetworking.Instance;
  }
  void Update() { }
}
```

### Tworzenie połączenia

Przykład skryptu łączącego się z serwerem i tworzącym pokój (system automatycznie dołącza do pokoju po stworzeniu go):

```

public class CreateConnection : UnityMultiNetworkingCallbacks
{
  public string url = "url to server";
  public string username = "TestA";
  
  private void Start(){
    multiNetworking.ConnectToServer(url, username);
  }
  
  public override OnClientConnected(){
    multiNetworking.CreateRoom(new UnityMultiRoomSettings(RoomName: "RoomTestA", Password: "", SceneName: "TutorialSceneTwo"));
  }
}

```

Przykład skryptu łączącego się z serwerem i dołączającego do pokoju:


```

public class CreateConnection : UnityMultiNetworkingCallbacks
{
  public string url = "url to server";
  public string username = "TestA";
  
  private void Start(){
    multiNetworking.ConnectToServer(url, username);
  }
  
  public override OnClientConnected(){
    multiNetworking.JoinRoom(new UnityMultiRoomSettings(RoomName: "RoomTestA", Password: ""));
  }
}

```


### Tworzenie obiektu po dołączeniu do pokoju

```
public class NewSceneScript : UnityMultiNetworkingCallbacks
{
  public override void OnClientJoin(UnityMultiUser user)
    {
        base.OnClientJoin(user);
        Debug.Log("User " + user.Username + " joined the room.");
        multiNetworking.InstantiatePlayerObject("TutorialBall", new Vector3(0, 0, 0), Quaternion.identity, new Vector3(1, 1, 1), user);
    }
}
```

