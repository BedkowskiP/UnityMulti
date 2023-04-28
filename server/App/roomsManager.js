const usersMan = require("./usersManager");
const MSG = require('./message')

let Rooms ={}; // Room objects
let UserInRoom ={}// id:roomname

const BroadcastMsgToUsersInRoom = async (RoomName,msg,except,Users) =>
{
    const users = {...Rooms[RoomName].users};//hollow copy to prevent async errors
    for (const key in users) {
        const ID = users[key].UserID;
        if(ID!=except)
        {
            //console.log(msg)
            //console.log('\x1b[36m%s\x1b[0m','sending msg above to user:',Rooms[RoomName].users[key]);
            Users[ID]?.socket?.send(msg);
        }
    }
}
const AddRoom = async (content,UserID) =>
{
    const room = new Room(content.RoomName,UserID,content.Password,content.IsPublic,content.MaxPlayers)
    room.HostChangeCallaback(async ()=>{
        console.log("changing host from : ",UserID,"->", Rooms[content.RoomName].host)
        const jsonContent =
        {
            UserID : room.host
        }
        const msg = JSON.stringify((await MSG.CreateMsg("responseHostChange",jsonContent,0,true)))
        BroadcastMsgToUsersInRoom(room.name,msg,null,usersMan.Users)
    })
    room.UserChangeCallaback(()=>{
        //console.log("Current userlist : ",GetUsersInRoom(content.RoomName),"for :",content.RoomName)
        //BroadcastMsgToUsersInRoom()
        //console.log(room.name)
    })
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
        if(Object.keys(await GetUsersInRoom(RoomName)).length <= 0)
        {
            await DeleteRoom(RoomName);//everyone left
            return 1;
        }
        else if(await GetRoomHost(RoomName)==UserID)await ChooseNewHost(RoomName);//host left but still users inside
    }

    
}
const GetUserRoomname =  async  (userID) =>
{
    if(UserInRoom[userID] === undefined)return null
    else return UserInRoom[userID]
}
const GetUsersInRoom = async (RoomName) =>
{
    result = Rooms[RoomName].users;
    return result;
}
const DeleteRoom = async (RoomName) =>
{
    delete Rooms[RoomName]
}
const GetRoomHost = async (RoomName) =>
{
    return Rooms[RoomName].host
}
const ChooseNewHost = async (RoomName) =>
{
    try
    {
        //console.log(await GetUsersInRoom(RoomName))
        Rooms[RoomName].host=Rooms[RoomName].users[0].UserID;
        //send braodcast to all users
    }
    catch
    {
        console.log('\x1b[35m%s\x1b[0m','COULDNT CHOOSE NEW HOST FOR ROOM REMOVING ROOM',RoomName);
        await DeleteRoom(RoomName)
        
    }
    
}
class Room
{
    constructor(name,host,password,isPublic,maxPlayers)
    {
        this._name=name;
        this._host=host;
        this._password=password;
        this._isPublic=isPublic;
        this._maxPlayers=maxPlayers;
        this._scene;
        this._users=[];
        this._objectList=[];
        this._onHostChange = null;
        this._onUserChange =null;
    }   
    get name(){return this._name}
    
    get users(){return this._users}
    
    get host(){return this._host;}

    set name(NAME){this._name=NAME}

    set users(USER)//if int recived removes user at index //if json object adds user to userlist
    {
        if(this._onUserChange)
            {
                this._onUserChange();
            }  
        if(typeof USER === "object")
        {
            this._users.push(USER) 
        }
        else if (typeof USER ==="number")
        {
            this._users.splice(USER,1)
        }
       
        
    }


    set host(ID)
    {
        this._host=ID
        if(this._onHostChange)
        {
            this._onHostChange();
        }
    }
    UserChangeCallaback(callback)
    {
        this._onUserChange = callback;
    }
    HostChangeCallaback(callback)
    {
        this._onHostChange = callback;
    }
}



module.exports={AddRoom,Rooms,GetUsersInRoom,AddUserToRoom,UserInRoom,RemoveUserFromRoom,Room,BroadcastMsgToUsersInRoom,GetUserRoomname,GetRoomHost,ChooseNewHost }


