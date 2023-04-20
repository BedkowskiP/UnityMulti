const userData = require('./userData');
const messageTypes = require('./messageTypes');
let user =
{

}

let message = {
    Type: '',
    Content: ''
};

let validateContent = {
    UserID :'',
    Username : '',
    Validated : '',
    ErrorCode : ''
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

const HandleValidation =  async (socket,msg) => 
{
  let jsonmsg = JSON.parse(msg)
  let content = JSON.parse(jsonmsg.Content)
  let id = await userData.createUserId();
  let usrn= "";
  /// ErrorCode 
  /// 0 - succes
  /// 1 - wrong username
  /// 2 - later
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

  validateContent = {
    UserID : id,
    Username : usrn,
    Validated : isValid,
    ErrorCode : isErrorCode
  }
  
  message = 
  {
    Type : messageTypes.RESVALIDATION,
    Content : JSON.stringify(validateContent)
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
      Timestamp: Date.now() // CHANGE LATER
    };
    socket.send(JSON.stringify(message));
  };
///
///ROOMS
///
const HandleCreateRoom = async (socket,msg) =>
{
  let jsonmsg = JSON.parse(msg)
  let content = JSON.parse(jsonmsg.Content)
}




///
///ROOMS END
///
module.exports = {
    HandleValidation,
    HandleCreateRoom,
    HandlePing
  };