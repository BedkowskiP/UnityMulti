const { User } = require("./usersManager");

let Rooms =
{

};
let UserInRoom =
{

}// id:username

const BroadcastMsgToUsersInRoom = async (RoomName,msg,except) =>
{
    for (let key in Rooms[RoomName].users)
    {   
        if(Rooms[RoomName].users[key].UserID!=except)
        {
            //console.log('\x1b[36m%s\x1b[0m','sending msg above to user:',Rooms[RoomName].users[key]);
            if(Rooms[RoomName].users[key].socket!=null)
            {
                
                Rooms[RoomName].users[key].socket.send(msg);
            }
        }   
    }
    for (let id in UserInRoom)
    {
        //console.log('\x1b[36m%s\x1b[0m',UserInRoom);
        //console.log('\x1b[35m%s\x1b[0m',UserInRoom[users].roomname);
    }
    //console.log(Users);
}
const AddRoom = async (content,UserID) =>
{
    let room = new Room(content.RoomName,UserID,content.Password,content.IsPublic,content.MaxPlayers)
    Rooms[room.name]=room;
}
const AddUserToRoom = async (RoomName,userID,username) =>
{
    UserInRoom[userID]=RoomName;      
    Rooms[RoomName].users.push({  UserID: userID,Username: username });
}
const RemoveUserFromRoom = async (RoomName,UserID) =>
{
    console.log("removing "+ UserID+" from room: "+RoomName);
    delete UserInRoom[UserID] //user left but still users inside
    Rooms[RoomName].users = Rooms[RoomName].users.filter(item => item.UserID !== UserID); //filtering userlist array to includes element != userid
    if(Object.keys(await GetUsersInRoom(RoomName)).length <= 0)await DeleteRoom(RoomName);//everyone left
    else if(Rooms[RoomName].host==UserID)await ChooseNewHost(RoomName);//host left but still users inside
}

const GetUsersInRoom = async (RoomName) =>
{
    result = Rooms[RoomName].users;
    return result;
}
const DeleteRoom = async (RoomName) =>
{
    console.log('\x1b[01m%s\x1b[0m',"DOESNT REMOVE ROOM FULLY")
    

}
const ChooseNewHost = async (RoomName) =>
{
    try
    {
        Rooms[RoomName].host=Rooms[RoomName].users[0].UserID;
    }
    catch
    {
        console.log('\x1b[01m%s\x1b[0m','COULDNT CHOOSE NEW HOST FOR ROOM REMOVING ROOM',RoomName);
        DeleteRoom(RoomName)
    }
    
}
class Room
{
    constructor(name,host,password,isPublic,maxPlayers)
    {
        this.name=name;
        this.host=host
        this.password=password
        this.isPublic=isPublic
        this.maxPlayers=maxPlayers
        this.users=[]
    } 
}



module.exports={AddRoom,Rooms,GetUsersInRoom,AddUserToRoom,UserInRoom,RemoveUserFromRoom,Room,BroadcastMsgToUsersInRoom}


