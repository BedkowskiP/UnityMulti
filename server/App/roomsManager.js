const usersMan = require("./usersManager");
const MSG = require('./message')

let Rooms ={}; // Room objects

const BroadcastMsgToUsersInRoom = async (RoomName,msg,except,Users) =>
{
    if(RoomName)
    {
        //console.log(Rooms[RoomName])
        const users = {...Rooms[RoomName].users};//hollow copy to prevent async errors
        for (const key in users) {
            const ID = users[key].UserID;
            if(ID!=except){
                usersMan.Users[ID]?.socket?.send(msg);
                console.log(msg,'\x1b[36m sent to',usersMan.Users[ID].id,'\x1b[0m');
            }
        }
    }

}

const AddRoom = async (content,UserID) =>
{
    const room = new Room(content.RoomName,UserID,content.Password,content.IsPublic,content.MaxPlayers,content.SceneName)
    Rooms[room.name]=room;
}
const AddUserToRoom = async (RoomName,userID,username) =>
{
    usersMan.Users[userID].inRoom = RoomName
    Rooms[RoomName].users={  UserID: userID,Username: username };
}
const RemoveUserFromRoom = async (UserID) =>
{
    
    const RoomName = usersMan.Users[UserID].inRoom;

    if(RoomName!=null)
    {
        usersMan.Users[UserID].inRoom=null;
        Rooms[RoomName].users = Rooms[RoomName].users.findIndex(user => user.UserID === UserID);
        if(Object.keys(Rooms[RoomName].users).length <= 0)
        {
            await DeleteRoom(RoomName);//everyone left
            return 1;
        }
        else if(await Rooms[RoomName].host==UserID)await ChooseNewHost(RoomName);//host left but still users inside
    }


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
    const msg = JSON.stringify((MSG.CreateMsg("responseHostChange",{},ERR,false,true)))
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
    get objectList(){return this._objectList;}

    set name(NAME){this._name=NAME}

    set users(USER)//if int recived removes user at index //if json object adds user to userlist LATER CHANGE TO NORMAL METHODS
    { 
        if(typeof USER === "object")
        {
            this._users.push(USER) 
            
        }
        else if (typeof USER ==="number")
        {

            //console.log(this._users[USER].UserID)
            const ID=this._users[USER].UserID
            this._users.splice(USER,1)
            //delete olbject of user
            for (let i = this._objectList.length - 1; i >= 0; i--) {
                //console.log(ID," : ",this._objectList[i].owner)
                if (this._objectList[i].owner === ID) {
                   
                    this._objectList.splice(i, 1);
                }
            }
            //OnUserLeaveRoom()
            
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
    OnUserLeaveRoom()
    {
        

    }
    OnUserJoin()
    {
        const LIST = {...this._objectList};//hollow copy to prevent async errors
        //console.log(LIST)
        /// sending list of objects 
    }
    onHostChange(NEWHOST)
    {
        console.log("changing host from : ",this.host,"->", NEWHOST)
        const jsonContent =
        {
            UserID : NEWHOST
        }
        const msg = JSON.stringify((MSG.CreateMsg("responseHostChange",jsonContent,0,false,true)))
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
        else{return {ErrorCode:207}}//Didnt change scene beacuse no HOST privilage 
    }
    onSceneChange(SCENE)
    {
        ///REMOVE ALL THE OBJECTS
        const jsonContent =
        {
            SceneName : SCENE
        }
        const msg = JSON.stringify((MSG.CreateMsg("responseSceneChange",jsonContent,0,false,true)))
        BroadcastMsgToUsersInRoom(this._name,msg,null,usersMan.Users)
    }
    ///
    /// OBJECTLIST
    ///
    GetObjectFromList(){return this._objectList}
    AddObject(CONTENT,OWNER)
    {
        
        if(OWNER!=this._host)
        {  
            return {ErrorCode:208,ObjectID:null};//not a host
        }
        else 
        { 
            let OBJECT=new ObjectUnity(CONTENT.PrefabName,  CONTENT.Owner,  CONTENT.Position,   CONTENT.Rotation,   CONTENT.Scale,this._objectNum);
            this._objectList[this._objectNum]=OBJECT
            //console.log("Added",OBJECT)                 
            return {ErrorCode:0,ObjectID:this._objectNum++};
        }
            
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
            this.w=W
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
    constructor(PREFAB,OWNER,POS,ROT,SCA,ID)
    {
        this._id=ID;
        this._pos=new ObjectUnity.PositionUnity(POS.x,POS.y,POS.z)
        this._rot=new ObjectUnity.RotationUnity(ROT.x,ROT.y,ROT.z,ROT.w)
        this._sca=new ObjectUnity.ScaleUnity(SCA.x,SCA.y,SCA.z)
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
    get prefab()
    {
        return this._prefab
    }
    
    get owner()
    {
        return this._owner
    }
    set pos(POS)
    {
        this._pos.x=POS.x
        this._pos.y=POS.y
        this._pos.z=POS.z
    }
    set rot(ROT)
    {
        this._rot.x=ROT.x
        this._rot.y=ROT.y
        this._rot.z=ROT.z
        this._rot.w=ROT.w
    }
    set sca(SCA)
    {
        this._sca.x=SCA.x
        this._sca.y=SCA.y
        this._sca.z=SCA.z
    }
    get id()
    {return this._id}
    Update(arg1,content)
    {
        if(arg1==7)
        {
            this.pos=content.Position
            this.rot=content.Rotation
            this.sca=content.Scale
            //console.log(content.Position)
        }
        return 0;///ERRORCHECK
        
    }
    
}


module.exports={AddRoom,Rooms,AddUserToRoom,RemoveUserFromRoom,Room,BroadcastMsgToUsersInRoom,ChooseNewHost}


