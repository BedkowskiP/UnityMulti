const WebSocket = require('ws');
const msghand = require('./App/messageHandler')
const messageTypes = require('./App/messageTypes');
const DB = require('./App/database');

const { room } = require('./App/room');

let Users = {

};

let Rooms =
{

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

const clientsLoop = async () => {
  while(true){
    for (const userId in Users) {
      console.log(""+Users[userId].id);
      
    }
    //console.log(db.DBSTATE);
    await new Promise(resolve => setTimeout(resolve, 5000));
  }
};

process.argv.forEach(function (val, index) {
  if(index==2&&val=="remote");
});

/*let message = 
    {
      Type: 'Join/create/leave',
      Content:{
        RoomID : 111111,
        Password : 'null'
      },
      Timestamp: Date.now() // CHANGE LATER
    };
*///console.log(message);
clientsLoop();
//db.Connect();

//console.log(db.Query("select * from user"));


console.log('Starting WebSocket server: ');

server.on('connection', (socket) => {
    console.log('Client connected');

    socket.on('message', async (data) => {
      HandleMessage(socket, data);
    });
  
    socket.on('close', () => {
      console.log('Client disconnected');
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
        let user = await msghand.HandleValidation(socket,message);
        Users[user.id]=user;
        break;
      case messageTypes.PING:
        msghand.HandlePing(socket,message);
        break;
      case messageTypes.CREATEROOM:
        msghand.HandleCreateRoom();
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
    console.error('Received message error:', e.message, '\nMessage from server:', message);
  }
};



