
const messageTypes = require('./messageTypes');
const usersMan = require('./usersManager')
const roomsMan = require('./roomsManager')
const MSG = require('./message')

const DEBUGMODE = true;  //shows outcoming msgs                           

const HandleValidation =  async (socket,jsonmsg) => 
{
    let content = JSON.parse(jsonmsg.Content)
    let isErrorCode=0;
    let jsonContent = {}
    const UserID = await usersMan.CreateUser(socket);
    if(UserID===400)socket?.send(JSON.stringify(MSG.CreateMsg(messageTypes.RESVALIDATION,jsonContent,400,DEBUGMODE)));
    else
    {
        isErrorCode=usersMan.Users[UserID].SetName(content.Username)
        //isErrorCode=usersMan.Users[UserID].name=content.Username  //have to mkae custom setter beacuse setters cant return value i java script
        if(isErrorCode===0)
        {
            jsonContent =
            {
                UserID : UserID,
                Username : usersMan.Users[UserID].name,
                Validated : usersMan.Users[UserID].valid
            };
        }
        let msg = JSON.stringify(MSG.CreateMsg(messageTypes.RESVALIDATION,jsonContent,isErrorCode,DEBUGMODE))
        socket?.send(msg);
        
    }
}
const HandlePing = async (socket,jsonmsg) => 
{
    let isErrorCode = 0;
    let msg=JSON.stringify(MSG.CreateMsg(messageTypes.PONG,jsonmsg.Timestamp,isErrorCode,false))
    socket?.send(msg);
};

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
        HostID : jsonmsg.UserID,
        SceneName : content.SceneName
    };
    await roomsMan.AddRoom(content,jsonmsg.UserID);
    let msg = JSON.stringify((MSG.CreateMsg(messageTypes.RESCREATEROOM,jsonContent,isErrorCode,DEBUGMODE)))
    socket?.send(msg);
    HandleJoinRoom(socket,jsonmsg)//205 Await
}
const HandleJoinRoom = async (socket,jsonmsg) =>
{
    let isErrorCode = 0;
    let content = JSON.parse(jsonmsg.Content)
    await roomsMan.AddUserToRoom(
        content.RoomName,
        jsonmsg.UserID,
        usersMan.Users[jsonmsg.UserID].name); 
    ///check if room id/name exist
    let jsonContent =
    {
        Settings: { RoomName:content.RoomName,SceneName:roomsMan.Rooms[content.RoomName].sceneName,HostID:await roomsMan.Rooms[content.RoomName].host},
        UserList: await(roomsMan.Rooms[content.RoomName].users)//get list of users that joined given room   "list:[{UserID,Username},{UserID,Username}]"
    };
    let msg = JSON.stringify(MSG.CreateMsg(messageTypes.RESJOINROOM,jsonContent,isErrorCode,DEBUGMODE))
    socket?.send(msg);
    //broadcast to users in room
    jsonContent =
    {
        UserID : jsonmsg.UserID,
        Username : usersMan.Users[jsonmsg.UserID].name
    }
    msg = JSON.stringify((MSG.CreateMsg(messageTypes.USERJOIN,jsonContent,isErrorCode,DEBUGMODE)))
    
    roomsMan.BroadcastMsgToUsersInRoom(content.RoomName,msg,jsonmsg.UserID,usersMan.Users);
}

const HandleLeaveRoom = async (socket,jsonmsg) =>
{

  //make checks for if user in room if room still exist so on
  let isErrorCode=0;
  let content = JSON.parse(jsonmsg.Content)
  let jsonContent =
  {
      ame: content.RoomName
  };
  if(1!=await roomsMan.RemoveUserFromRoom(jsonmsg.UserID))//if function returns 1 it means room is about to be deleted no need to broadcast
  {
      //broadcast to users in room
      jsonContent =
      {
        UserID : jsonmsg.UserID,
        Username : usersMan.Users[jsonmsg.UserID].name
      }
      let msg = JSON.stringify((MSG.CreateMsg(messageTypes.USERLEAVE,jsonContent,isErrorCode,DEBUGMODE)))
      roomsMan.BroadcastMsgToUsersInRoom(content.RoomName,msg,jsonmsg.UserID,usersMan.Users);
  }
  let msg = JSON.stringify(MSG.CreateMsg(messageTypes.RESLEAVEROOM,jsonContent,isErrorCode,DEBUGMODE))
  socket?.send(msg);
  
}
const HandleHostChange = async (socket,MsgRecvived) =>
{
    
    //CHECK IF USER CHANGING HOST HAS PREMISSION!!!!!!!!!!!!!!!!!!!
    let content = JSON.parse(MsgRecvived.Content)
    const RoomName = await usersMan.Users[MsgRecvived.UserID].inRoom;
    if(content.UserID!=null)roomsMan.Rooms[RoomName].host=content.UserID;
    else roomsMan.ChooseNewHost(RoomName)
  
    
}

const HandleObjectUnity = async (socket,MsgRecvived) =>
{
    let isErrorCode=0;
    let content = JSON.parse(MsgRecvived.Content)
    const RoomName = await usersMan.Users[MsgRecvived.UserID].inRoom;
    const reuslt=roomsMan.Rooms[RoomName].AddObject(content,MsgRecvived.UserID)
    isErrorCode=reuslt.ErrorCode;
    let jsonContent =
    {
        PrefabName:content.PrefabName,                  
        ObjectID:reuslt.ObjectID,
        Position:{X:content.Position.x,   Y:content.Position.y,     Z:content.Position.z},
        Rotation:{X:content.Rotation.x,   Y:content.Rotation.y,     Z:content.Rotation.z,     W:content.Rotation.w},
        Scale:{X:content.Scale.x,     Y:content.Scale.y,    Z:content.Scale.z},
        Owner:MsgRecvived.UserID
    };
    let msg = JSON.stringify((MSG.CreateMsg(messageTypes.UNITYOBJECTRES,jsonContent,isErrorCode,1)))
    roomsMan.BroadcastMsgToUsersInRoom(RoomName,msg,null,usersMan.Users);//changed except from jsonmsg.UserID-> null
}
const HandleSceneChange  = async (socket,MsgRecvived) =>
{
    let isErrorCode=0;
    let contentMsgRecvived = JSON.parse(MsgRecvived.Content)
    const RoomName = await usersMan.Users[MsgRecvived.UserID].inRoom;

    isErrorCode=roomsMan.Rooms[RoomName].SetSceneName(contentMsgRecvived.SceneName,MsgRecvived.UserID);
    if(isErrorCode.ErrorCode!=0)
    {
        let msg = JSON.stringify((MSG.CreateMsg(messageTypes.RESSCENECHANGE,contentMsgRecvived,isErrorCode,DEBUGMODE)))
        socket?.send(msg);
    }
}
module.exports = {
    HandleValidation,
    HandleCreateRoom,
    HandleJoinRoom,
    HandleLeaveRoom,
    HandlePing,
    HandleHostChange,
    HandleObjectUnity,
    HandleSceneChange
  };