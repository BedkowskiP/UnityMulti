
const messageTypes = require('./messageTypes');
const usersMan = require('./usersManager')
const roomsMan = require('./roomsManager')
const Util = require("./servUtil")
const MSG = require('./message')

const DEBUGMODE = false;  //shows outcoming msgs                           




const HandleValidation =  async (socket,jsonmsg) => 
{
  let content = JSON.parse(jsonmsg.Content)
  content.UserID = await Util.createUserId(usersMan.Users);//Creates UNIQUE ID
  if(content.Username==""||content.Username==null)content.Username=content.UserID;//if USERNAME empty GUEST LOGIN
  //else if (/^[0-9a-zA-Z_]+$/.test(content.Username))isErrorCode = 102;//invalid Username for register later
  //else if (/^[0-9a-zA-Z_]+$/.test(content.Password))isErrorCode = 103;//invalid Username for register later
  
  if(true);//check if username has valid chracters
  if(true);//check if password is correct DB
  //Check for invalid signs

  /// ErrorCode 100-200 validation
  /// 0 - succes
  /// 101 - wrong username
  /// 102 - later
  let isErrorCode = 101;
  let isValid = false;
  
  
  if(content.Username=="betek") //case sensetive only
  //check if username is valid
  //if(jsonmsg.Content.username.localeCompare("Betek")==0)// case sensitive with no accents
  {
    isValid = true
    isErrorCode = 0;
  }
  await usersMan.AddUser(content,isValid,socket);
  //-1 - username has wrong casing 0 - correct nad 1 - wrong // pewnie potem haslo i zapytanie do abzy danych
  let jsonContent =
  {
      UserID : content.UserID,
      Username : content.Username,
      Validated : isValid
  };
  let msg = JSON.stringify(await MSG.CreateMsg(messageTypes.RESVALIDATION,jsonContent,isErrorCode,DEBUGMODE))
  socket?.send(msg);
}
const HandlePing = async (socket,jsonmsg) => {
    let isErrorCode = 0;
    let msg=JSON.stringify(await MSG.CreateMsg(messageTypes.PONG,jsonmsg.Timestamp,isErrorCode,DEBUGMODE))
    socket?.send(msg);
  };

//#region ROOMS
const HandleCreateRoom = async (socket,jsonmsg) =>
{
  ///error 200-300 ROOMS
  let content = JSON.parse(jsonmsg.Content)
  let isErrorCode = 0;
  ///
  ///ADD CHECK for maxplayers/ ROOM NAME/PASSWORD/ISPUBLIC IF BAD ADD ERROR CODE
  ///

  let jsonContent =
  {
    RoomName: content.RoomName, 
    IsPublic: content.IsPublic, 
    MaxPlayers: content.MaxPlayers,
    HostID : jsonmsg.UserID
  };
  await roomsMan.AddRoom(content,jsonmsg.UserID);
  let msg = JSON.stringify((await MSG.CreateMsg(messageTypes.RESCREATEROOM,jsonContent,isErrorCode,DEBUGMODE)))
  socket?.send(msg);
  await HandleJoinRoom(socket,jsonmsg)
}
const HandleJoinRoom = async (socket,jsonmsg) =>
{
  let isErrorCode = 0;
  let content = JSON.parse(jsonmsg.Content)
  await roomsMan.AddUserToRoom(content.RoomName,jsonmsg.UserID,usersMan.Users[jsonmsg.UserID].name); 
  ///check if room id/name exist
  let jsonContent =
  {
    Settings: { RoomName:content.RoomName,HostID:await roomsMan.GetRoomHost(content.RoomName)},
    UserList: await(roomsMan.GetUsersInRoom(content.RoomName))//get list of users that joined given room   "list:[{UserID,Username},{UserID,Username}]"
  };
  
  let msg = JSON.stringify(await MSG.CreateMsg(messageTypes.RESJOINROOM,jsonContent,isErrorCode,DEBUGMODE))
  socket?.send(msg);
  //broadcast to users in room
  jsonContent =
  {
    UserID : jsonmsg.UserID,
    Username : usersMan.Users[jsonmsg.UserID].name
  }
  msg = JSON.stringify((await MSG.CreateMsg(messageTypes.USERJOIN,jsonContent,isErrorCode,DEBUGMODE)))
  await roomsMan.BroadcastMsgToUsersInRoom(content.RoomName,msg,jsonmsg.UserID,usersMan.Users);


}

const HandleLeaveRoom = async (socket,jsonmsg) =>
{

  //make checks for if user in room if room still exist so on
  let isErrorCode=0;
  let content = JSON.parse(jsonmsg.Content)
  let jsonContent =
  {
    RoomName: content.RoomName
  };
  if(1!=await roomsMan.RemoveUserFromRoom(content.RoomName,jsonmsg.UserID))//if function returns 1 it means room is about to be deleted no need to broadcast
  {
    //broadcast to users in room
    jsonContent =
    {
      UserID : jsonmsg.UserID,
      Username : usersMan.Users[jsonmsg.UserID].name
    }
    let msg = JSON.stringify((await MSG.CreateMsg(messageTypes.USERLEAVE,jsonContent,isErrorCode,DEBUGMODE)))
    await roomsMan.BroadcastMsgToUsersInRoom(content.RoomName,msg,jsonmsg.UserID,usersMan.Users);
  }
  let msg = JSON.stringify(await MSG.CreateMsg(messageTypes.RESLEAVEROOM,jsonContent,isErrorCode,DEBUGMODE))
  socket?.send(msg);
  
}
const HandleHostChange = async (socket,jsonmsg) =>
{
    //CHECK IF USER CHANGING HOST HAS PREMISSION!!!!!!!!!!!!!!!!!!!
    let content = JSON.parse(jsonmsg.Content)
    const RoomName = await roomsMan.GetUserRoomname(jsonmsg.UserID);
    if(content.UserID!=null)roomsMan.Rooms[RoomName].host=content.UserID;
    else roomsMan.ChooseNewHost(RoomName)
    console.log("hosthandler")
    
}

//#endregion

module.exports = {
    HandleValidation,
    HandleCreateRoom,
    HandleJoinRoom,
    HandleLeaveRoom,
    HandlePing,
    HandleHostChange
  };