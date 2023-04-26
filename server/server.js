const WebSocket = require('ws');
const msghand = require('./App/messageHandler')
const messageTypes = require('./App/messageTypes');
const DB = require('./App/database');
const roomsMan = require('./App/roomsManager');
const usersMan = require('./App/usersManager');

const DEBUGMODE= false;   //shows incoming msgs

const server = new WebSocket.Server({
  host: 'localhost',
  port: 8080
});


const db = new DB.database(
  "localhost",
  "root",
  "betolo9528UM",
  "mysql")

const UsersLoop = async () => {
  while(true){
    for (const key in usersMan.Users) 
      console.log("Users connected ID: ",usersMan.Users[key].id);
    await new Promise(resolve => setTimeout(resolve, 5000));
  }
};

const RoomLoop = async () => {
  while(true){  
    for (const name in roomsMan.Rooms)console.log("Rooms created with name: ",roomsMan.Rooms[name].name);
    await new Promise(resolve => setTimeout(resolve, 5000));
  }
};
const UserInRoomLoop = async () => {
  while(true){  
      console.log("List of Users in rooms: ",roomsMan.UserInRoom);

    await new Promise(resolve => setTimeout(resolve, 5000));
  }
};
process.argv.forEach(function (val, index) {
  if(index==2&&val=="remote");
});



UsersLoop();
//RoomLoop();
//db.Connect();
//UserInRoomLoop();
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
            delete usersMan.Users[UserID];
            //remove users from all instances on disconnet !!!!!!!!!!!!!!!
            break;
        }
      }
    });
});

const HandleMessage = async (socket, message) => { 
  try 
  {
    const serverMessage = JSON.parse(message);
    if(DEBUGMODE)console.log(serverMessage);
    switch (serverMessage.Type) {
      case messageTypes.REQVALIDATION:
        await msghand.HandleValidation(socket,serverMessage);
        break;
      case messageTypes.PING:
        await msghand.HandlePing(socket,serverMessage);//test later
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
      case messageTypes.DISCONNECT:
        // handle disconnect message
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
        if (messageTypes.CUSTOM.includes(serverMessage.type)) {
          // handle custom message type
        } else {
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

let Content = {
  Username:"betek",
  UserID:null
}
let message = 
    {
      Type: messageTypes.REQVALIDATION,
      Content : JSON.stringify(Content),
      Timestamp: Date.now(),
      UserID : null
    };
const TesterValid = async (message) =>
{
  
  await new Promise(resolve => setTimeout(resolve, 1000));
  await HandleMessage(null,message);
  
}
TesterValid(JSON.stringify(message));
TesterValid(JSON.stringify(message));


//create room
/*
const TesterCreate = async () =>
{
  
  await new Promise(resolve => setTimeout(resolve, 2000));
  let Content2 = {
    RoomName:'Room1',
    Password:null,
    IsPublic:true,
    MaxPlayers:10
  }
  
  let message2 = 
      {
        Type: messageTypes.CREATEROOM,
        Content : JSON.stringify(Content2),
        Timestamp: Date.now(),
        UserID : Object.keys(usersMan.Users)[0]
      };
  await HandleMessage(null,JSON.stringify(message2));
  
}
TesterCreate()

//join room



const Tester = async (message) =>
{
  await new Promise(resolve => setTimeout(resolve, 4000));
  Content = {
    RoomName:'Room1',
    Password:null
  }
  
  message = 
      {
        Type: messageTypes.JOINROOM,
        Content : JSON.stringify(Content),
        Timestamp: Date.now(),
        UserID : Object.keys(usersMan.Users)[1]
      };
  
  await HandleMessage(null,JSON.stringify(message));
  
}



 Tester();

//leave room


const Tester2 = async ( message) =>
{
  
  await new Promise(resolve => setTimeout(resolve, 6000));
  Content = {
    RoomName:'Room1' 
  }
  
  message = 
      {
        Type: messageTypes.LEAVEROOM,
        Content : JSON.stringify(Content),
        Timestamp: Date.now(),
        UserID : Object.keys(usersMan.Users)[0]
      };
    await HandleMessage(null,JSON.stringify(message));
    await new Promise(resolve => setTimeout(resolve, 1000));
  
  message = 
      {
        Type: messageTypes.LEAVEROOM,
        Content : JSON.stringify(Content),
        Timestamp: Date.now(),
        UserID : Object.keys(usersMan.Users)[1]
      };
  
  await HandleMessage(null,JSON.stringify(message));
  
}



Tester2((message));
*/

