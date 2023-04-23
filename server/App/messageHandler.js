const userData = require('./userData');
const messageTypes = require('./messageTypes');
const { Room } = require('./room');
const roomsMan = require('./roomsManager')
const Util = require("./servUtil")


const DEBUGMODE= false;

let message = {
    Type: '',
    Content: ''
};


const HandleValidation =  async (socket,jsonmsg) => 
{
  let content = JSON.parse(jsonmsg.Content)
  let id = await Util.createUserId();
  let usrn= "";
  /// ErrorCode 100-200 validation
  /// 0 - succes
  /// 101 - wrong username
  /// 102 - later
  let isErrorCode = 1;
  let isValid = false;
  
  if(content.Username==""||content.Username==null)usrn=id;
  else usrn=content.Username;

  if(content.Username=="betek") //case sensetive only
  //if(jsonmsg.Content.username.localeCompare("Betek")==0)// case sensitive with no accents
  {
    isValid = true
    isErrorCode = 0;
  } 
  //-1 - username has wrong casing 0 - correct nad 1 - wrong // pewnie potem haslo i zapytanie do abzy danych
  let jsonContent =
  {
      UserID : id,
      Username : usrn,
      Validated : isValid
  };
  message = 
  {
    Type : messageTypes.RESVALIDATION,
    Content : JSON.stringify(jsonContent),
    ErrorCode : isErrorCode,
    Timestamp: Date.now() // CHANGE LATER
  }
  let user = {id:id,name:usrn,socket:socket,valid:isValid}
    /*
  user.id=id;
  user.name=usrn;
  user.socket=socket;
  user.valid=isValid;*/
  //console.log(message);
  if(socket!=null)socket.send(JSON.stringify(message));
  return user;
}
const HandlePing = async (socket,msg) => {
    message = 
    {
      Type: messageTypes.PONG,
      Content : msg.Timestamp,
      Timestamp: Date.now()
    };
    //console.log(message);
    if(socket!=null)socket.send(JSON.stringify(message));
  };

//#region ROOMS
const HandleCreateRoom = async (socket,jsonmsg,Userid) =>
{
  ///error 200-300 ROOMS
  let content = JSON.parse(jsonmsg.Content)
  let isErrorCode = 0;
  ///
  ///ADD CHECK for maxplayers/ ROOM NAME/PASSWORD/ISPUBLIC IF BAD ADD ERROR CODE
  ///
  let room = new Room(content.RoomName,Userid,content.Password,content.IsPublic,content.MaxPlayers)
  await roomsMan.AddRoom(room);
  let jsonContent =
  {
    RoomName:content.RoomName,
  };
  message = 
  {
    Type : messageTypes.RESCREATEROOM,
    Content : JSON.stringify(jsonContent),
    ErrorCode : isErrorCode, //0 succesfully created room //201 to create failed beacuse something
    Timestamp: Date.now() // CHANGE LATER
  }
  //console.log(message);
  if(socket!=null)socket.send(JSON.stringify(message));
  
  await HandleJoinRoom(socket,jsonmsg,Userid)
}
const HandleJoinRoom = async (socket,jsonmsg,Userid) =>
{
  let isErrorCode = 0;
  let content = JSON.parse(jsonmsg.Content)
  
  roomsMan.AddUserToRoom(content.RoomName,Userid);
  ///check if room id/name exist
  //console.log(roomsMan.GetUsersInRoom(content.RoomID));
  
  let jsonContent =
  {
    RoomName: content.RoomName,
    UserList: await(roomsMan.GetUsersInRoom(content.RoomName))
  };
  message = 
  {
    Type : messageTypes.RESJOINROOM,
    Content : JSON.stringify(jsonContent),
    ErrorCode : isErrorCode, //0 Succesfully joined room // 201 failed to join room
    Timestamp: Date.now() // CHANGE LATER
  }
  //console.log(message);
  if(socket!=null)socket.send(JSON.stringify(message));

}
const HandleLeaveRoom = async (socket,jsonmsg,Userid) =>
{

  //make checks for if user in room if room still exist so on
  let isErrorCode=0;
  let content = JSON.parse(jsonmsg.Content)
  let jsonContent =
  {
    RoomName: content.RoomName
  };
  message = 
  {
    Type : messageTypes.RESLEAVEROOM,
    Content : JSON.stringify(jsonContent),
    ErrorCode : isErrorCode, 
    Timestamp: Date.now()
  }
  await roomsMan.RemoveUserFromRoom(content.RoomName,Userid)
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