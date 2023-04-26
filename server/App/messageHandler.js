
const messageTypes = require('./messageTypes');
const usersMan = require('./usersManager')
const roomsMan = require('./roomsManager')
const Util = require("./servUtil")
const MSG = require('./message')

const DEBUGMODE = false;  //shows outcoming msgs                           




const HandleValidation =  async (socket,jsonmsg) => 
{
  let content = JSON.parse(jsonmsg.Content)
  content.UserID = await Util.createUserId();
  //add unique check !!!!!!!!!!!!!!!!
  /// ErrorCode 100-200 validation
  /// 0 - succes
  /// 101 - wrong username
  /// 102 - later
  let isErrorCode = 101;
  let isValid = false;
  
  if(content.Username==""||content.Username==null)content.Username=content.UserID;
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
  let msg = JSON.stringify(MSG.CreateMsg(messageTypes.RESVALIDATION,jsonContent,isErrorCode,DEBUGMODE))
  if(socket!=null)socket.send(msg);
}
const HandlePing = async (socket,jsonmsg) => {
    let isErrorCode = 0;
    let msg=JSON.stringify(MSG.CreateMsg(messageTypes.PONG,jsonmsg.Timestamp,isErrorCode,DEBUGMODE))
    if(socket!=null)socket.send(msg);
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
  await roomsMan.AddRoom(content,jsonmsg.UserID);
  let msg = JSON.stringify((MSG.CreateMsg(messageTypes.RESCREATEROOM,content,isErrorCode,DEBUGMODE)))
  if(socket!=null)socket.send(msg);
  await HandleJoinRoom(socket,jsonmsg)
}
const HandleJoinRoom = async (socket,jsonmsg) =>
{
  let isErrorCode = 0;
  let content = JSON.parse(jsonmsg.Content)
  await roomsMan.AddUserToRoom(content.RoomName,jsonmsg.UserID,usersMan.Users[jsonmsg.UserID].name); 

  ///check if room id/name exist
  //console.log(roomsMan.GetUsersInRoom(content.RoomID));
  
  let jsonContent =
  {
    Settings: content,
    UserList: await(roomsMan.GetUsersInRoom(content.RoomName))//get list of users that joined given room   "list:[id:name,id:name]"
  };
  let msg = JSON.stringify(MSG.CreateMsg(messageTypes.RESJOINROOM,jsonContent,isErrorCode,DEBUGMODE))
  if(socket!=null)socket.send(msg);
  //broadcast to users in room
  jsonContent =
  {
    UserID : jsonmsg.UserID,
    Username : usersMan.Users[jsonmsg.UserID].name
  }
  msg = JSON.stringify((MSG.CreateMsg(messageTypes.USERJOIN,jsonContent,isErrorCode,DEBUGMODE)))
  roomsMan.BroadcastMsgToUsersInRoom(content.RoomName,msg,jsonmsg.UserID);
  //msg = JSON.stringify(MSG.CreateMsg(messageTypes.USERJOIN,jsonContent,isErrorCode,true))

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
  await roomsMan.RemoveUserFromRoom(content.RoomName,jsonmsg.UserID);
  let msg = JSON.stringify(MSG.CreateMsg(messageTypes.RESLEAVEROOM,jsonContent,isErrorCode,DEBUGMODE))
  if(socket!=null)socket.send(msg);
  //broadcast to users in room
  jsonContent =
  {
    UserID : jsonmsg.UserID,
    Username : usersMan.Users[jsonmsg.UserID].name
  }
  msg = JSON.stringify((MSG.CreateMsg(messageTypes.USERLEAVE,jsonContent,isErrorCode,DEBUGMODE)))
  await roomsMan.BroadcastMsgToUsersInRoom(content.RoomName,msg,jsonmsg.UserID);
}

//#endregion

module.exports = {
    HandleValidation,
    HandleCreateRoom,
    HandleJoinRoom,
    HandleLeaveRoom,
    HandlePing
  };