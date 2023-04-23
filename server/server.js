const WebSocket = require('ws');
const msghand = require('./App/messageHandler')
const messageTypes = require('./App/messageTypes');
const DB = require('./App/database');
const roomsMan = require('./App/roomsManager')


let Users = {

};



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
    for (const socket in Users) 
      console.log(Users[socket].id);
    await new Promise(resolve => setTimeout(resolve, 5000));
  }
};

const RoomLoop = async () => {
  while(true){  
    for (const name in roomsMan.Rooms) {
      console.log(roomsMan.Rooms[name].name);
    }
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



///console.log(message);
UsersLoop();
RoomLoop();
//db.Connect();
UserInRoomLoop();
//console.log(db.Query("select * from user"));


console.log('Starting WebSocket server: ');

server.on('connection', (socket) => {
    console.log('Client connected');

    socket.on('message', async (data) => {
      HandleMessage(socket, data);
    });
  
    socket.on('close', (code) => {
      console.log('Client disconnected with code: '+code);
      for (let userId in Users) {
        if (Users[userId].socket === socket) {
            delete Users[userId];
            break;
        }
      }
    });
});

const HandleMessage = async (socket, message) => { 
  try 
  {
    const serverMessage = JSON.parse(message);
    console.log(serverMessage);
    switch (serverMessage.Type) {
      case messageTypes.REQVALIDATION:
        let user = await msghand.HandleValidation(socket,serverMessage);
        Users[user.socket]=user;
        break;
      case messageTypes.PING:
        msghand.HandlePing(socket,serverMessage);
        break;
      case messageTypes.CREATEROOM:
        await msghand.HandleCreateRoom(socket,serverMessage,Users[socket].id);
        break;
      case messageTypes.JOINROOM:
        await msghand.HandleJoinRoom(socket,serverMessage,Users[socket].id);
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

    console.error('Received message error:', e, '\nMessage from server:', message);
  }
};
//Ghost object to test locally
/*
let testUser = 
{
  id:'test',
  name:'test',
  socket:'socket1',
  valid:true
}
let testUser2 = 
{
  id:'test2',
  name:'test2',
  socket:'socket2',
  valid:true
}
Users[testUser2.socket]=testUser2;
Users[testUser.socket]=testUser;
let Content = {
  RoomName:'RoomA',
  Password:null,
  IsPublic:true,
  MaxPlayers:10
}

let message = 
    {
      Type: messageTypes.CREATEROOM,
      Content : JSON.stringify(Content),
      Timestamp: Date.now()
    };

HandleMessage(testUser.socket,JSON.stringify(message))



Content = {
  RoomName:'RoomA',
  Password:null
}

message = 
    {
      Type: messageTypes.JOINROOM,
      Content : JSON.stringify(Content),
      Timestamp: Date.now()
    };

const Tester = async (Usr, message) =>
{
  
  await new Promise(resolve => setTimeout(resolve, 1000));
  console.log(message);
  msghand.HandleJoinRoom(Usr.socket,(message),Users[Usr.socket].id);
  
}
const Tester2 = async (Usr, message) =>
{
  
  await new Promise(resolve => setTimeout(resolve, 6000));
  console.log(message);
  msghand.HandleLeaveRoom(Usr.socket,(message),Users[Usr.socket].id);
  
}


Tester(testUser2,(message));

Content = {
  RoomName:'RoomA' 
}

message = 
    {
      Type: messageTypes.LEAVEROOM,
      Content : JSON.stringify(Content),
      Timestamp: Date.now()
    };

Tester2(testUser2,(message));

Tester2(testUser,(message));


*/