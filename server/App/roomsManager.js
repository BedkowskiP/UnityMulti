let Rooms =
{

};
let UserInRoom =
{

}


const AddRoom = async (content) =>
{
    let room = new Room(content.RoomName,content.Userid,content.Password,content.IsPublic,content.MaxPlayers)
    Rooms[room.name]=room;
}
const AddUserToRoom = async (RoomName,userID,username) =>
{
    UserInRoom[userID]=RoomName;
    Rooms[RoomName].users.push({ Username: username, UserID: userID });
    //console.log(UserInRoom);
}
const RemoveUserFromRoom = async (RoomName,UserID) =>
{
    console.log("removing "+ UserID+" from room: "+RoomName);
    delete UserInRoom[UserID] //user left but still users inside
    if(Object.keys(await GetUsersInRoom(RoomName)).length <= 0)DeleteRoom();//everyone left
    //FIX LATER Cannot read properties of undefined (reading 'host')
    //else if(Rooms[RoomName].host===UserID)ChooseNewHost();//host left but still users inside
}

const GetUsersInRoom = async (RoomName) =>
{
    result = Rooms[RoomName].users;
    return result;
}
const GetUserRoom = async () =>
{
    console.error("to do GetUserRoom");
}
const DeleteRoom = async () =>
{
    console.error("to do DeleteRoom");
    console.log("DELETING ROOM");
}
const ChooseNewHost = async () =>
{
    console.error("to do ChooseNewHost");
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



module.exports={AddRoom,Rooms,GetUsersInRoom,AddUserToRoom,UserInRoom,RemoveUserFromRoom,GetUserRoom,Room}


