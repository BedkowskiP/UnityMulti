const chars = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789';

const createUserId = async () => {
    let userId = '';
    for (let i = 0; i < 16; i++) {
      const randomIndex = Math.floor(Math.random() * chars.length);
      userId += chars[randomIndex];
    }
    return userId;
};
const GenKey = async(length) =>
{
    let result = '';
    const characters = '0123456789';
    for (let i = 0; i < length; i++) {
        result += characters.charAt(
          Math.floor(Math.random() * characters.length));
    }
    return result;
}

const createUsername = async () => {
    let username = '';
    for (let i = 0; i < 8; i++) {
      const randomIndex = Math.floor(Math.random() * chars.length);
      username += chars[randomIndex];
    }
    return username;
};

let user = {
    userId: '',
    username: '',
};
const getUserData = async (socket, Content) => {
  let user = userData.user;
  user = JSON.parse(Content);

  if(user.userId == '') user.userId = await userData.createUserId();
  if(user.username == '') user.username = await userData.createUsername();

  console.log('Adding new client to dictionary');
  clients[user.userId] = {
    socket: socket,
    userId: user.userId
  };

  let content = JSON.stringify(user);

  message = {
      Type: 'userData',
      Content: content
  }

  for (let userId in clients) {
    if (clients[userId].socket === socket) {
        socket.send(JSON.stringify(message));
        break;
    }
  }

};
module.exports = {
    user,
    createUserId,
    createUsername,
    getUserData,
    GenKey
  };