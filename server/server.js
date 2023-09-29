const WebSocket = require('ws');
const msghand = require('./App/messageHandler')
const messageTypes = require('./App/messageTypes');
const DB = require('./App/database');
const usersMan = require('./App/usersManager');
const roomsMan = require('./App/roomsManager');
const logger = require('./logger')



const DEBUGMODE= true;   //shows incoming msgs
const TESTMODE = false;   //generate local users
const server = new WebSocket.Server({
  host: '0.0.0.0',
  port: 8080
});

debugger
const db = new DB.database(
  "localhost",
  "root",
  "betolo9528UM",
  "mysql")



const RoomLoop = async () => {
  while(true){  
    for (const name in roomsMan.Rooms)
    {
      console.log("Rooms created with name: ",roomsMan.Rooms[name].name,": Users - ",JSON.stringify(roomsMan.Rooms[name].users),"\nobjects - ",JSON.stringify(roomsMan.Rooms[name].objectList));
     
     //console.log("Rooms created with name: ",roomsMan.Rooms[name].name,": Users - ",JSON.stringify(roomsMan.Rooms[name].users));
      //console.log(roomsMan.Rooms[name].objectList.map(obj => JSON.stringify(obj)).join('\n')) 
    }
    await new Promise(resolve => setTimeout(resolve, 1000));
  }
};

process.argv.forEach(function (val, index) {
  if(index==2&&val=="remote");
});


RoomLoop();
//db.Connect();
//console.log(db.Query("select * from user"));


console.log('Starting WebSocket server: \n@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@@');

server.on('connection', (socket) => {
    console.log('Client connected');

    socket.on('message', async (data) => {
      HandleMessage(socket, data);
    });
  
    socket.on('close', (code) => {
      console.log('Client disconnected with code: '+code);
      for (let UserID in usersMan.Users) {
        if (usersMan.Users[UserID].socket === socket) {
          Disconnect(UserID,socket)
          break;
        }
      }
    });
});
const Disconnect =  async (UserID,socket)=>
{

  msg = {UserID:UserID,Content:JSON.stringify({RoomName:usersMan.Users[UserID].inRoom})}
  await msghand.HandleLeaveRoom(socket,msg)
  await usersMan.RemoveUser(UserID);
}
const HandleMessage = async (socket, message) => { 
    try 
    {
        const serverMessage = JSON.parse(message);
        if(DEBUGMODE){if(serverMessage.Type!=messageTypes.PING)logger.LogFile(message);}
        switch (serverMessage.Type) 
        {
            case messageTypes.REQVALIDATION:
                await msghand.HandleValidation(socket,serverMessage);
                break;
            case messageTypes.PING:
                msghand.HandlePing(socket,serverMessage);//test later
                break;
            case messageTypes.CREATEROOM:
                await msghand.HandleCreateRoom(socket,serverMessage);
                break;
            case messageTypes.JOINROOM:
                await msghand.HandleJoinRoom(socket,serverMessage);
                break;
            case messageTypes.LEAVEROOM:
                await msghand.HandleLeaveRoom(socket,serverMessage);
                break;
            case messageTypes.HOSTCHANGE:
                await msghand.HandleHostChange(socket,serverMessage);
                break;
            case messageTypes.UNITYOBJECT:
                await msghand.HandleObjectUnity(socket,serverMessage);
                break;                
            case messageTypes.SCENECHANGE:
                await msghand.HandleSceneChange(socket,serverMessage);
                break;
            case messageTypes.UNITYOBJECTUPDATE:
              await msghand.HandleObjectUnityUpdate(socket,serverMessage);
              break;
                break;
            case messageTypes.RPCMETHOD:
              await msghand.HandleRPCMethod(socket,serverMessage);
              break;
            case messageTypes.USER_DATA_RESPONSE:
                // handle user data response message
                break;
            case messageTypes.GAME_STATE:
                // handle game state message
                break;
            case messageTypes.PLAYER_POSITION:
                // handle player position message
                break;
            case messageTypes.PLAYER_ROTATION:
                // handle player rotation message
                break;
            case messageTypes.PLAYER_SCALE:
                // handle player scale message
                break;
            case messageTypes.SERVER_STATUS:
                // handle server status message
                break;
            case messageTypes.CHAT_MESSAGE:
                // handle chat message
                break;
            case messageTypes.SERVER_MESSAGE:
                // handle server message
                break;
            default:
                if (messageTypes.CUSTOM.includes(serverMessage.type)) 
                {
                  // handle custom message type
                } 
                else 
                {
                  // unknown message type
                }
                break;
        }
    } 
    catch (e) 
    {
        logger.ErrorFile(message);
        console.log(e)
    }
};
