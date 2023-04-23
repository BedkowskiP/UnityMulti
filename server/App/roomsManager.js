const { room, Room } = require('./room');
const { user } = require('./userData');

let Rooms =
{

};
let UserInRoom =
{

}


const AddRoom = async (room) =>
{
    Rooms[room.id]=room;


}
const AddUserToRoom = async (RoomName,UserID) =>
{
    UserInRoom[UserID]=RoomName;
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
    const result = {};

    for (const key in UserInRoom) {
        if (UserInRoom.hasOwnProperty(key) && UserInRoom[key] === RoomName)result[key] = UserInRoom[key];
    }
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
module.exports={AddRoom,Rooms,GetUsersInRoom,AddUserToRoom,UserInRoom,RemoveUserFromRoom,GetUserRoom}