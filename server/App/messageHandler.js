const userData = require('./userData');
const messageTypes = require('./messageTypes');
const { Room } = require('./room');
const roomsMan = require('./roomsManager')

let user =
{

}

let message = {
    Type: '',
    Content: ''
};


const getUserData = async (Content) => {
    let user = userData.user;
    user = JSON.parse(Content);

    if(user.userId == '') user.userId = createUserId();
    if(user.username == '') user.username = createUserId();

    let content = JSON.stringify(user);

    message = 
    {
        Type: 'userData',
        Content: content
    }

    return 0;
};

const HandleValidation =  async (socket,jsonmsg) => 
{
  let content = JSON.parse(jsonmsg.Content)
  let id = await userData.createUserId();
  let usrn= "";
  /// ErrorCode 100-200 validation
  /// 100 - succes
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
    isErrorCode = 100;
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
  user.id=id;
  user.name=usrn;
  user.socket=socket;
  user.valid=isValid;
  console.log(message);
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
const HandleCreateRoom = async (socket,jsonmsg) =>
{
  ///error 200-300 ROOMS
  let content = JSON.parse(jsonmsg.Content)
  let isErrorCode = 200;
  ///
  ///ADD CHECK for maxplayers/ ROOM NAME/PASSWORD/ISPUBLIC IF BAD ADD ERROR CODE
  ///
  let room = new Room(content.RoomName,socket,content.Password,content.IsPublic,content.MaxPlayers)
  await roomsMan.AddRoom(room);
  let jsonContent =
  {
    RoomName:content.RoomName,
  };
  message = 
  {
    Type : messageTypes.RESCREATEROOM,
    Content : JSON.stringify(jsonContent),
    ErrorCode : isErrorCode, //200 succesfully created room //201 to create failed beacuse something
    Timestamp: Date.now() // CHANGE LATER
  }
  //console.log(message);
  if(socket!=null)socket.send(JSON.stringify(message));
}
const HandleJoinRoom = async (socket,jsonmsg,Userid) =>
{
  let isErrorCode = 220;
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
    ErrorCode : isErrorCode, //220 Succesfully joined room // 201 failed to join room
    Timestamp: Date.now() // CHANGE LATER
  }
  console.log(message);
  if(socket!=null)socket.send(JSON.stringify(message));

}
const HandleLeaveRoom = async (socket,jsonmsg,Userid) =>
{
  let isErrorCode=230;
  alert("notimplemented");
  let jsonContent =
  {
    RoomName: "NOT DONE"
  };
  message = 
  {
    Type : messageTypes.RESLEAVEROOM,
    Content : JSON.stringify(jsonContent),
    ErrorCode : isErrorCode, 
    Timestamp: Date.now()
  }
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