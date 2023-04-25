const userData = require('./userData');
const messageTypes = require('./messageTypes');
const usersMan = require('./usersManager')
const roomsMan = require('./roomsManager')
const Util = require("./servUtil")


const DEBUGMODE= false;




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
  message = 
  {
    Type : messageTypes.RESVALIDATION,
    Content : JSON.stringify(jsonContent),
    ErrorCode : isErrorCode,
    Timestamp: Date.now() // CHANGE LATER
  }
  console.log(message);
  if(socket!=null)socket.send(JSON.stringify(message));
}
const HandlePing = async (socket,msg) => {
    message = 
    {
      Type: messageTypes.PONG,
      Content : msg.Timestamp,
      Timestamp: Date.now()
    };
    //console.log(message);
    //if(socket!=null)socket.send(JSON.stringify(message));
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
  
  
  await roomsMan.AddRoom(content);
  let message = 
  {
    Type : messageTypes.RESCREATEROOM,
    Content : JSON.stringify(content),
    ErrorCode : isErrorCode, //0 succesfully created room //201 to create failed beacuse something
    Timestamp: Date.now() // CHANGE LATER
  }
  console.log(message);
  if(socket!=null)socket.send(JSON.stringify(message));
  
  await HandleJoinRoom(socket,jsonmsg)
}
const HandleJoinRoom = async (socket,jsonmsg) =>
{
  let isErrorCode = 0;
  let content = JSON.parse(jsonmsg.Content)
  
  roomsMan.AddUserToRoom(content.RoomName,jsonmsg.UserID);

  ///check if room id/name exist
  //console.log(roomsMan.GetUsersInRoom(content.RoomID));
  
  let jsonContent =
  {
    Settings: content,
    UserList: await(roomsMan.GetUsersInRoom(content.RoomName))
  };
  let message = 
  {
    Type : messageTypes.RESJOINROOM,
    Content : JSON.stringify(jsonContent),
    ErrorCode : isErrorCode, //0 Succesfully joined room // 201 failed to join room
    Timestamp: Date.now() // CHANGE LATER
  }
  console.log(message);
  if(socket!=null)socket.send(JSON.stringify(message));

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
  let message = 
  {
    Type : messageTypes.RESLEAVEROOM,
    Content : JSON.stringify(jsonContent),
    ErrorCode : isErrorCode, 
    Timestamp: Date.now()
  }
  await roomsMan.RemoveUserFromRoom(content.RoomName,jsonmsg.UserID);
  console.log(message)
  if(socket!=null)socket.send(JSON.stringify(message));
}
HandleLeaveRoom
//#endregion

module.exports = {
    HandleValidation,
    HandleCreateRoom,
    HandleJoinRoom,
    HandleLeaveRoom,
    HandlePing
  };