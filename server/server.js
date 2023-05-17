const WebSocket = require('ws');
const msghand = require('./App/messageHandler')
const messageTypes = require('./App/messageTypes');
const DB = require('./App/database');
const roomsMan = require('./App/roomsManager');
const usersMan = require('./App/usersManager');
const tester = require('./App/Test')



const DEBUGMODE= true;   //shows incoming msgs
const TESTMODE = false;   //generate local users
const server = new WebSocket.Server({
  host: 'localhost',
  port: 8080
});

debugger
const db = new DB.database(
  "localhost",
  "root",
  "betolo9528UM",
  "mysql")

const UsersLoop = async () => {
  while(true){
    for (const key in usersMan.Users) 
      console.log("Users connected ID: ",usersMan.Users[key].id);
    await new Promise(resolve => setTimeout(resolve, 1000));
  }
};

const RoomLoop = async () => {
  while(true){  
    for (const name in roomsMan.Rooms)console.log("Rooms created with name: ",roomsMan.Rooms[name].name,": Users - ",JSON.stringify(roomsMan.Rooms[name].users));
    await new Promise(resolve => setTimeout(resolve, 1000));
  }
};

process.argv.forEach(function (val, index) {
  if(index==2&&val=="remote");
});



UsersLoop();
RoomLoop();
//db.Connect();
//console.log(db.Query("select * from user"));


console.log('Starting WebSocket server: ');

server.on('connection', (socket) => {
    console.log('Client connected');

    socket.on('message', async (data) => {
      HandleMessage(socket, data);
    });
  
    socket.on('close', (code) => {
      console.log('Client disconnected with code: '+code);
      for (let UserID in usersMan.Users) {
        if (usersMan.Users[UserID].socket === socket) {
          Disconnect(UserID)
          break;
        }
      }
    });
});
const Disconnect =  async (UserID)=>
{
  await roomsMan.RemoveUserFromRoom(UserID)
  await usersMan.RemoveUser(UserID);
}
const HandleMessage = async (socket, message) => { 
    try 
    {
        const serverMessage = JSON.parse(message);
        if(DEBUGMODE){if(serverMessage.Type!=messageTypes.PING)console.log(serverMessage);}
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
            case messageTypes.USER_DATA_REQUEST:
                // handle user data request message
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
        console.log('\x1b[41m%s\x1b[0m','Received message error:', e, '\nMessage from server:', message);
    }
};
//Ghost object to test locally
//adding users and createing




async function waitAndDoSomething() {
  let waiter=1000;


  let Content = {Username:"betek",Password:null, UserID:null}
  let msg = {Type:messageTypes.REQVALIDATION, Content:JSON.stringify(Content),      Timestamp:Date.now(),      UserID:null};
  HandleMessage(null,JSON.stringify(msg));
  await new Promise(resolve => setTimeout(resolve, waiter));

  Content = {Username:"betek2",Password:null, UserID:null}
  msg =     {Type:messageTypes.REQVALIDATION, Content:JSON.stringify(Content),      Timestamp:Date.now(),      UserID:null};
  HandleMessage(null,JSON.stringify(msg));
  await new Promise(resolve => setTimeout(resolve, waiter));

  Content = {RoomName:'Room1',Password:null,IsPublic:true,MaxPlayers:10,SceneName:"Default"}
  msg =     {Type: messageTypes.CREATEROOM,   Content : JSON.stringify(Content),Timestamp: Date.now(),UserID :  Object.keys(usersMan.Users)[0]};
  HandleMessage(null,JSON.stringify(msg));
  await new Promise(resolve => setTimeout(resolve, waiter));

  Content = {RoomName:'Room1'}
  msg =     {Type: messageTypes.JOINROOM,     Content : JSON.stringify(Content),Timestamp: Date.now(), UserID : Object.keys(usersMan.Users)[1]};
  HandleMessage(null,JSON.stringify(msg));
  await new Promise(resolve => setTimeout(resolve, waiter));
/*
  Content = {UserID:Object.keys(usersMan.Users)[1]}
  msg =     {Type: messageTypes.HOSTCHANGE,   Content : JSON.stringify(Content),Timestamp: Date.now(),UserID : Object.keys(usersMan.Users)[0]};
  HandleMessage(null,JSON.stringify(msg));
  await new Promise(resolve => setTimeout(resolve, waiter));
  
  Content = {PrefabName:'FabA',Position:{x:"1",y:"2",z:"6"},Rotation:{x:"1",y:"4",z:"5",w:"7"},Scale:{x:"11",y:"3",z:"2"},Owner:null }
  msg =     {Type: messageTypes.UNITYOBJECT,  Content : JSON.stringify(Content),Timestamp: Date.now(), UserID : Object.keys(usersMan.Users)[0] };
  HandleMessage(null,JSON.stringify(msg));
  await new Promise(resolve => setTimeout(resolve, waiter));

  Content = {PrefabName:'FabA',Position:{x:"1",y:"2",z:"6"},Rotation:{x:"1",y:"4",z:"5",w:"7"},Scale:{x:"11",y:"3",z:"2"},Owner:null }
  msg =     {Type: messageTypes.UNITYOBJECT,  Content : JSON.stringify(Content),Timestamp: Date.now(), UserID : Object.keys(usersMan.Users)[1] };
  HandleMessage(null,JSON.stringify(msg));
  await new Promise(resolve => setTimeout(resolve, waiter));
  */
/*
  Content = {SceneName:'Scene2'}
  msg =     {Type: messageTypes.SCENECHANGE,  Content : JSON.stringify(Content),Timestamp: Date.now(),UserID : Object.keys(usersMan.Users)[1]};
  HandleMessage(null,JSON.stringify(msg));
  await new Promise(resolve => setTimeout(resolve, waiter));

  Content = {SceneName:'Scene2'}
  msg =     {Type: messageTypes.SCENECHANGE,  Content : JSON.stringify(Content),Timestamp: Date.now(),UserID : Object.keys(usersMan.Users)[0]};
  HandleMessage(null,JSON.stringify(msg));
  await new Promise(resolve => setTimeout(resolve, waiter));
*/
  Content = {RoomName:'Room1' }
  msg =     {Type: messageTypes.LEAVEROOM,    Content : JSON.stringify(Content),Timestamp: Date.now(),UserID : Object.keys(usersMan.Users)[1]};
  HandleMessage(null,JSON.stringify(msg));
  await new Promise(resolve => setTimeout(resolve, waiter));

  Content = {RoomName:'Room1' }
  msg =     {Type: messageTypes.LEAVEROOM,    Content : JSON.stringify(Content),Timestamp: Date.now(),UserID : Object.keys(usersMan.Users)[0]};
  HandleMessage(null,JSON.stringify(msg));
  await new Promise(resolve => setTimeout(resolve, waiter));
  
}

if(TESTMODE)waitAndDoSomething()