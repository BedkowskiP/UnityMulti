const usersMan = require("./usersManager");
const MSG = require('./message')

let Rooms ={}; // Room objects
let UserInRoom ={}// id:roomname

const BroadcastMsgToUsersInRoom = async (RoomName,msg,except,Users) =>
{
    const users = {...Rooms[RoomName].users};//hollow copy to prevent async errors
    for (const key in users) {
        const ID = users[key].UserID;
        if(ID!=except){usersMan.Users[ID]?.socket?.send(msg);console.log(msg,'sent to',usersMan.Users[ID].id);}
    }
}
const AddRoom = async (content,UserID) =>
{
    const room = new Room(content.RoomName,UserID,content.Password,content.IsPublic,content.MaxPlayers,content.SceneName)
    Rooms[room.name]=room;
}
const AddUserToRoom = async (RoomName,userID,username) =>
{
    UserInRoom[userID]=RoomName;      
    Rooms[RoomName].users={  UserID: userID,Username: username };
}
const RemoveUserFromRoom = async (RoomName,UserID) =>
{
    if(RoomName!=null)
    {
        //console.log("removing "+ UserID+" from room: "+RoomName);
        delete UserInRoom[UserID] //user left but still users inside
        //Rooms[RoomName].users = Rooms[RoomName].users.filter(item => item.UserID !== UserID); //filtering userlist array to includes element != userid
        Rooms[RoomName].users = Rooms[RoomName].users.findIndex(user => user.UserID === UserID);
        if(Object.keys(Rooms[RoomName].users).length <= 0)
        {
            await DeleteRoom(RoomName);//everyone left
            return 1;
        }
        else if(await Rooms[RoomName].host==UserID)await ChooseNewHost(RoomName);//host left but still users inside
    }


}
const GetUserRoomname =  async  (userID) =>
{
    if(UserInRoom[userID] === undefined)return null
    else return UserInRoom[userID]
}

const DeleteRoom = async (RoomName,ERR) =>
{
    if(Rooms[RoomName].users.length!=0)
    {
       await OnDeletRoom(RoomName,ERR)
    } 
    delete Rooms[RoomName]
}
const OnDeletRoom =async (RoomName,ERR)=>
{
    const msg = JSON.stringify((MSG.CreateMsg("responseHostChange",{},ERR,false)))
    BroadcastMsgToUsersInRoom(RoomName,msg,null,usersMan.Users)
}
const ChooseNewHost = async (RoomName) =>
{
    try
    {
        Rooms[RoomName].host=Rooms[RoomName].users[0].UserID;
    }
    catch
    {
        await DeleteRoom(RoomName,204)//204 deleting room coudlnt choose new host
    }
    
}
class Room
{
    constructor(name,host,password,isPublic,maxPlayers,sceneName)
    {
        this._name=name;
        this._host=host;
        this._password=password;
        this._isPublic=isPublic;
        this._maxPlayers=maxPlayers;
        this._sceneName=sceneName;
        this._users=[];
        this._objectList=[];
        this._onHostChange = null;
        this._objectNum = 0;
    }   
    get name(){return this._name}
    
    get users(){return this._users}
    
    get host(){return this._host;}

    get sceneName(){return this._sceneName;}

    set name(NAME){this._name=NAME}

    set users(USER)//if int recived removes user at index //if json object adds user to userlist LATER CHANGE TO NORMAL METHODS
    { 
        if(typeof USER === "object")
        {
            this._users.push(USER) 
        }
        else if (typeof USER ==="number")
        {
            this._users.splice(USER,1)
            //REMOVE OBJECTS ON USER LEAVE !!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        }  
    }
    set host(ID)
    {
        /*(this._onHostChange)
        {
             to add callbacks to created rooms outside of class
        }*/
        this.onHostChange(ID);
        this._host=ID
        
    }   
    //set sceneName(SCENENAME){this._sceneName=SCENENAME}
    

    onHostChange(NEWHOST)
    {
        console.log("changing host from : ",this.host,"->", NEWHOST)
        const jsonContent =
        {
            UserID : NEWHOST
        }
        const msg = JSON.stringify((MSG.CreateMsg("responseHostChange",jsonContent,0,false)))
        BroadcastMsgToUsersInRoom(this._name,msg,null,usersMan.Users)
    }
    
    SetSceneName(SCENE,USERID)
    {
        if(USERID==this._host)
        {
            this._SceneName=SCENE
            this.onSceneChange(SCENE);
            return 0;
        }
        else{return 207}//Didnt change scene beacuse no HOST privilage 
    }
    onSceneChange(SCENE)
    {
        ///REMOVE ALL THE OBJECTS
        const jsonContent =
        {
            SceneName : SCENE
        }
        const msg = JSON.stringify((MSG.CreateMsg("responseSceneChange",jsonContent,0,false)))
        BroadcastMsgToUsersInRoom(this._name,msg,null,usersMan.Users)
    }
    ///
    /// OBJECTLIST
    ///
    GetObjectList(){}
    GetObjectFromList(){return this._objectList}
    AddObject(CONTENT,OWNER)
    {
        let OBJECT=new ObjectUnity
        (
            CONTENT.PrefabName,OWNER,
            CONTENT.PosX,CONTENT.PosY,CONTENT.PosZ,
            CONTENT.RotX,CONTENT.RotY,CONTENT.RotZ,CONTENT.RotW,
            CONTENT.ScalX,CONTENT.ScalY,CONTENT.ScalZ
        );
        this._objectList[this._objectNum++]=OBJECT
        console.log("Added",OBJECT)
        if(false){}//problem
        else {
            return {ErrorCode:0,ObjectID:this._objectNum-1};}
    }
}

class ObjectUnity
{
    static PositionUnity = class
    {
        constructor(X,Y,Z)
        {
            this.x=X
            this.y=Y
            this.z=Z
        }

        
    }
    static RotationUnity = class
    {
        constructor(X,Y,Z,W)
        {
            this.x=X
            this.y=Y
            this.z=Z
            this.z=W
        }
    }
    static ScaleUnity = class
    {
        constructor(X,Y,Z)
        {
            this.x=X
            this.y=Y
            this.z=Z
        }
        
    }
    constructor(PREFAB,OWNER,POSX,POSY,POSZ,ROTX,ROTY,ROTZ,ROTW,SCAX,SCAY,SCAZ)
    {
        this._pos=new ObjectUnity.PositionUnity(POSX,POSY,POSZ)
        this._rot=new ObjectUnity.RotationUnity(ROTX,ROTY,ROTZ,ROTW)
        this._sca=new ObjectUnity.ScaleUnity(SCAX,SCAY,SCAZ)
        this._owner=OWNER;
        this._prefab = PREFAB;
    }
    get pos()
    {
        return {x:this._pos.x,  y:this._pos.y,  z:this._pos.z}
    }
    get rot()
    {
        return {x:this._rot.x,  y:this._rot.y,  z:this._rot.z,  w:this._rot.w}
    }
    get sca()
    {
        return {x:this._sca.x,  y:this._sca.y,  z:this._sca.z}
    }

    
    
    
    
}


module.exports={AddRoom,Rooms,AddUserToRoom,UserInRoom,RemoveUserFromRoom,Room,BroadcastMsgToUsersInRoom,GetUserRoomname,ChooseNewHost}


