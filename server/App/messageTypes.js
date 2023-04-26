

/// these messages are used for room managment
/// Create,Join,Leave
const ROOMSMANAGE = "roomsManagement";
  ///
  ///Room Actions
  ///
  const LEAVEROOM = "leaveRoom";
  const JOINROOM = "joinRoom";
  const CREATEROOM = "createRoom"
  const RESCREATEROOM ="responseCreateRoom"
  const RESJOINROOM  ="responseJoinRoom"
  const RESLEAVEROOM = "responseLeaveRoom"
  ///
  ///Actions
  ///
  ///broadcast to notify users in room
  const USERJOIN = "userJoin";
  const USERLEAVE = "userLeave";

/// <summary>
/// these messages are used check latency.
/// </summary>
const PING = "ping";
const PONG = "pong";
/// <summary>
/// these message types could be used to establisz connection between client and server.
/// </summary>
const CONNECT = "connect";  ///NOT IMPLEMENTED
const DISCONNECT = "disconnect"; ///NOT IMPLEMENTED
/// <summary>
/// these message types could be used to request/send user data from/to server.
/// </summary>
const REQVALIDATION = "validationRequest";
const RESVALIDATION = "validationResponse";
const USER_DATA_REQUEST = "userDataRequest"; ///NOT IMPLEMENTED
const USER_DATA_RESPONSE = "userDataResponse"; ///NOT IMPLEMENTED
/// <summary>
/// something like closed instantions of the game (e.g. dungeons, missions)?
/// </summary>
const GAME_STATE = "gameState"; ///NOT IMPLEMENTED
/// <summary>
/// these message types could be used to send player position, rotation and scale.
/// </summary>
const PLAYER_POSITION = "playerPosition"; ///NOT IMPLEMENTED
const PLAYER_ROTATION = "playerRotation"; ///NOT IMPLEMENTED
const PLAYER_SCALE = "playerScale"; ///NOT IMPLEMENTED
/// <summary>
/// this message type could be used to send information about the server's status (e.g. number of clients connected, server load) to the clients.
/// </summary>
const SERVER_STATUS = "serverStatus"; ///NOT IMPLEMENTED
/// <summary>
/// this message type could be used to implement chat.
/// </summary>
const CHAT_MESSAGE = "chatMessage"; ///NOT IMPLEMENTED
/// <summary>
/// this message type could be used to send server related information to clients (e.g. server maintenance, scheduled downtime, etc.).
/// </summary>
const SERVER_MESSAGE = "serverMessage"; ///NOT IMPLEMENTED
/// <summary>
/// this message type could be used to make custom message types that aren't included as base message types.
/// </summary> 
let CUSTOM = [];

module.exports = {
  ROOMSMANAGE,
  CREATEROOM,
  JOINROOM,
  LEAVEROOM,
    RESVALIDATION,
    REQVALIDATION,
    PING,
    PONG,
    CONNECT,
    DISCONNECT,
    USER_DATA_REQUEST,
    USER_DATA_RESPONSE,
    GAME_STATE,
    PLAYER_POSITION,
    PLAYER_ROTATION,
    PLAYER_SCALE,
    SERVER_STATUS,
    CHAT_MESSAGE,
    SERVER_MESSAGE,
    CUSTOM,
    RESCREATEROOM,
    RESJOINROOM,
    RESLEAVEROOM,
    USERJOIN,
    USERLEAVE
  };