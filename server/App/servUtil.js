const chars = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789';
const createUserId = async () => {
  let userId = '';
  for (let i = 0; i < 16; i++) {
    const randomIndex = Math.floor(Math.random() * chars.length);
    userId += chars[randomIndex];
  }
  return userId;
};

module.exports = {createUserId}