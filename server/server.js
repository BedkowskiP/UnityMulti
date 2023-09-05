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
//Ghost object to test locally
//adding users and createing




async function waitAndDoSomething() {
  let waiter=2000;


  let Content = {Username:"betek",Password:null, UserID:null}
  let msg = {Type:messageTypes.REQVALIDATION, Content:JSON.stringify(Content),      Timestamp:Date.now(),      UserID:null};
  HandleMessage(null,JSON.stringify(msg));
  await new Promise(resolve => setTimeout(resolve, waiter));

  Content = {Username:"betek2",Password:null, UserID:null}
  msg =     {Type:messageTypes.REQVALIDATION, Content:JSON.stringify(Content),      Timestamp:Date.now(),      UserID:null};
  HandleMessage(null,JSON.stringify(msg));
  await new Promise(resolve => setTimeout(resolve, waiter));

  Content = {RoomName:'A',Password:null,IsPublic:true,MaxPlayers:10,SceneName:"TutorialSceneTwo"}
  msg =     {Type: messageTypes.CREATEROOM,   Content : JSON.stringify(Content),Timestamp: Date.now(),UserID :  Object.keys(usersMan.Users)[0]};
  HandleMessage(null,JSON.stringify(msg));
  await new Promise(resolve => setTimeout(resolve, waiter));

 

  Content = {RoomName:'A'}
  msg =     {Type: messageTypes.JOINROOM,     Content : JSON.stringify(Content),Timestamp: Date.now(), UserID : Object.keys(usersMan.Users)[1]};
  HandleMessage(null,JSON.stringify(msg));
  await new Promise(resolve => setTimeout(resolve, waiter));
/*
  Content = {UserID:Object.keys(usersMan.Users)[1]}
  msg =     {Type: messageTypes.HOSTCHANGE,   Content : JSON.stringify(Content),Timestamp: Date.now(),UserID : Object.keys(usersMan.Users)[0]};
  HandleMessage(null,JSON.stringify(msg));
  await new Promise(resolve => setTimeout(resolve, waiter));
  */

  Content = {PrefabName:'FabA',Position:{x:"1",y:"2",z:"6"},Rotation:{x:"1",y:"4",z:"5",w:"7"},Scale:{x:"11",y:"3",z:"2"},Owner:Object.keys(usersMan.Users)[0] }
  msg =     {Type: messageTypes.UNITYOBJECT,  Content : JSON.stringify(Content),Timestamp: Date.now(), UserID : Object.keys(usersMan.Users)[0] };
  HandleMessage(null,JSON.stringify(msg));
  await new Promise(resolve => setTimeout(resolve, waiter));

  Content = {PrefabName:'FabA',Position:{x:"1",y:"2",z:"6"},Rotation:{x:"1",y:"4",z:"5",w:"7"},Scale:{x:"11",y:"3",z:"2"},Owner: Object.keys(usersMan.Users)[1] }
  msg =     {Type: messageTypes.UNITYOBJECT,  Content : JSON.stringify(Content),Timestamp: Date.now(), UserID : Object.keys(usersMan.Users)[0] };
  HandleMessage(null,JSON.stringify(msg));
  await new Promise(resolve => setTimeout(resolve, waiter));
 /*
  Content = {SceneName:'Scene2'}
  msg =     {Type: messageTypes.SCENECHANGE,  Content : JSON.stringify(Content),Timestamp: Date.now(),UserID : Object.keys(usersMan.Users)[1]};
  HandleMessage(null,JSON.stringify(msg));
  await new Promise(resolve => setTimeout(resolve, waiter));

  Content = {SceneName:'Scene2'}
  msg =     {Type: messageTypes.SCENECHANGE,  Content : JSON.stringify(Content),Timestamp: Date.now(),UserID : Object.keys(usersMan.Users)[0]};
  HandleMessage(null,JSON.stringify(msg));
  await new Promise(resolve => setTimeout(resolve, waiter));

  Content = {RoomName:'Room1' }
  msg =     {Type: messageTypes.LEAVEROOM,    Content : JSON.stringify(Content),Timestamp: Date.now(),UserID : Object.keys(usersMan.Users)[1]};
  HandleMessage(null,JSON.stringify(msg));
  await new Promise(resolve => setTimeout(resolve, waiter));
*/
  Content = {RoomName:'A' }
  msg =     {Type: messageTypes.LEAVEROOM,    Content : JSON.stringify(Content),Timestamp: Date.now(),UserID : Object.keys(usersMan.Users)[1]};
  HandleMessage(null,JSON.stringify(msg));
  await new Promise(resolve => setTimeout(resolve, waiter));
  /*
  Content = {PrefabName:'FabA',Position:{x:"-0.941437542",y:"0.0",z:"0.0"},Rotation:{x:"1",y:"4",z:"5",w:"7"},Scale:{x:"11",y:"3",z:"2"},ObjID:"0",ObjName:"MultiObject(0)"}
  msg =     {Type: messageTypes.UNITYOBJECTUPDATE,  Content : JSON.stringify(Content),Timestamp: Date.now(), UserID : Object.keys(usersMan.Users)[0] };
  HandleMessage(null,JSON.stringify(msg));
  await new Promise(resolve => setTimeout(resolve, waiter));*/
  UsersLoop();
}

if(TESTMODE)waitAndDoSomething()

const UsersLoop = async () => {
  while(true){
    //for (const key in usersMan.Users) 
      //console.log("Users connected ID: ",usersMan.Users[key].id);
      
      let Content2 = {PrefabName:'FabA',Position:{x:"-0.941437542",y:"0.0",z:"0.0"},Rotation:{x:"1",y:"4",z:"5",w:"7"},Scale:{x:"11",y:"3",z:"2"},ObjID:"1",ObjName:"MultiObject(0)"}
      let msg2 =     {Type: messageTypes.UNITYOBJECTUPDATE,  Content : JSON.stringify(Content2),Timestamp: Date.now(), UserID : Object.keys(usersMan.Users)[0] };
      HandleMessage(null,JSON.stringify(msg2));
      for (const name in roomsMan.Rooms)console.log("Rooms created with name: ",roomsMan.Rooms[name].name,": Users - ",JSON.stringify(roomsMan.Rooms[name].users),": objects - ",JSON.stringify(roomsMan.Rooms[name].objectList));
      
    await new Promise(resolve => setTimeout(resolve, 1000));
  }
};

