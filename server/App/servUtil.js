const chars = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789';
const regex = '/^[0-9a-zA-Z_]+$/'

const createUserId = async (Users) => {
  let userId = '';
  let unique = false;
  while(!unique)
  {
    unique = true
    userId = '';
    for (let i = 0; i < 16; i++) {
      const randomIndex = Math.floor(Math.random() * chars.length);
      userId += chars[randomIndex];
      
    }
    for(let key in Users)if(key==userId)unique=false;
  }
  return userId;
};

module.exports = {createUserId}