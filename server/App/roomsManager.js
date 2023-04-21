const { room } = require('./room');
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
const AddUserToRoom = async (RoomName,User) =>
{
    UserInRoom[User.id]=RoomName;
}

const GetUsersInRoom = async (RoomName) =>
{
    const result = {};

    for (const key in UserInRoom) {
        if (UserInRoom.hasOwnProperty(key) && UserInRoom[key] === RoomName)result[key] = UserInRoom[key];
    }
    return result;
}
module.exports={AddRoom,Rooms,GetUsersInRoom,AddUserToRoom}